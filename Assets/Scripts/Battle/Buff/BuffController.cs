using JKFrame;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    [ShowInInspector]
    protected Dictionary<BuffConfig, Buff> buffDic = new Dictionary<BuffConfig, Buff>();
    [SerializeField] private BuffEffectResolverBase buffEffectResolver;
    protected List<Buff> destroyBuffs = new List<Buff>();

    private void Update()
    {
        foreach (Buff item in buffDic.Values)
        {
            item.OnUpdate();
            if (item.destroyTimer <= 0)
            {
                destroyBuffs.Add(item);
            }
        }
        foreach (Buff item in destroyBuffs)
        {
            buffDic.Remove(item.config);
            item.Stop();
        }
        destroyBuffs.Clear();
    }

    [Button]
    public Buff AddBuff(BuffConfig buffConfig, int layer = 1)
    {
        if (buffDic.TryGetValue(buffConfig, out Buff buff))
        {
            buff.AddLayer(layer);
        }
        else
        {
            buff = ResSystem.GetOrNew<Buff>();
            buff.Init(buffConfig, OnBuffStart, OnBuffPeriodic, OnBuffEnd, OnBuffUpdate);
            buff.AddLayer(layer);
            buff.Start();
            buffDic.Add(buffConfig, buff);
        }
        return buff;
    }


    public void CleanBuffs()
    {
        foreach (Buff item in buffDic.Values)
        {
            item.Stop();
        }
        buffDic.Clear();
    }

    protected virtual void OnBuffStart(Buff buff)
    {
        if (buff.config.startEffect != null)
        {
            buffEffectResolver.Resolve(buff, buff.config.startEffect);
        }
    }

    protected virtual void OnBuffPeriodic(Buff buff)
    {
        buffEffectResolver.Resolve(buff, buff.config.periodicEffect);
    }

    protected virtual void OnBuffUpdate(Buff buff)
    {
    }

    protected virtual void OnBuffEnd(Buff buff)
    {
        if (buff.config.endEffect != null)
        {
            buffEffectResolver.Resolve(buff, buff.config.endEffect);
        }
    }
}
