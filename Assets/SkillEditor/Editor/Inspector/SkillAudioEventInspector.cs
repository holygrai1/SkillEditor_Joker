using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class SkillAudioEventInspector : SkillEventDataInspectorBase<AudioTrackItem, AudioTrack>
{
    private FloatField voluemFiled;

    public override void OnDraw()
    {
        // 动画资源
        ObjectField audioClipAssetField = new ObjectField("音效资源");
        audioClipAssetField.objectType = typeof(AudioClip);
        audioClipAssetField.value = trackItem.SkillAudioEvent.AudioClip;
        audioClipAssetField.RegisterValueChangedCallback(AudioClipAssetFiedlValueChanged);
        root.Add(audioClipAssetField);

        // 音量
        voluemFiled = new FloatField("播放音量");
        voluemFiled.value = trackItem.SkillAudioEvent.Voluem;
        voluemFiled.RegisterCallback<FocusInEvent>(VoluemFiledFocusIn);
        voluemFiled.RegisterCallback<FocusOutEvent>(VoluemFiledFocusOut);
        root.Add(voluemFiled);
    }
    private void AudioClipAssetFiedlValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        AudioClip audioClip = evt.newValue as AudioClip;
        // 保存到配置中
        trackItem.SkillAudioEvent.AudioClip = audioClip;
        trackItem.ResetView();
    }

    float oldVoluemFiledValue;
    private void VoluemFiledFocusIn(FocusInEvent evt)
    {
        oldVoluemFiledValue = voluemFiled.value;
    }
    private void VoluemFiledFocusOut(FocusOutEvent evt)
    {
        if (voluemFiled.value != oldVoluemFiledValue)
        {
            trackItem.SkillAudioEvent.Voluem = voluemFiled.value;
        }
    }
}
