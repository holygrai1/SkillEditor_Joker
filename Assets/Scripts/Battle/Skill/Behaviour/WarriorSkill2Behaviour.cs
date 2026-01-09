using UnityEngine;

public class WarriorSkill2Behaviour : Player_SkillBehaviourBase
{
    public override SkillBehaviourBase DeepCopy()
    {
        return new WarriorSkill2Behaviour();
    }

    public override void Release(bool calCDTimer = true)
    {
        base.Release(true);
        skill_Player.StartPlaySkillBehaviour(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[0]);
        skillBrain.AddOrUpdateShareData(WarriorSkillBrain.StandAttackSClip1, true);
    }
    public override void OnSkillClipEnd()
    {
        skillBrain.AddOrUpdateShareData(WarriorSkillBrain.StandAttackSClip1, false);
        owner.ChangeToIdleState();
    }
    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y += Time.deltaTime * -9.8f;
        owner.OnSkillMove(deltaPosition);
        owner.OnSkillRotate(deltaRotation);
    }
}