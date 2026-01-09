using JKFrame;
using System;
using UnityEngine;

public class Buff
{
    public BuffConfig config { get; private set; }
    public float destroyTimer { get; private set; }
    public float periodicTimer { get; private set; }
    public int layer { get; private set; }

    private Action<Buff> onStart;
    private Action<Buff> onPeriodic;
    private Action<Buff> onEnd;
    private Action<Buff> onUpdate;

    public void Init(BuffConfig config, Action<Buff> onStart, Action<Buff> onPeriodic, Action<Buff> onEnd, Action<Buff> onUpdate)
    {
        this.config = config;
        this.onStart = onStart;
        this.onPeriodic = onPeriodic;
        this.onEnd = onEnd;
        this.onUpdate = onUpdate;
    }

    public void Start()
    {
        destroyTimer = config.duration;
        periodicTimer = config.periodicTime;
        onStart?.Invoke(this);
        Debug.Log("Start生效一次");
    }

    public void OnUpdate()
    {
        // 周期性生效
        if (onPeriodic != null && config.periodicEffect != null)
        {
            periodicTimer -= Time.deltaTime;
            if (periodicTimer <= 0)
            {
                // 生效一次
                onPeriodic.Invoke(this);
                periodicTimer = config.periodicTime + periodicTimer;
                Debug.Log("周期性生效一次");
            }
        }

        // 销毁倒计时
        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0)
        {
            if (layer > 1) // 层数下降
            {
                layer -= 1;
                destroyTimer = config.duration;
            }
            else // Buff结束
            {
                onEnd?.Invoke(this);
                Debug.Log("OnEnd生效一次");
            }
        }
        else
        {
            onUpdate?.Invoke(this);
        }
    }
    public void Stop()
    {
        config = null;
        onStart = null;
        onPeriodic = null;
        onEnd = null;
        this.ObjectPushPool();
    }
    public void AddLayer(int layer)
    {
        if (config.canStack)
        {
            this.layer = Mathf.Clamp(this.layer + layer, 0, config.maxLayer);
        }
        else
        {
            this.layer = 1;
        }
        // 刷新存在时间
        destroyTimer = config.duration;
    }
}
