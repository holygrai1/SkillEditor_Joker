using System.Collections.Generic;
using UnityEngine.UIElements;

public class AttackDetectionTrack : SkillTrackBase
{
    private SkillMultilineTrackStyle trackStyle;
    public SkillAttackDetectionData SkillAttackDetectionData { get => SkillEditorWindow.Instance.SkillConfig.SkillAttackDetectionData; }
    private List<AttackDetectionTrackItem> trackItemList = new List<AttackDetectionTrackItem>();
    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWdith)
    {
        base.Init(menuParent, trackParent, frameWdith);
        trackStyle = new SkillMultilineTrackStyle();
        trackStyle.Init(menuParent, trackParent, "攻击伤害监测", AddChildTrack, CheckDeleteChildTrack, SwapChildTrack, UpdateChildTrackName);
        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);

        // 销毁已有的
        foreach (AttackDetectionTrackItem item in trackItemList)
        {
            item.Destroy();
        }
        trackItemList.Clear();
        if (SkillEditorWindow.Instance.SkillConfig == null) return;

        // 根据数据绘制TrackItem
        foreach (SkillAttackDetectionEvent item in SkillAttackDetectionData.FrameData)
        {
            CreateItem(item);
        }
    }

    private void CreateItem(SkillAttackDetectionEvent skillAttackDetectionEvent)
    {
        AttackDetectionTrackItem item = new AttackDetectionTrackItem();
        item.Init(this, frameWidth, skillAttackDetectionEvent, trackStyle.AddChildTrack());
        item.SetTrackName(skillAttackDetectionEvent.TrackName);
        trackItemList.Add(item);
    }

    private void UpdateChildTrackName(SkillMultilineTrackStyle.ChildTrack childTrack, string newName)
    {
        // 同步给配置
        SkillAttackDetectionData.FrameData[childTrack.GetIndex()].TrackName = newName;
        SkillEditorWindow.Instance.SaveConfig();
    }

    private void AddChildTrack()
    {
        SkillAttackDetectionEvent skillAttackDetectionEvent = new SkillAttackDetectionEvent();
        SkillAttackDetectionData.FrameData.Add(skillAttackDetectionEvent);
        CreateItem(skillAttackDetectionEvent);
        SkillEditorWindow.Instance.SaveConfig();
    }

    private bool CheckDeleteChildTrack(int index)
    {
        if (index < 0 || index >= SkillAttackDetectionData.FrameData.Count)
        {
            return false;
        }
        SkillAttackDetectionEvent skillAttackDetectionEvent = SkillAttackDetectionData.FrameData[index];
        if (skillAttackDetectionEvent != null)
        {
            SkillAttackDetectionData.FrameData.RemoveAt(index);
            trackItemList.RemoveAt(index);
            SkillEditorWindow.Instance.SaveConfig();
        }
        return skillAttackDetectionEvent != null;
    }

    private void SwapChildTrack(int index1, int index2)
    {
        SkillAttackDetectionEvent data1 = SkillAttackDetectionData.FrameData[index1];
        SkillAttackDetectionEvent data2 = SkillAttackDetectionData.FrameData[index2];
        SkillAttackDetectionData.FrameData[index1] = data2;
        SkillAttackDetectionData.FrameData[index2] = data1;
        // 保存交给窗口的退出机制
    }

    public override void Destory()
    {
        trackStyle.Destory();
    }

    public override void DrawGizmos()
    {
        for (int i = 0; i < trackItemList.Count; i++)
        {
            int currFrameIndex = SkillEditorWindow.Instance.CurrentSelectFrameIndex;
            SkillAttackDetectionEvent detectionEvent = trackItemList[i].SkillAttackDetectionEvent;
            if (currFrameIndex < detectionEvent.FrameIndex || currFrameIndex > detectionEvent.FrameIndex + detectionEvent.DurationFrame)
            {
                continue;
            }
            trackItemList[i].DrawGizmos();
        }
    }

    public override void OnSceneGUI()
    {
        for (int i = 0; i < trackItemList.Count; i++)
        {
            int currFrameIndex = SkillEditorWindow.Instance.CurrentSelectFrameIndex;
            SkillAttackDetectionEvent detectionEvent = trackItemList[i].SkillAttackDetectionEvent;
            if (currFrameIndex < detectionEvent.FrameIndex || currFrameIndex > detectionEvent.FrameIndex + detectionEvent.DurationFrame)
            {
                continue;
            }
            if (SkillEditorInspector.currentTrackItem != trackItemList[i]) // 必须选中才绘制
            {
                continue;
            }
            trackItemList[i].OnSceneGUI();
        }
    }
}