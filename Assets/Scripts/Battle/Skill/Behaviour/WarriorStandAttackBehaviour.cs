using UnityEngine;

public class WarriorStandAttackBehaviour : Player_SkillBehaviourBase
{
    private int attackIndex = -1;
    [SerializeField] private int standAttackCount = 3;
    [SerializeField] private int sClip1Index = 3;
    public override bool autoUpdateSlot => false;
    public override SkillBehaviourBase DeepCopy()
    {
        return new WarriorStandAttackBehaviour()
        {
            standAttackCount = standAttackCount,
            sClip1Index = sClip1Index,
        };
    }

    public override void Release(bool calCDTimer = true)
    {
        base.Release(false);

        // 翻滚的特殊攻击
        if (skillBrain.TryGetShareData(WarriorSkillBrain.StandAttackSClip1, out bool sClip1) && sClip1)
        {
            skillBrain.AddOrUpdateShareData(WarriorSkillBrain.StandAttackSClip1, false);
            attackIndex = sClip1Index;
        }
        else
        {
            attackIndex += 1;
            if (attackIndex > standAttackCount - 1)
            {
                attackIndex = 0;
            }
        }

        skill_Player.StartPlaySkillBehaviour(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
    }

    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y += Time.deltaTime * -9.8f;
        owner.OnSkillMove(deltaPosition);
        owner.OnSkillRotate(deltaRotation);
    }
    public override void OnSkillClipEnd()
    {
        base.OnSkillClipEnd();
        owner.ChangeToIdleState();
    }
    public override void OnClipEndOrReleaseNewSkill()
    {
        base.OnClipEndOrReleaseNewSkill();
        skillBrain.TryGetShareData(WarriorSkillBrain.ContinuouStanrdAttackModelDataKey, out bool continuou);
        if (!continuou) attackIndex = -1;
        skillBrain.AddOrUpdateShareData(WarriorSkillBrain.ContinuouStanrdAttackModelDataKey, false);
    }
}