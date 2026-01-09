using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class CharacterProperties : SerializedMonoBehaviour
{
    [ShowInInspector] public float currentHP { get; private set; }
    [ShowInInspector] public float currentMP { get; private set; }
    public FloatProperties maxHP = new FloatProperties();
    public FloatProperties maxMP = new FloatProperties();
    public FloatProperties attack = new FloatProperties();

    // TODO:基于存档恢复当前血量等信息
    public void Init(CharacterConfig characterConfig, float currentHP = 100, float currentMP = 100)
    {
        maxHP.Init(characterConfig.hpBaseValue, null, null, null, OnMaxHPChanged);
        maxMP.Init(characterConfig.mpBaseValue, null, null, null, OnMaxMPChanged);
        attack.Init(characterConfig.attackBaseValue, null, null, null, null);
        this.currentHP = currentHP;
        this.currentMP = currentMP;
    }

    public void AddHP(float add)
    {
        SetHP(currentHP + add);
    }
    public void SetHP(float value)
    {
        currentHP = Mathf.Clamp(value, 0, maxHP.Total);
        // TODO:同步给UI
    }
    public void AddMP(float add)
    {
        SetMP(currentMP + add);
    }
    public void SetMP(float value)
    {
        currentMP = Mathf.Clamp(value, 0, maxMP.Total);
        // TODO:同步给UI
    }

    private void OnMaxHPChanged(float oldMaxHP, float newMaxHP)
    {
        // 当最大值变化时候，当前值根据之前的比列同步变化
        float proportion = currentHP / oldMaxHP;
        currentHP = newMaxHP * proportion;
        // TODO:同步给UI
    }

    private void OnMaxMPChanged(float oldMaxMP, float newMaxMP)
    {
        // 当最大值变化时候，当前值根据之前的比列同步变化
        float proportion = currentMP / oldMaxMP;
        currentMP = newMaxMP * proportion;
        // TODO:同步给UI
    }
}

public class FloatProperties
{
    [SerializeField] private float baseValue;
    [SerializeField] private float fixedBonus;
    [SerializeField] private float multiplierBonus;

    private Action<float, float> onBaseValueChangedAction;
    private Action<float, float> onFixedValueChangedAction;
    private Action<float, float> onMultiplierValueChangedAction;
    private Action<float, float> onTotalValueChangedAction;

    public void Init(float baseValue, Action<float, float> onBaseValueChangedAction, Action<float, float> onFixedValueChangedAction, Action<float, float> onMultiplierValueChangedAction, Action<float, float> onTotalValueChangedAction)
    {
        this.BaseValue = baseValue;
        this.onBaseValueChangedAction = onBaseValueChangedAction;
        this.onFixedValueChangedAction = onFixedValueChangedAction;
        this.onMultiplierValueChangedAction = onMultiplierValueChangedAction;
        this.onTotalValueChangedAction = onTotalValueChangedAction;
    }
    public float Total => baseValue + FixedBonus + (baseValue * MultiplierBonus);

    public float BaseValue
    {
        get => baseValue;
        set
        {
            onBaseValueChangedAction?.Invoke(baseValue, value);
            if (onTotalValueChangedAction != null)
            {
                float oldTotal = Total;
                baseValue = value;
                onTotalValueChangedAction?.Invoke(oldTotal, Total);
            }
            else baseValue = value;
        }
    }

    public float FixedBonus
    {
        get => fixedBonus;
        set
        {
            onFixedValueChangedAction?.Invoke(fixedBonus, value);
            if (onTotalValueChangedAction != null)
            {
                float oldTotal = Total;
                fixedBonus = value;
                onTotalValueChangedAction?.Invoke(oldTotal, Total);
            }
            else fixedBonus = value;
        }
    }
    public float MultiplierBonus
    {
        get => multiplierBonus;
        set
        {
            onMultiplierValueChangedAction?.Invoke(multiplierBonus, value);
            if (onTotalValueChangedAction != null)
            {
                float oldTotal = Total;
                multiplierBonus = value;
                onTotalValueChangedAction?.Invoke(oldTotal, Total);
            }
            else multiplierBonus = value;
        }
    }
}
