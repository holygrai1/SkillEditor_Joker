using UnityEngine;

public struct AttackData
{
    public SkillAttackDetectionEvent detectionEvent;
    public ICharacter soure;
    public Vector3 hitPoint;
    public float attackValue;
}
