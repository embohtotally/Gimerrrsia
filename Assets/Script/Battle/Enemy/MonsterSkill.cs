using UnityEngine;

// THIS IS THE CORRECTED ENUM
public enum SkillEffect
{
    None,
    LowerDurability,
    RaiseDurability,
    RaiseAttack,
    DamageOverTime,
    RaiseDodge,      // New
    LowerDodge,      // New
    Heal,            // New
    MultiHit         // New
}

[CreateAssetMenu(fileName = "New Skill", menuName = "Pathos/Monster Skill")]
public class MonsterSkill : ScriptableObject
{
    public string skillName;
    public int spCost = 2;
    public int baseDamage = 5;
    public EmotionalState skillState;

    [Header("Special Effect")]
    public SkillEffect effect = SkillEffect.None;
    public int effectValue = 0;
    public int effectDuration = 2;
}