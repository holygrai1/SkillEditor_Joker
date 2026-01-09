using UnityEngine;

public class WarriorSkill1Behaviour : Player_SkillBehaviourBase
{
    #region 配置
    public float standingTime = 5;
    private Color normalColor = new Color(0, 0, 0, 0.3f);
    private Color standingColor = new Color(1, 0, 0, 0.3f);
    #endregion

    // -1意味着没有进入技能，0、1、2代表在技能中（并不是技能播放中）
    private int attackIndex = -1;
    public override bool autoUpdateSlot => false;
    public override SkillBehaviourBase DeepCopy()
    {
        return new WarriorSkill1Behaviour();
    }

    public override void Release(bool calCDTimer = true)
    {
        base.Release(false);
        playing = true;
        attackIndex += 1;
        // 如果是最后一段则立刻进入完整CD
        if (attackIndex == skillConfig.Clips.Length - 1) cdTimer = GetCDTime();
        else cdTimer = standingTime;
        skill_Player.StartPlaySkillBehaviour(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
        // 让普攻连续
        skillBrain.AddOrUpdateShareData(WarriorSkillBrain.ContinuouStanrdAttackModelDataKey, true);
    }
    public override bool CheckRelease()
    {
        bool checkCD = true;
        if (attackIndex == -1) checkCD = cdTimer <= 0; // 未释放状态
        else if (attackIndex == skillConfig.Clips.Length - 1) checkCD = cdTimer <= 0;// 释放最后一段的状态
        return checkCD && base.CheckReleaseCost();
    }

    public override void UpdateCDTime()
    {
        if (playing)
        {
            // 播放状态:技能处于最后一段的技能，已经在计算CD中
            if (attackIndex == skillConfig.Clips.Length - 1)
            {
                cdTimer = Mathf.Clamp(cdTimer - Time.deltaTime, 0, float.MaxValue);
            }
            // 播放状态:技能没有处于最后一段的技能，不计算任何CD
        }
        else
        {
            cdTimer = Mathf.Clamp(cdTimer - Time.deltaTime, 0, float.MaxValue);
            // 技能处于某一段
            if (attackIndex != -1)
            {
                // 技能已经播放完某一段，但是没有播放完整个技能，同时开始已经进入CD了！
                if (cdTimer <= 0)
                {
                    cdTimer = GetCDTime();
                    attackIndex = -1;
                }
                // 技能没有播放完某一段，正在计算内部CD
            }
        }

        if (TrGetSkillSlot(out UI_ShortcutSkillSlot slot))
        {
            int iconIndex = attackIndex + 1; // 预期的下一个技能索引
            if (iconIndex >= skillConfig.skillIcons.Length) iconIndex = 0;
            slot.UpdateIcon(skillConfig.skillIcons[iconIndex]);
            bool standing = iconIndex != 0;
            float showMaxCD = standing ? standingTime : GetCDTime();
            slot.UpdateCDTimeAndMaskColor(cdTimer / showMaxCD, standing ? standingColor : normalColor);
        }
    }
    public override void OnSkillClipEnd()
    {
        base.OnSkillClipEnd();
        owner.ChangeToIdleState();
    }

    public override void OnClipEndOrReleaseNewSkill()
    {
        base.OnClipEndOrReleaseNewSkill();
        // 当前结束的是最后一段，说明技能全部结束了
        if (attackIndex == skillConfig.Clips.Length - 1)
        {
            attackIndex = -1;
        }
        // 结束的是中间某一段技能
        else
        {
            cdTimer = standingTime;
        }
    }

    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y += Time.deltaTime * -9.8f;
        owner.OnSkillMove(deltaPosition);
        owner.OnSkillRotate(deltaRotation);
    }
}
