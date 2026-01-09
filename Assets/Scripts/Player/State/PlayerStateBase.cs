using JKFrame;

/// <summary>
/// 玩家状态的基类
/// </summary>
public abstract class PlayerStateBase : StateBase
{
    protected Animation_Controller animation;
    protected Player_Controller player;
    protected static int currentReleaseSkillIndex;
    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        player = (Player_Controller)owner;
        animation = player.Animation_Controller;

    }

    // TODO:临时测试逻辑
    protected bool CheckAndEnterSkillState()
    {
        if (UISystem.CheckMouseOnUI()) return false;

        // 默认0是普攻
        for (int i = 0; i < player.SkillBrain.SkillCount; i++)
        {
            bool vaild = false;
            // 技能对应角色配置中的技能索引
            int skillIndex = player.SkillBrain.GetSkillIndex(i);
            if (i == 0) // 鼠标普攻的专门检测
            {
                vaild = InputManager.Instance.GetStandAttackState() && player.SkillBrain.CheckReleaseSkill(i);
                if (vaild) InputManager.Instance.ResetStandAttackCacheTimer();
            }

            if (!vaild) // 有可能普攻也放在技能快捷栏中
            {
                vaild = InputManager.Instance.GetSkillKeyState(skillIndex) && player.SkillBrain.CheckReleaseSkill(i);
            }

            if (vaild)
            {
                currentReleaseSkillIndex = i;
                InputManager.Instance.ResetSkillKeyCodeCacheTimer(skillIndex);
                player.ChangeState(PlayerState.Skill);
                return true;
            }
        }
        return false;
    }
    protected void OnFootStep()
    {
        int index = UnityEngine.Random.Range(0, player.CharacterConfig.FootStepAudioClips.Length);
        AudioSystem.PlayOneShot(player.CharacterConfig.FootStepAudioClips[index], player.transform.position, false, 0.2f);
    }
}
