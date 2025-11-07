using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// An enum to define the types of characters
public enum CharacterType
{
    Player,
    Enemy
}

public class CharacterStats : MonoBehaviour
{
    [Header("Identity")]
    public CharacterType type;

    [Header("State & Skills")]
    public EmotionalState currentState = EmotionalState.Essence;
    public List<MonsterSkill> skills; // Used for enemies to define their available moves
    [Header("Core Stats")]
    public int maxHp = 100;
    public int currentHp;
    public int attack = 10;
    public int baseSkillPoints = 5;
    public int currentSkillPoints;

    public List<EmotionalState> weaknesses;
    public List<EmotionalState> resistances;

    [Tooltip("How much more damage to take from a weakness. 1.1 = 10% more damage.")]
    [Range(1f, 3f)]
    public float weaknessMultiplier = 1.1f;

    [Tooltip("How much damage to take from a resistance. 0.6 = 40% less damage.")]
    [Range(0f, 1f)]
    public float resistanceMultiplier = 0.6f;

    [Header("Defensive Stats")]
    public int shield = 0;
    [Range(0f, 1f)] // Shows as a slider in the Inspector
    public float durability = 0f; // Damage reduction, e.g., 0.1 = 10%
    [Range(0f, 1f)] // Shows as a slider in the Inspector
    public float dodgeRate = 0.05f; // 5% chance to dodge

    [Header("Buffs & Debuffs")]
    public List<Buff> activeBuffs = new List<Buff>();

    [Header("Connections")]
    public Animator animator;
    public List<Image> spIcons; // Used for the player's SP UI
    public Slider hpSlider;

    public void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
        // You would also call UpdateHpSlider() here if you have one
    }

    public float GetDodgeRate()
    {
        float finalDodge = dodgeRate;
        foreach (Buff buff in activeBuffs)
        {
            if (buff.effectType == SkillEffect.RaiseDodge)
                finalDodge += (float)buff.value / 100f; // e.g., value 20 becomes 0.20
            if (buff.effectType == SkillEffect.LowerDodge)
                finalDodge -= (float)buff.value / 100f;
        }
        return Mathf.Clamp01(finalDodge); // Clamp between 0% and 100%
    }

    public float GetDurability()
    {
        float finalDurability = durability;
        foreach (Buff buff in activeBuffs)
        {
            if (buff.effectType == SkillEffect.RaiseDurability)
                finalDurability += (float)buff.value / 100f;
            if (buff.effectType == SkillEffect.LowerDurability)
                finalDurability -= (float)buff.value / 100f;
        }
        return Mathf.Clamp(finalDurability, -1f, 0.9f); // Cap durability
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        currentHp = maxHp;

        // Set starting SP based on character type
        if (type == CharacterType.Player)
        {
            currentSkillPoints = 3;
        }
        else
        {
            currentSkillPoints = baseSkillPoints;
        }
    }

    public void SetupHpSlider()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;
        }
    }

    /// <summary>
    /// Applies damage to this character and triggers the TakeDamage animation.
    /// </summary>
    public void TakeDamage(int damage)
    {
        // The damage calculation logic will go here in the future.
        // For now, we just apply the base damage.
        currentHp -= damage; // Corrected from 'finalDamage' to 'damage'
        if (currentHp < 0) currentHp = 0;

        // Update the slider's value
        if (hpSlider != null)
        {
            hpSlider.value = currentHp;
        }

        if (animator != null)
        {
            animator.SetTrigger("TakeDamage");
        }
    }

    /// <summary>
    /// Updates the visibility of SP icons based on currentSkillPoints.
    /// </summary>
    public void UpdateSpUI()
    {
        for (int i = 0; i < spIcons.Count; i++)
        {
            if (i < currentSkillPoints)
            {
                spIcons[i].enabled = true;
            }
            else
            {
                spIcons[i].enabled = false;
            }
        }
    }

    /// <summary>
    /// Adds a new buff or debuff to this character from a skill.
    /// </summary>
    public void AddBuff(SkillEffect effect, int value, int duration)
    {
        if (effect == SkillEffect.None) return;

        Buff newBuff = new Buff
        {
            effectType = effect,
            value = value,
            turnsRemaining = duration
        };
        activeBuffs.Add(newBuff);
        Debug.Log($"{name} gained buff: {effect} for {duration} turns.");
    }

    /// <summary>
    /// Checks if this character currently has a specific buff active.
    /// </summary>
    public bool HasBuff(SkillEffect effect)
    {
        foreach (Buff buff in activeBuffs)
        {
            if (buff.effectType == effect)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Reduces the duration of all active buffs by one turn and removes expired ones.
    /// </summary>
    public void TickDownBuffs()
    {
        // Loop backwards through the list to safely remove items while iterating
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            activeBuffs[i].turnsRemaining--;
            if (activeBuffs[i].turnsRemaining <= 0)
            {
                Debug.Log($"{name}'s {activeBuffs[i].effectType} wore off.");
                activeBuffs.RemoveAt(i);
            }
        }
    }
}