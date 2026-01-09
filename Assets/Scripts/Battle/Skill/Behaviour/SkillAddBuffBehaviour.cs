using UnityEngine;

public class SkillAddBuffBehaviour : SkillBehaviourBase
{
    public override SkillBehaviourBase DeepCopy()
    {
        return new SkillAddBuffBehaviour();
    }

    public override void Release(bool calCDTimer = true)
    {
        base.Release(calCDTimer);
        skill_Player.StartPlaySkillBehaviour(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[0]);
    }

    public override void OnSkillClipEnd()
    {
        base.OnSkillClipEnd();
        owner.ChangeToIdleState();

    }
    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y += Time.deltaTime * -9.8f;
        owner.OnSkillMove(deltaPosition);
        owner.OnSkillRotate(deltaRotation);
    }
}
