using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillEffectEventInspector : SkillEventDataInspectorBase<EffectTrackItem, EffectTrack>
{
    private IntegerField effectDurationFiled;
    public override void OnDraw()
    {
        // 预制体
        ObjectField effectPrefabAssetField = new ObjectField("特效预制体");
        effectPrefabAssetField.objectType = typeof(GameObject);
        effectPrefabAssetField.value = trackItem.SkillEffectEvent.Prefab;
        effectPrefabAssetField.RegisterValueChangedCallback(EffectPrefabAssetFiedlValueChanged);
        root.Add(effectPrefabAssetField);

        // 坐标
        Vector3Field posFiled = new Vector3Field("坐标");
        posFiled.value = trackItem.SkillEffectEvent.Position;
        posFiled.RegisterValueChangedCallback(EffectPosFiledValueChanged);
        root.Add(posFiled);

        // 旋转
        Vector3Field rotFiled = new Vector3Field("旋转");
        rotFiled.value = trackItem.SkillEffectEvent.Rotation;
        rotFiled.RegisterValueChangedCallback(EffectRotFiledValueChanged);
        root.Add(rotFiled);

        // 旋转
        Vector3Field scaleFiled = new Vector3Field("缩放");
        scaleFiled.value = trackItem.SkillEffectEvent.Scale;
        scaleFiled.RegisterValueChangedCallback(EffectScaleFiledValueChanged);
        root.Add(scaleFiled);

        // 自动销毁
        Toggle autoDestructToggle = new Toggle("自动销毁");
        autoDestructToggle.value = trackItem.SkillEffectEvent.AutoDestruct;
        autoDestructToggle.RegisterValueChangedCallback(EffectAutoDestructToggleValueChanged);
        root.Add(autoDestructToggle);

        // 时间
        effectDurationFiled = new IntegerField("持续时间");
        effectDurationFiled.value = trackItem.SkillEffectEvent.Duration;
        effectDurationFiled.RegisterCallback<FocusInEvent>(EffectDurationFiledFocusIn);
        effectDurationFiled.RegisterCallback<FocusOutEvent>(EffectDurationFiledFocusOut);
        root.Add(effectDurationFiled);

        // 时间计算按钮
        Button calculateEffectButton = new Button(CalculateEffectDuration);
        calculateEffectButton.text = "重新计时";
        root.Add(calculateEffectButton);

        // 引用模型Transform属性
        Button applyModelTransformDataButton = new Button(ApplyModelTransformData);
        applyModelTransformDataButton.text = "引用模型Transform属性";
        root.Add(applyModelTransformDataButton);

        // 设置持续帧数至选中帧
        Button setFrameButton = new Button(SetEffectDurationFrameButton);
        setFrameButton.text = "设置持续帧数至选中帧";
        root.Add(setFrameButton);
    }
    private void ApplyModelTransformData()
    {
        EffectTrackItem effectTrackItem = trackItem;
        effectTrackItem.ApplyModelTransformData();
        SkillEditorInspector.Instance.Show();
    }

    private void CalculateEffectDuration()
    {
        EffectTrackItem effectTrackItem = trackItem;
        ParticleSystem[] particleSystems = effectTrackItem.SkillEffectEvent.Prefab.GetComponentsInChildren<ParticleSystem>();

        float max = -1;
        int curr = -1;
        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (particleSystems[i].main.duration > max)
            {
                max = particleSystems[i].main.duration;
                curr = i;
            }
        }

        effectTrackItem.SkillEffectEvent.Duration = (int)(particleSystems[curr].main.duration * SkillEditorWindow.Instance.SkillConfig.FrameRote);
        effectDurationFiled.value = effectTrackItem.SkillEffectEvent.Duration;
        effectTrackItem.ResetView();
    }

    private void EffectPrefabAssetFiedlValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        EffectTrackItem effectTrackItem = trackItem;
        effectTrackItem.SkillEffectEvent.Prefab = evt.newValue as GameObject;
        // 重新计时
        CalculateEffectDuration();
        effectTrackItem.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }

    private void EffectPosFiledValueChanged(ChangeEvent<Vector3> evt)
    {
        EffectTrackItem effectTrackItem = trackItem;
        effectTrackItem.SkillEffectEvent.Position = evt.newValue;
        effectTrackItem.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }

    private void EffectRotFiledValueChanged(ChangeEvent<Vector3> evt)
    {
        EffectTrackItem effectTrackItem = trackItem;
        effectTrackItem.SkillEffectEvent.Rotation = evt.newValue;
        effectTrackItem.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }

    private void EffectScaleFiledValueChanged(ChangeEvent<Vector3> evt)
    {
        EffectTrackItem effectTrackItem = trackItem;
        effectTrackItem.SkillEffectEvent.Scale = evt.newValue;
        effectTrackItem.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }

    private void EffectAutoDestructToggleValueChanged(ChangeEvent<bool> evt)
    {
        EffectTrackItem effectTrackItem = trackItem;
        effectTrackItem.SkillEffectEvent.AutoDestruct = evt.newValue;
    }

    float oldEffectDurationFiled;
    private void EffectDurationFiledFocusIn(FocusInEvent evt)
    {
        oldEffectDurationFiled = effectDurationFiled.value;
    }
    private void EffectDurationFiledFocusOut(FocusOutEvent evt)
    {
        if (effectDurationFiled.value != oldEffectDurationFiled)
        {
            EffectTrackItem effectTrackItem = trackItem;
            effectTrackItem.SkillEffectEvent.Duration = effectDurationFiled.value;
            effectTrackItem.ResetView();
            SkillEditorWindow.Instance.TickSkill();
        }
    }

    private void SetEffectDurationFrameButton()
    {
        EffectDurationFiledFocusIn(null);
        effectDurationFiled.value = SkillEditorWindow.Instance.CurrentSelectFrameIndex - trackItem.FrameIndex;
        EffectDurationFiledFocusOut(null);
    }
}
