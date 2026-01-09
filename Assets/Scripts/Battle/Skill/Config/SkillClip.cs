using JKFrame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Skill/SkillClip", fileName = "SkillClip")]
public class SkillClip : ConfigBase
{
    [LabelText("技能名称")] public string SkillName;
    [LabelText("帧数上限")] public int FrameCount = 100;
    [LabelText("帧率")] public int FrameRote = 30;

    [NonSerialized, OdinSerialize] public SkillCustomEventData skillCustomEventData = new SkillCustomEventData();
    [NonSerialized, OdinSerialize] public SkillAnimationData SkillAnimationData = new SkillAnimationData();
    [NonSerialized, OdinSerialize] public SkillAudioData SkillAudioData = new SkillAudioData();
    [NonSerialized, OdinSerialize] public SkillEffectData SkillEffectData = new SkillEffectData();
    [NonSerialized, OdinSerialize] public SkillAttackDetectionData SkillAttackDetectionData = new SkillAttackDetectionData();

#if UNITY_EDITOR
    private static Action skillConfigValidate;
    public static void SetValidateAction(Action action)
    {
        skillConfigValidate = action;
    }

    private void OnValidate()
    {
        skillConfigValidate?.Invoke();
    }
#endif
}
