using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro; // Make sure this is here

public enum BattleState { INACTIVE, START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    [Header("Stage References")]
    public GameObject mindscapeStage;
    public GameObject battleStage;
    public GameObject battleBackground;
    public Camera explorationCamera;
    public Camera battleCamera;
    private float originalLightIntensity;
    public float battleLightIntensity = 0.2f;
    public float lightFadeDuration = 0.75f; 
    [Header("Prefabs & Spawn Points")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    [Header("UI Connections")]
    public List<Image> playerSpIcons;
    public Slider playerHpSlider;
    public TMP_Text outcomeText;    // <-- NEW
    public Slider enemyHpSlider;

    // Private variables
    private List<EmotionalState> queuedActions = new List<EmotionalState>();
    private GameObject playerGO;
    private GameObject enemyGO;
    private CharacterStats playerStats;
    private CharacterStats enemyStats;
    private PlayerMovement playerOnMap;
    public GameObject damagePopupPrefab; // <-- ADD THIS
    public Canvas battleCanvas;          // <-- ADD THIS
    public Light2D globalLight;
    public Transform playerBuffsPanel;
    public Transform enemyBuffsPanel;
    public GameObject buffIconPrefab;

    public Transform queuedActionsPanel;
    public GameObject actionIconPrefab;

    [Header("UI Sprites")]
    public Sprite raiseDurabilityIcon;
    public Sprite lowerDurabilityIcon;
    public Sprite damageOverTimeIcon;
    public Sprite essenceIcon;
    public Sprite delightIcon;
    public Sprite griefIcon;
    public Sprite temperIcon;

    private MonsterSkill lastEnemySkillUsed;

    void ShowDamagePopup(CharacterStats target, int damage, EmotionalState state, string message = null)
    {
        // Convert the character's world position to a screen position for the UI
        Vector3 screenPosition = battleCamera.WorldToScreenPoint(target.transform.position);

        // Add a random offset to prevent stacking
        float randomXOffset = Random.Range(-40f, 40f);
        float yOffset = 30f; // <-- This moves the popup up
        Vector3 spawnPosition = screenPosition + new Vector3(randomXOffset, 0, 0);

        // Create an instance of the popup prefab
        GameObject popupGO = Instantiate(damagePopupPrefab, spawnPosition, Quaternion.identity, battleCanvas.transform);
        DamagePopup popup = popupGO.GetComponent<DamagePopup>();

        if (popup != null)
        {
            // --- THIS IS THE CORRECTED LOGIC ---
            if (message != null)
            {
                // If a special message (like "DODGED!") is provided, use the text setup.
                popup.SetupAsText(message);
            }
            else
            {
                // Otherwise, use the normal damage number setup.
                popup.Setup(damage, state);
            }
        }
    }

    void ShowDodgePopup(CharacterStats target)
    {
        Vector3 screenPosition = battleCamera.WorldToScreenPoint(target.transform.position);
        float randomXOffset = Random.Range(-40f, 40f);
        float yOffset = 30f;
        Vector3 spawnPosition = screenPosition + new Vector3(randomXOffset, yOffset, 0);
        GameObject popupGO = Instantiate(damagePopupPrefab, spawnPosition, Quaternion.identity, battleCanvas.transform);
        DamagePopup popup = popupGO.GetComponent<DamagePopup>();
        if (popup != null)
        {
            popup.SetupAsDodge();
        }
    }


    void Start()
    {
        
        battleStage.SetActive(false);
        if (battleBackground != null) battleBackground.SetActive(false);
        if (battleCamera != null) battleCamera.gameObject.SetActive(false);
        state = BattleState.INACTIVE;
    }

    public void StartBattle(GameObject enemyToSpawn, PlayerMovement originalPlayer)
    {
        this.playerOnMap = originalPlayer;
        Rigidbody2D playerRigid = originalPlayer.GetComponent<Rigidbody2D>();
        playerRigid.simulated = false;
        if (outcomeText != null) outcomeText.gameObject.SetActive(false);
        //AudioManager.instance.PlayMusic("Battle"); 
        //AudioManager.instance.MusicVolume(1.0f); 
        // --- NEW LIGHTING LOGIC ---
        if (globalLight != null)
        {
            originalLightIntensity = globalLight.intensity; // Save the original value
            globalLight.intensity = 1f; // Set to 100% for battle
        }

        // --- Hide Map, Show Battle ---
        if (mindscapeStage != null) mindscapeStage.SetActive(false);
        if (explorationCamera != null) explorationCamera.gameObject.SetActive(false);
        battleStage.SetActive(true);
        if (battleBackground != null) battleBackground.SetActive(true);
        if (battleCamera != null) battleCamera.gameObject.SetActive(true);
        state = BattleState.START;
        StartCoroutine(SetupBattle(enemyToSpawn));
    }


    IEnumerator TransitionToBattle(GameObject enemyToSpawn)
    {
        if (globalLight != null)
        {
            originalLightIntensity = globalLight.intensity;

            float timer = 0f;
            while (timer < lightFadeDuration)
            {
                timer += Time.deltaTime;
                globalLight.intensity = Mathf.Lerp(originalLightIntensity, battleLightIntensity, timer / lightFadeDuration);
                yield return null;
            }
        }

        // --- Now that it's dark, set up the battle ---
        if (mindscapeStage != null) mindscapeStage.SetActive(false);
        if (explorationCamera != null) explorationCamera.gameObject.SetActive(false);

        battleStage.SetActive(true);
        if (battleBackground != null) battleBackground.SetActive(true);
        if (battleCamera != null) battleCamera.gameObject.SetActive(true);

        state = BattleState.START;
        StartCoroutine(SetupBattle(enemyToSpawn));
    }


    IEnumerator SetupBattle(GameObject enemyToSpawn)
    {
        playerGO = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        playerStats = playerGO.GetComponent<CharacterStats>();
        playerGO.GetComponent<Animator>().SetFloat("Horizontal", 1);
        playerStats.spIcons = this.playerSpIcons;
        playerStats.hpSlider = this.playerHpSlider;
        playerStats.SetupHpSlider();
        playerStats.UpdateSpUI();

        enemyGO = Instantiate(enemyToSpawn, enemySpawnPoint.position, Quaternion.identity);
        enemyStats = enemyGO.GetComponent<CharacterStats>();
        enemyStats.hpSlider = this.enemyHpSlider;
        enemyStats.SetupHpSlider();

        yield return new WaitForSeconds(1f);
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        playerStats.TickDownBuffs();
        playerStats.currentSkillPoints += 2;
        playerStats.currentSkillPoints = Mathf.Min(playerStats.currentSkillPoints, playerStats.baseSkillPoints);
        playerStats.UpdateSpUI();
        queuedActions.Clear();
        UpdateQueuedActionsUI();
    }

    public void OnPlayerAction(EmotionalState actionState)
    {
        int cost = (actionState == EmotionalState.Essence) ? 1 : 2;
        if (playerStats.currentSkillPoints < cost) return;
        playerStats.currentSkillPoints -= cost;
        playerStats.UpdateSpUI();
        queuedActions.Add(actionState);
        UpdateQueuedActionsUI(); 

    }

    public void OnConfirmAttackButton()
    {
        if (state != BattleState.PLAYERTURN || queuedActions.Count == 0) return;
        StartCoroutine(ExecutePlayerActions());
        UpdateQueuedActionsUI();
    }

    IEnumerator ExecutePlayerActions()
    {
        // You would disable your action buttons here to prevent more input
        // SetActionButtons(false);

        foreach (EmotionalState action in queuedActions)
        {
            // --- 1. APPLY BOON/DEBUFF FROM THE ACTION ---
            switch (action)
            {
                case EmotionalState.Delight:
                    // Applies a Durability boost to the player for 1 turn
                    playerStats.AddBuff(SkillEffect.RaiseDurability, 50, 1);
                    break;
                case EmotionalState.Grief:
                    // Applies Vulnerable to the enemy for 2 turns
                    enemyStats.AddBuff(SkillEffect.LowerDurability, 50, 2);
                    break;
                case EmotionalState.Temper:
                    // Applies a DoT of 5 damage for 2 turns to the enemy
                    enemyStats.AddBuff(SkillEffect.DamageOverTime, 5, 2);
                    break;
            }

            // Update the UI to show the new buffs immediately
            UpdateBuffIcons(playerStats, playerBuffsPanel);
            UpdateBuffIcons(enemyStats, enemyBuffsPanel);

            // --- 2. CALCULATE DAMAGE (this now includes the dodge check) ---
            float damage = CalculateDamage(action, playerStats, enemyStats);

            if (damage < 0) // A negative value means it was a dodge
            {
                ShowDodgePopup(enemyStats);
            }
            else // Otherwise, it was a normal hit
            {
                int damageInt = (int)damage;
                enemyStats.TakeDamage(damageInt);
                ShowDamagePopup(enemyStats, damageInt, action);
            }

            yield return new WaitForSeconds(0.75f);
            if (enemyStats.currentHp <= 0) break;
        }

        // --- 4. AFTER ALL ACTIONS ARE DONE, END THE TURN ---
        if (enemyStats.currentHp <= 0)
        {
            state = BattleState.WON;
            StartCoroutine(EndBattle(true));
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    // --- THIS IS THE CORRECTED ENEMY TURN METHOD ---
    IEnumerator EnemyTurn()
    {
        // --- 1. Handle Start-of-Turn Effects (like Damage-over-Time) ---
        foreach (Buff buff in new List<Buff>(enemyStats.activeBuffs))
        {
            if (buff.effectType == SkillEffect.DamageOverTime)
            {
                Debug.Log($"{enemyStats.name} takes {buff.value} damage from Temper!");
                enemyStats.TakeDamage(buff.value);
                ShowDamagePopup(enemyStats, buff.value, EmotionalState.Temper);
            }
        }

        // Check for defeat from DoT before proceeding
        if (enemyStats.currentHp <= 0)
        {
            state = BattleState.WON;
            StartCoroutine(EndBattle(true));
            yield break; // End the turn immediately if DoT was lethal
        }

        // --- 2. Upkeep Phase (Tick down buffs, gain SP) ---
        enemyStats.TickDownBuffs();
        // UpdateBuffIcons(enemyStats, enemyBuffsPanel); // If you have buff icons
        enemyStats.currentSkillPoints = enemyStats.baseSkillPoints;
        // enemyStats.UpdateSpUI(); // If you have enemy SP icons

        yield return new WaitForSeconds(1f); // Pause to let the player see DoT/buff changes

        // --- 3. Action Phase (AI chooses and uses a skill) ---
        if (enemyStats.skills.Count > 0)
        {
            MonsterSkill chosenSkill = enemyStats.skills[Random.Range(0, enemyStats.skills.Count)];

            // Prevent using MultiHit twice in a row
            if (chosenSkill.effect == SkillEffect.MultiHit && lastEnemySkillUsed != null && lastEnemySkillUsed.effect == SkillEffect.MultiHit)
            {
                // If we chose MultiHit again, try to pick another skill
                chosenSkill = enemyStats.skills[Random.Range(0, enemyStats.skills.Count)];
            }
            lastEnemySkillUsed = chosenSkill;

            if (enemyStats.currentSkillPoints >= chosenSkill.spCost)
            {
                Debug.Log($"Enemy uses {chosenSkill.skillName}!");
                enemyStats.currentSkillPoints -= chosenSkill.spCost;
                // enemyStats.UpdateSpUI();

                // Handle the skill's unique effect
                switch (chosenSkill.effect)
                {
                    case SkillEffect.Heal:
                        enemyStats.Heal(chosenSkill.effectValue);
                        break;
                    case SkillEffect.RaiseDodge:
                    case SkillEffect.RaiseDurability:
                    case SkillEffect.RaiseAttack:
                        enemyStats.AddBuff(chosenSkill.effect, chosenSkill.effectValue, chosenSkill.effectDuration);
                        break;
                    case SkillEffect.LowerDodge:
                    case SkillEffect.LowerDurability:
                        playerStats.AddBuff(chosenSkill.effect, chosenSkill.effectValue, chosenSkill.effectDuration);
                        break;
                    case SkillEffect.MultiHit:
                        StartCoroutine(PerformMultiHit(chosenSkill));
                        break;
                }

                // Apply direct damage if the skill has any
                if (chosenSkill.baseDamage > 0)
                {
                    float damage = CalculateDamage(chosenSkill.skillState, enemyStats, playerStats);
                    int damageInt = (int)damage;
                    playerStats.TakeDamage(damageInt);
                    ShowDamagePopup(playerStats, damageInt, chosenSkill.skillState);
                }
            }
            else
            {
                Debug.Log("Enemy doesn't have enough SP for its skill.");
            }
        }

        yield return new WaitForSeconds(1.5f); // Wait for the action to resolve visually

        // --- 4. End of Turn Phase (Check for player defeat) ---
        if (playerStats.currentHp <= 0)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle(false));
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    IEnumerator PerformMultiHit(MonsterSkill skill)
    {
        Debug.Log("Consecutive Attack!");
        // The first hit is applied in the main EnemyTurn logic.
        // This coroutine just handles the follow-up strike.
        yield return new WaitForSeconds(0.75f);

        Debug.Log("Enemy follows up with a second strike!");
        // The second hit is a basic Essence attack for simplicity.
        float damage = CalculateDamage(EmotionalState.Essence, enemyStats, playerStats);
        switch (EmotionalState.Essence)
        {
            case EmotionalState.Essence:
                AudioManager.instance.PlaySFX("Essence"); 
                break;
            case EmotionalState.Delight:
                AudioManager.instance.PlaySFX("Delight"); 
                break;
            case EmotionalState.Grief:
                AudioManager.instance.PlaySFX("Grief"); 
                break;
            case EmotionalState.Temper:
                AudioManager.instance.PlaySFX("Temper"); 
                break;
        }
        int damageInt = (int)damage;
        playerStats.TakeDamage(damageInt);
        ShowDamagePopup(playerStats, damageInt, EmotionalState.Essence);
    }

    IEnumerator EndBattle(bool playerWon)
    {
        // 1. Display the outcome text and trigger the correct "Die" animation
        if (playerWon)
        {
            if (outcomeText != null) outcomeText.text = "You've Won";
            if (enemyStats != null && enemyStats.animator != null)
                enemyStats.animator.SetTrigger("Die");
        }
        else
        {
            if (outcomeText != null) outcomeText.text = "Defeated";
            if (playerStats != null && playerStats.animator != null)
                playerStats.animator.SetTrigger("Die");
        }

        if (outcomeText != null)
            outcomeText.gameObject.SetActive(true);
        // 2. Wait for a few seconds so the player can see the message and animation
        yield return new WaitForSeconds(2.5f);

        // 3. Restore the original exploration lighting
        if (globalLight != null)
        {
            globalLight.intensity = originalLightIntensity;
        }

        // 4. Switch the stages and cameras back to exploration mode
        if (battleStage != null) battleStage.SetActive(false);
        if (battleBackground != null) battleBackground.SetActive(false);
        if (battleCamera != null) battleCamera.gameObject.SetActive(false);
        if (mindscapeStage != null) mindscapeStage.SetActive(true);
        if (explorationCamera != null) explorationCamera.gameObject.SetActive(true);

        // 5. Clean up the temporary player and enemy objects created for the battle
        if (playerGO != null) Destroy(playerGO);
        if (enemyGO != null) Destroy(enemyGO);

        // 6. Re-enable the player's movement script on the map
        if (playerOnMap != null)
        {
            playerOnMap.enabled = true;
        }

        // 7. Reset the battle system's state to be ready for the next fight
        state = BattleState.INACTIVE;

        //AudioManager.instance.PlayMusic("Theme");
        //AudioManager.instance.MusicVolume(1.0f);  // 50% volume
    }



    IEnumerator TransitionFromBattle()
    {
        if (globalLight != null)
        {
            float currentIntensity = globalLight.intensity;

            float timer = 0f;
            while (timer < lightFadeDuration)
            {
                timer += Time.deltaTime;
                globalLight.intensity = Mathf.Lerp(currentIntensity, originalLightIntensity, timer / lightFadeDuration);
                yield return null;
            }
        }

        // --- Now that light is restored, return to the map ---
        battleStage.SetActive(false);
        if (battleBackground != null) battleBackground.SetActive(false);
        if (battleCamera != null) battleCamera.gameObject.SetActive(false);
        if (mindscapeStage != null) mindscapeStage.SetActive(true);
        if (explorationCamera != null) explorationCamera.gameObject.SetActive(true);

        Destroy(playerGO);
        Destroy(enemyGO);

        if (playerOnMap != null) playerOnMap.enabled = true;
        state = BattleState.INACTIVE;
    }

    void UpdateBuffIcons(CharacterStats character, Transform panel)
    {
        // Clear existing icons
        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }

        // Add an icon for each active buff
        foreach (Buff buff in character.activeBuffs)
        {
            GameObject iconGO = Instantiate(buffIconPrefab, panel);
            Image iconImage = iconGO.GetComponent<Image>();

            // Assign the correct sprite based on the effect
            switch (buff.effectType)
            {
                case SkillEffect.RaiseDurability:
                    iconImage.sprite = raiseDurabilityIcon;
                    break;
                case SkillEffect.LowerDurability:
                    iconImage.sprite = lowerDurabilityIcon;
                    break;
                case SkillEffect.DamageOverTime:
                    iconImage.sprite = damageOverTimeIcon;
                    break;
            }
        }
    }

    float CalculateDamage(EmotionalState attackerState, CharacterStats attacker, CharacterStats defender)
    {
        float currentDamage = attacker.attack; // Start with the attacker's base damage.
        if (Random.value < defender.GetDodgeRate())
        {
            Debug.Log($"{defender.name} dodged the attack!");
            // You can add a "DODGE!" popup here
            return 0f; // Return 0 damage
        }

        float baseDamage = attacker.attack;

        // Use the new GetDurability() method to account for buffs
        float totalDurability = defender.GetDurability();
        float damageAfterDurability = baseDamage * (1f - totalDurability);

        if (Random.Range(0f, 1f) < defender.dodgeRate)
        {
            Debug.Log($"{defender.name} DODGED the attack!");
            return 0; // We use -1 as a special code for a successful dodge.
        }

        // --- 1. Apply Defender's Durability (Base Stat + Buffs/Debuffs) ---
        if (defender.HasBuff(SkillEffect.RaiseDurability))
        {
            totalDurability += 0.50f; // 50% durability boost from Delight
        }
        if (defender.HasBuff(SkillEffect.LowerDurability)) // Vulnerable from Grief
        {
            totalDurability -= 0.50f; // 50% durability reduction (takes more damage)
        }
        // Cap durability to prevent 100% or more reduction/increase
        totalDurability = Mathf.Clamp(totalDurability, -1f, 0.9f);

        // Apply the durability calculation to the damage. (damage = damage * (1 - reduction))
        currentDamage *= (1f - totalDurability);


        // --- 2. Apply Weakness & Resistance Multipliers from Lists ---
        if (defender.weaknesses.Contains(attackerState))
        {
            currentDamage *= defender.weaknessMultiplier;
        }
        else if (defender.resistances.Contains(attackerState))
        {
            currentDamage *= defender.resistanceMultiplier;
        }


        // --- 3. Apply Same-State Nullification ---
        if (attackerState != EmotionalState.Essence && attackerState == defender.currentState)
        {
            currentDamage = 0;
        }

        // Ensure damage is never negative and return it
        return Mathf.Max(0, currentDamage);
    }

    void ApplyBoon(EmotionalState state, CharacterStats character)
    {
        switch (state)
        {
            case EmotionalState.Delight:
                character.shield += 10;
                break;
        }
    }
    void UpdateQueuedActionsUI()
    {
        // Clear existing icons
        foreach (Transform child in queuedActionsPanel)
        {
            Destroy(child.gameObject);
        }

        // Add an icon for each queued action
        foreach (EmotionalState action in queuedActions)
        {
            GameObject iconGO = Instantiate(actionIconPrefab, queuedActionsPanel);
            Image iconImage = iconGO.GetComponent<Image>();

            // Assign the correct sprite based on the state
            switch (action)
            {
                case EmotionalState.Essence:
                    iconImage.sprite = essenceIcon;
                    break;
                case EmotionalState.Delight:
                    iconImage.sprite = delightIcon;
                    break;
                case EmotionalState.Grief:
                    iconImage.sprite = griefIcon;
                    break;
                case EmotionalState.Temper:
                    iconImage.sprite = temperIcon;
                    break;
            }
        }
    }

    public void OnEssenceButton() {
        OnPlayerAction(EmotionalState.Essence); }
    public void OnDelightButton() {
        OnPlayerAction(EmotionalState.Delight); }
    public void OnGriefButton() {
        OnPlayerAction(EmotionalState.Grief); }
    public void OnTemperButton() {
        OnPlayerAction(EmotionalState.Temper); }
}