using System.Collections.Generic;
using UnityEngine.UIElements;

public class AudioTrack : SkillTrackBase
{
    private SkillMultilineTrackStyle trackStyle;
    public SkillAudioData AudioData { get => SkillEditorWindow.Instance.SkillConfig.SkillAudioData; }
    private List<AudioTrackItem> trackItemList = new List<AudioTrackItem>();
    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWdith)
    {
        base.Init(menuParent, trackParent, frameWdith);
        trackStyle = new SkillMultilineTrackStyle();
        trackStyle.Init(menuParent, trackParent, "音效配置", AddChildTrack, CheckDeleteChildTrack, SwapChildTrack, UpdateChildTrackName);
        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);

        // 销毁已有的
        foreach (AudioTrackItem item in trackItemList)
        {
            item.Destroy();
        }
        trackItemList.Clear();
        if (SkillEditorWindow.Instance.SkillConfig == null) return;

        // 根据数据绘制TrackItem
        foreach (SkillAudioEvent item in AudioData.FrameData)
        {
            CreateItem(item);
        }
    }

    private void CreateItem(SkillAudioEvent audioEvent)
    {
        AudioTrackItem item = new AudioTrackItem();
        item.Init(this, frameWidth, audioEvent, trackStyle.AddChildTrack());
        item.SetTrackName(audioEvent.TrackName);
        trackItemList.Add(item);
    }

    private void UpdateChildTrackName(SkillMultilineTrackStyle.ChildTrack childTrack, string newName)
    {
        // 同步给配置
        AudioData.FrameData[childTrack.GetIndex()].TrackName = newName;
        SkillEditorWindow.Instance.SaveConfig();
    }

    private void AddChildTrack()
    {
        SkillAudioEvent skillAudioEvent = new SkillAudioEvent();
        AudioData.FrameData.Add(skillAudioEvent);
        CreateItem(skillAudioEvent);
        SkillEditorWindow.Instance.SaveConfig();
    }

    private bool CheckDeleteChildTrack(int index)
    {
        if (index < 0 || index >= AudioData.FrameData.Count)
        {
            return false;
        }
        SkillAudioEvent skillAudioEvent = AudioData.FrameData[index];
        if (skillAudioEvent != null)
        {
            AudioData.FrameData.RemoveAt(index);
            trackItemList.RemoveAt(index);
            SkillEditorWindow.Instance.SaveConfig();
        }
        return skillAudioEvent != null;
    }

    private void SwapChildTrack(int index1, int index2)
    {
        SkillAudioEvent data1 = AudioData.FrameData[index1];
        SkillAudioEvent data2 = AudioData.FrameData[index2];
        AudioData.FrameData[index1] = data2;
        AudioData.FrameData[index2] = data1;
        // 保存交给窗口的退出机制
    }

    public override void Destory()
    {
        trackStyle.Destory();
    }

    public override void OnPlay(int startFrameIndex)
    {
        for (int i = 0; i < AudioData.FrameData.Count; i++)
        {
            SkillAudioEvent audioEvent = AudioData.FrameData[i];
            if (audioEvent.AudioClip == null) continue;

            int audioFrameCount = (int)(audioEvent.AudioClip.length * SkillEditorWindow.Instance.SkillConfig.FrameRote);
            int audioLastFrameIndex = audioFrameCount + audioEvent.FrameIndex;
            // 意味着开始位置在左边 && 并且长度大于当前选中帧
            // 也就是时间轴播放帧 在 轨道的中间部分
            if (audioEvent.FrameIndex < startFrameIndex
                && audioLastFrameIndex > startFrameIndex)
            {
                int offset = startFrameIndex - audioEvent.FrameIndex;
                float playRate = (float)offset / audioFrameCount;
                EditorAudioUnility.PlayAudio(audioEvent.AudioClip, playRate);
            }
            else if (audioEvent.FrameIndex == startFrameIndex)
            {
                // 播放音效，从头播放
                EditorAudioUnility.PlayAudio(audioEvent.AudioClip, 0);
            }
        }
    }

    public override void TickView(int frameIndex)
    {
        if (SkillEditorWindow.Instance.IsPlaying)
        {
            for (int i = 0; i < AudioData.FrameData.Count; i++)
            {
                SkillAudioEvent audioEvent = AudioData.FrameData[i];
                if (audioEvent.AudioClip != null && audioEvent.FrameIndex == frameIndex)
                {
                    // 播放音效，从头播放
                    EditorAudioUnility.PlayAudio(audioEvent.AudioClip, 0);
                }
            }
        }
    }

}
