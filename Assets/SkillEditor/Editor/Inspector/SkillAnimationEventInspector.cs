using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillAnimationEventInspector : SkillEventDataInspectorBase<AnimationTrackItem, AnimationTrack>
{
    private Label clipFrameLabel;
    private Toggle rootMotionToggle;
    private Toggle mainWeaponOnLeftHandToggle;
    private Label isLoopLable;
    private IntegerField durationField;
    private FloatField transitionTimeField;

    public override void OnDraw()
    {
        // 动画资源
        ObjectField animationClipAssetField = new ObjectField("动画资源");
        animationClipAssetField.objectType = typeof(AnimationClip);
        animationClipAssetField.value = trackItem.AnimationEvent.AnimationClip;
        animationClipAssetField.RegisterValueChangedCallback(AnimationClipAssetFiedlValueChanged);
        root.Add(animationClipAssetField);

        // 根运动
        rootMotionToggle = new Toggle("应用根运动");
        rootMotionToggle.value = trackItem.AnimationEvent.ApplyRootMotion;
        rootMotionToggle.RegisterValueChangedCallback(RootMotionToggleValueChanged);
        root.Add(rootMotionToggle);

        // 主武器左右手
        mainWeaponOnLeftHandToggle = new Toggle("武器左手位置");
        mainWeaponOnLeftHandToggle.value = trackItem.AnimationEvent.MainWeaponOnLeftHand;
        mainWeaponOnLeftHandToggle.RegisterValueChangedCallback(MainWeaponOnLeftHandValueChanged);
        root.Add(mainWeaponOnLeftHandToggle);

        // 轨道长度
        durationField = new IntegerField("轨道长度");
        durationField.value = trackItem.AnimationEvent.DurationFrame;
        durationField.RegisterCallback<FocusInEvent>(DurationFieldFocusIn);
        durationField.RegisterCallback<FocusOutEvent>(DurationFieldFocusOut);
        root.Add(durationField);

        // 过渡时间
        transitionTimeField = new FloatField("过渡时间");
        transitionTimeField.value = trackItem.AnimationEvent.TransitionTime;
        transitionTimeField.RegisterCallback<FocusInEvent>(TransitionTimeFieldFocusIn);
        transitionTimeField.RegisterCallback<FocusOutEvent>(TransitionTimeFieldFocusOut);
        root.Add(transitionTimeField);

        // 动画相关的信息
        int clipFrameCount = (int)(trackItem.AnimationEvent.AnimationClip.length * trackItem.AnimationEvent.AnimationClip.frameRate);
        clipFrameLabel = new Label("动画资源长度:" + clipFrameCount);
        root.Add(clipFrameLabel);
        isLoopLable = new Label("循环动画:" + trackItem.AnimationEvent.AnimationClip.isLooping);
        root.Add(isLoopLable);

        // 删除
        Button deleteButton = new Button(DeleteAnimationTrackItemButtonClick);
        deleteButton.text = "删除";
        deleteButton.style.backgroundColor = new Color(1, 0, 0, 0.5f);
        root.Add(deleteButton);

        // 设置持续帧数至选中帧
        Button setFrameButton = new Button(SetAnimationDurationFrameButton);
        setFrameButton.text = "设置持续帧数至选中帧";
        root.Add(setFrameButton);
    }

    private void AnimationClipAssetFiedlValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        AnimationClip clip = evt.newValue as AnimationClip;
        // 修改自身显示效果
        clipFrameLabel.text = "动画资源长度:" + ((int)(clip.length * clip.frameRate));
        isLoopLable.text = "循环动画:" + clip.isLooping;
        // 保存到配置
        trackItem.AnimationEvent.AnimationClip = clip;
        trackItem.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }

    private void RootMotionToggleValueChanged(ChangeEvent<bool> evt)
    {
        trackItem.AnimationEvent.ApplyRootMotion = evt.newValue;
        SkillEditorWindow.Instance.TickSkill();
    }

    private void MainWeaponOnLeftHandValueChanged(ChangeEvent<bool> evt)
    {
        trackItem.AnimationEvent.MainWeaponOnLeftHand = evt.newValue;
        SkillEditorWindow.Instance.TickSkill();
    }

    int oldDurationValue;
    private void DurationFieldFocusIn(FocusInEvent evt)
    {
        oldDurationValue = durationField.value;
    }
    private void DurationFieldFocusOut(FocusOutEvent evt)
    {
        if (durationField.value != oldDurationValue)
        {
            // 安全校验
            if (track.CheckFrameIndexOnDrag(itemFrameIndex + durationField.value, itemFrameIndex, false))
            {
                // 修改数据，刷新视图
                trackItem.AnimationEvent.DurationFrame = durationField.value;
                trackItem.CheckFrameCount();
                SkillEditorWindow.Instance.SaveConfig();
                trackItem.ResetView();
            }
            else
            {
                durationField.value = oldDurationValue;
            }
            SkillEditorWindow.Instance.TickSkill();
        }
    }
    private void SetAnimationDurationFrameButton()
    {
        DurationFieldFocusIn(null);
        durationField.value = SkillEditorWindow.Instance.CurrentSelectFrameIndex - trackItem.FrameIndex;
        DurationFieldFocusOut(null);
    }

    float oldTransitionTimeValue;
    private void TransitionTimeFieldFocusIn(FocusInEvent evt)
    {
        oldTransitionTimeValue = transitionTimeField.value;
    }
    private void TransitionTimeFieldFocusOut(FocusOutEvent evt)
    {
        if (transitionTimeField.value != oldTransitionTimeValue)
        {
            trackItem.AnimationEvent.TransitionTime = transitionTimeField.value;
        }
    }

    private void DeleteAnimationTrackItemButtonClick()
    {
        track.DeleteTrackItem(itemFrameIndex); // 此函数提供保存和刷新视图逻辑
        Selection.activeObject = null;
        SkillEditorWindow.Instance.TickSkill();
    }
}

