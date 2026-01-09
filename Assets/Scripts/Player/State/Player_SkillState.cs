/// <summary>
/// 玩家移动状态
/// </summary>
public class Player_SkillState : PlayerStateBase
{
    private void PlaySkill()
    {
        // TODO:测试技能播放逻辑
        player.SkillBrain.ReleaseSkill(currentReleaseSkillIndex);
    }
    public override void Enter()
    {
        animation.AddAniamtionEvent("FootStep", OnFootStep);
        PlaySkill();
    }

    public override void Update()
    {
        if (CheckAndEnterSkillState())
        {
            PlaySkill();
        }
    }

    public override void Exit()
    {
        animation.RemoveAnimationEvent("FootStep", OnFootStep);
    }
}