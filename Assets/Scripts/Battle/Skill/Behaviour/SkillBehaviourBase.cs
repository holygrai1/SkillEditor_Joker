using UnityEngine;
using System.Collections.Generic;
using JKFrame;

public abstract class SkillBehaviourBase
{
    protected ICharacter owner;
    protected SkillConfig skillConfig;
    protected SkillBrainBase skillBrain;
    protected Skill_Player skill_Player;
    protected bool canRotate = false;
    protected bool playing = false;
    protected float cdTimer;
    protected SkillLearnedData learnedData;
    public int skillIndex { get; private set; } // 角色配置中的技能索引
    public abstract SkillBehaviourBase DeepCopy();
    public virtual bool autoUpdateSlot { get => true; }
    private HashSet<IHitTarget> hitTargets;
    public int SkillLV => learnedData == null ? 1 : learnedData.lv;
    public virtual void Init(ICharacter owner, SkillConfig skillConfig, SkillBrainBase skillBrain, Skill_Player skill_Player, SkillLearnedData learnedData, int skillIndex)
    {
        this.owner = owner;
        this.skillConfig = skillConfig;
        this.skillBrain = skillBrain;
        this.skill_Player = skill_Player;
        this.learnedData = learnedData;
        this.skillIndex = skillIndex;
        hitTargets = new HashSet<IHitTarget>();
    }

    public virtual void OnUpdate()
    {
        UpdateCDTime();
        RotateOnUpdate();
        if (autoUpdateSlot)
        {
            UpdateSkillSlot();
        }
    }
    public virtual bool CheckRelease()
    {
        return CheckReleaseCost() && CheckReleaseCDTime();
    }

    public virtual float GetCDTime()
    {
        return skillConfig.GetCDTimeByLV(SkillLV);
    }

    public virtual bool CheckReleaseCDTime()
    {
        return cdTimer <= 0;
    }

    public virtual bool CheckReleaseCost()
    {
        foreach (KeyValuePair<SkillCostType, float> item in skillConfig.releaseCostDic)
        {
            if (!skillBrain.CheckCost(item.Key, item.Value))
            {
                return false;
            }
        }
        return true;
    }

    public virtual void Release(bool calCDTimer = true)
    {
        if (calCDTimer) cdTimer = GetCDTime();
        hitTargets.Clear();
        canRotate = false;
        playing = true;
        skillBrain.SetCanReleaseFlag(false);
        ApplyCosts();
    }

    public virtual void ApplyCosts()
    {
        foreach (KeyValuePair<SkillCostType, float> item in skillConfig.releaseCostDic)
        {
            skillBrain.ApplyCost(item.Key, item.Value);
        }
    }

    public virtual void UpdateCDTime()
    {
        if (cdTimer <= 0) return;
        cdTimer = Mathf.Clamp(cdTimer - Time.deltaTime, 0, float.MaxValue);
    }

    protected void UpdateSkillSlot()
    {
        if (TrGetSkillSlot(out UI_ShortcutSkillSlot slot))
        {
            OnUpdateSkillSlot(slot);
        }
    }

    protected bool TrGetSkillSlot(out UI_ShortcutSkillSlot slot)
    {
        return UISystem.GetWindow<UI_GameSceneMainWindow>().TryGetShortcutSkillSlotIndex(skillIndex, out slot);
    }

    protected virtual void OnUpdateSkillSlot(UI_ShortcutSkillSlot slot)
    {
        float max = skillConfig.GetCDTimeByLV(SkillLV);
        float value = 0;
        if (max != 0) value = cdTimer / max;
        slot.UpadteCDTime(value);
        slot.UpdateSkillReleaseState(CheckRelease());
    }

    protected virtual void RotateOnUpdate()
    {
        if (canRotate)
        {
            owner.OnSkillRotate();
        }
    }

    public virtual void OnReleaseNewSkill()
    {
        OnClipEndOrReleaseNewSkill();
    }
    public virtual void OnSkillClipEnd()
    {
        OnClipEndOrReleaseNewSkill();
    }
    public virtual void OnClipEndOrReleaseNewSkill()
    {
        playing = false;
        hitTargets.Clear();
    }
    #region 技能驱动时的事件
    public virtual void OnTickSkill(int frameIndex) { }
    public virtual SkillCustomEvent BeforeSkillCustomEvent(SkillCustomEvent customEvent) { return customEvent; }
    public virtual SkillAnimationEvent BeforeSkillAnimationEvent(SkillAnimationEvent animationEvent) { return animationEvent; }
    public virtual SkillAudioEvent BeforeSkillAudioEvent(SkillAudioEvent audioEvent) { return audioEvent; }
    public virtual SkillEffectEvent BeforeSkillEffectEvent(SkillEffectEvent effectEvent) { return effectEvent; }
    public virtual SkillAttackDetectionEvent BeforeSkillAttackDetectionEvent(SkillAttackDetectionEvent attackDetectionEvent) { return attackDetectionEvent; }

    public virtual void AfterSkillCustomEvent(SkillCustomEvent customEvent)
    {
        if (customEvent.EventType == SkillEventType.CanSkillRelease)
        {
            skillBrain.SetCanReleaseFlag(true);
        }
        else if (customEvent.EventType == SkillEventType.CanRotate)
        {
            canRotate = true;
        }
        else if (customEvent.EventType == SkillEventType.CantRotate)
        {
            canRotate = false;
        }
        else if (customEvent.EventType == SkillEventType.AddBuff)
        {
            owner.AddBuff((BuffConfig)customEvent.ObjectArg, customEvent.IntArg);
        }
    }
    public virtual void AfterSkillAnimationEvent(SkillAnimationEvent animationEvent) { }
    public virtual void AfterSkillAudioEvent(SkillAudioEvent audioEvent) { }
    public virtual void AfterSkillEffectEvent(SkillEffectEvent effectEvent) { }
    public virtual void AfterSkillAttackDetectionEvent(SkillAttackDetectionEvent attackDetectionEvent) { }
    public virtual void OnAttackDetection(IHitTarget hitTarget, AttackData attackData)
    {
        // 避免重复传递伤害行为与数据
        if (!hitTargets.Contains(hitTarget))
        {
            hitTargets.Add(hitTarget);
            OnHitTarget(hitTarget, attackData);
        }
    }

    public virtual void OnHitTarget(IHitTarget hitTarget, AttackData attackData)
    {
        if (attackData.detectionEvent.AttackHitConfig != null)
        {
            DoHitEffect(attackData);
        }
        hitTarget.BeHit(attackData);
    }

    protected void DoHitEffect(AttackData attackData)
    {
        AttackHitConfig attackHitConfig = attackData.detectionEvent.AttackHitConfig;
        if (attackHitConfig != null)
        {
            // 音效
            if (attackHitConfig.HitAudioClip != null) AudioSystem.PlayOneShot(attackHitConfig.HitAudioClip, attackData.hitPoint);
            // 特效
            if (attackHitConfig.HitEffectPrefab != null)
            {
                GameObject effect = ProjectUtility.GetOrInstantiateGameObject(attackHitConfig.HitEffectPrefab, null);
                effect.transform.position = attackData.hitPoint;
                effect.transform.LookAt(Camera.main.transform.position);
                effect.GetComponent<EffectController>().Init();
            }
        }
    }

    public virtual void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation) { }


    #endregion
}