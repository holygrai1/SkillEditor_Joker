using JKFrame;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class SkillBrainBase : MonoBehaviour
{
    [SerializeField] protected Skill_Player skill_Player;
    [ShowInInspector] protected List<SkillBehaviourBase> skillBehaviours;
    public virtual int lastReleaseBehaviourIndex { get; protected set; } = -1;
    public virtual bool canRelease { get; protected set; }
    public int SkillCount => skillBehaviours.Count;

    public virtual void Init(ICharacter owner)
    {
        canRelease = true;
        skill_Player.Init(owner, owner.Animation_Controller, owner.ModelTransform);
    }

    public void AddSkill(Player_Controller player, List<SkillConfig> skillConfigs, int skillIndex, SkillLearnedData skillLearnedData = null)
    {
        SkillConfig skillConfig = skillConfigs[skillIndex];
        SkillBehaviourBase skillBehaviour = skillConfig.Behaviour.DeepCopy();
        skillBehaviour.Init(player, skillConfig, this, skill_Player, skillLearnedData, skillIndex);
        skillBehaviours.Add(skillBehaviour);
    }
    public int GetSkillIndex(int behaviourIndex)
    {
        return skillBehaviours[behaviourIndex].skillIndex;
    }

    public virtual void SetCanReleaseFlag(bool newValue)
    {
        canRelease = newValue;
    }

    public bool CheckReleaseSkill(int behaviourIndex)
    {
        return canRelease && skillBehaviours[behaviourIndex].CheckRelease();
    }
    public virtual void ReleaseSkill(int behaviourIndex)
    {
        if (lastReleaseBehaviourIndex != -1 && lastReleaseBehaviourIndex != behaviourIndex)
        {
            skillBehaviours[lastReleaseBehaviourIndex].OnReleaseNewSkill();
        }
        skillBehaviours[behaviourIndex].Release();
        lastReleaseBehaviourIndex = behaviourIndex;
    }
    protected virtual void Update()
    {
        for (int i = 0; i < skillBehaviours.Count; i++)
        {
            skillBehaviours[i].OnUpdate();
        }
    }

    public virtual bool CheckCost(SkillCostType costType, float cost)
    {
        // TODO:和上一层做对接（如PlayerController）
        return true;
    }
    // 技能的消耗代价做实际的扣除
    public virtual void ApplyCost(SkillCostType costType, float cost)
    {
        // TODO:和上一层做对接（如PlayerController）
        Debug.Log($"释放技能的代价{costType}:{cost}");
    }
    #region 共享数据
    protected interface ISkillShareData { }
    protected class SkillShareData<T> : ISkillShareData
    {
        public T value;
    }

    private Dictionary<string, ISkillShareData> shareDataDic = new Dictionary<string, ISkillShareData>();

    protected SkillShareData<T> GetSkillShareData<T>()
    {
        return ResSystem.GetOrNew<SkillShareData<T>>();
    }

    protected void DestroySkillShareData(ISkillShareData obj)
    {
        obj.ObjectPushPool();
    }

    public void AddShareData<T>(string key, T value)
    {
        SkillShareData<T> skillShareData = GetSkillShareData<T>();
        skillShareData.value = value;
        shareDataDic.Add(key, skillShareData);
        if (sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            ((SharedDataEventData<T>)sharedDataEventData).TriggerCreate(value);
            ((SharedDataEventData<T>)sharedDataEventData).TriggerChanged(value);
        }
    }
    public void AddOrUpdateShareData<T>(string key, T value)
    {
        if (shareDataDic.TryGetValue(key, out ISkillShareData skillShareData))
        {
            ((SkillShareData<T>)skillShareData).value = value;
            if (sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
            {
                ((SharedDataEventData<T>)sharedDataEventData).TriggerChanged(value);
            }
        }
        else AddShareData<T>(key, value);
    }
    public bool ContainsShareData(string key)
    {
        return shareDataDic.ContainsKey(key);
    }
    public bool TryGetShareData<T>(string key, out T value)
    {
        bool res = shareDataDic.TryGetValue(key, out ISkillShareData data);
        if (res) value = ((SkillShareData<T>)data).value;
        else value = default;
        return res;
    }
    public void RemoveShareData<T>(string key)
    {
        if (shareDataDic.Remove(key, out ISkillShareData data))
        {
            if (sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
            {
                sharedDataEventData.TriggerRemove();
            }
            DestroySkillShareData(data);
        }
    }
    public void CleanShareData()
    {
        foreach (KeyValuePair<string, ISkillShareData> item in shareDataDic)
        {
            DestroySkillShareData(item.Value);
            if (sharedDataEventDic.TryGetValue(item.Key, out ISharedDataEventData sharedDataEventData))
            {
                sharedDataEventData.TriggerRemove();
            }
        }
        shareDataDic.Clear();
    }

    #endregion

    #region 共享数据相关事件
    private interface ISharedDataEventData
    {
        public void TriggerRemove();
    }
    private class SharedDataEventData<T> : ISharedDataEventData
    {
        public Action<T> onCreate;
        public Action<T> onChanged;
        public Action onRemove;
        public void TriggerCreate(T value)
        {
            onCreate?.Invoke(value);
        }
        public void TriggerChanged(T value)
        {
            onChanged?.Invoke(value);
        }
        public void TriggerRemove()
        {
            onRemove?.Invoke();
        }
    }
    private Dictionary<string, ISharedDataEventData> sharedDataEventDic = new Dictionary<string, ISharedDataEventData>();

    public void AddSharedDataCreateEventListener<T>(string key, Action<T> action)
    {
        if (!sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            SharedDataEventData<T> eventData = new SharedDataEventData<T>();
            eventData.onCreate += action;
            sharedDataEventDic.Add(key, eventData);
        }
        else
        {
            SharedDataEventData<T> eventData = (SharedDataEventData<T>)sharedDataEventData;
            eventData.onCreate += action;
        }
    }
    public void RemoveSharedDataCreateEventListener<T>(string key, Action<T> action)
    {
        if (sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            SharedDataEventData<T> eventData = (SharedDataEventData<T>)sharedDataEventData;
            eventData.onCreate -= action;
        }
    }

    public void AddSharedDataChangedEventListener<T>(string key, Action<T> action)
    {
        if (!sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            SharedDataEventData<T> eventData = new SharedDataEventData<T>();
            eventData.onChanged += action;
            sharedDataEventDic.Add(key, eventData);
        }
        else
        {
            SharedDataEventData<T> eventData = (SharedDataEventData<T>)sharedDataEventData;
            eventData.onChanged += action;
        }
    }
    public void RemoveSharedDataChangedEventListener<T>(string key, Action<T> action)
    {
        if (sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            SharedDataEventData<T> eventData = (SharedDataEventData<T>)sharedDataEventData;
            eventData.onChanged -= action;
        }
    }

    public void AddSharedDataRemoveEventListener<T>(string key, Action action)
    {
        if (!sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            SharedDataEventData<T> eventData = new SharedDataEventData<T>();
            eventData.onRemove += action;
            sharedDataEventDic.Add(key, eventData);
        }
        else
        {
            SharedDataEventData<T> eventData = (SharedDataEventData<T>)sharedDataEventData;
            eventData.onRemove += action;
        }
    }
    public void RemoveSharedDataRemoveEventListener<T>(string key, Action action)
    {
        if (sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            SharedDataEventData<T> eventData = (SharedDataEventData<T>)sharedDataEventData;
            eventData.onRemove -= action;
        }
    }
    #endregion

}
