using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class EffectTrack : SkillTrackBase
{
    private SkillMultilineTrackStyle trackStyle;
    public SkillEffectData EffectData { get => SkillEditorWindow.Instance.SkillConfig.SkillEffectData; }
    private List<EffectTrackItem> trackItemList = new List<EffectTrackItem>();

    public static Transform EffectParent { get; private set; }

    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWdith)
    {
        base.Init(menuParent, trackParent, frameWdith);
        trackStyle = new SkillMultilineTrackStyle();
        trackStyle.Init(menuParent, trackParent, "特效配置", AddChildTrack, CheckDeleteChildTrack, SwapChildTrack, UpdateChildTrackName);

        if (SkillEditorWindow.Instance.OnEditorScene)
        {
            EffectParent = GameObject.Find("Effects").transform;
            EffectParent.position = Vector3.zero;
            EffectParent.rotation = Quaternion.identity;
            for (int i = EffectParent.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(EffectParent.GetChild(i).gameObject);
            }
        }

        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);

        // 销毁已有的
        foreach (EffectTrackItem item in trackItemList)
        {
            item.Destroy();
        }
        trackItemList.Clear();

        // 根据数据绘制TrackItem
        foreach (SkillEffectEvent item in EffectData.FrameData)
        {
            CreateItem(item);
        }
    }

    private void CreateItem(SkillEffectEvent effectEvent)
    {
        EffectTrackItem item = new EffectTrackItem();
        item.Init(this, frameWidth, effectEvent, trackStyle.AddChildTrack());
        item.SetTrackName(effectEvent.TrackName);
        trackItemList.Add(item);
    }

    private void UpdateChildTrackName(SkillMultilineTrackStyle.ChildTrack childTrack, string newName)
    {
        // 同步给配置
        EffectData.FrameData[childTrack.GetIndex()].TrackName = newName;
        SkillEditorWindow.Instance.SaveConfig();
    }

    private void AddChildTrack()
    {
        SkillEffectEvent skillEffectEvent = new SkillEffectEvent();
        EffectData.FrameData.Add(skillEffectEvent);
        CreateItem(skillEffectEvent);
        SkillEditorWindow.Instance.SaveConfig();
    }

    private bool CheckDeleteChildTrack(int index)
    {
        if (index < 0 || index >= EffectData.FrameData.Count)
        {
            return false;
        }
        SkillEffectEvent skillEffectEvent = EffectData.FrameData[index];
        if (skillEffectEvent != null)
        {
            EffectData.FrameData.RemoveAt(index);
            SkillEditorWindow.Instance.SaveConfig();
            trackItemList[index].CleanEffectPreviewObj();
            trackItemList.RemoveAt(index);
        }
        return skillEffectEvent != null;
    }

    private void SwapChildTrack(int index1, int index2)
    {
        SkillEffectEvent data1 = EffectData.FrameData[index1];
        SkillEffectEvent data2 = EffectData.FrameData[index2];
        EffectData.FrameData[index1] = data2;
        EffectData.FrameData[index2] = data1;
        // 保存交给窗口的退出机制
    }

    public override void Destory()
    {
        trackStyle.Destory();
        for (int i = 0; i < trackItemList.Count; i++)
        {
            trackItemList[i].CleanEffectPreviewObj();
        }
    }

    public override void TickView(int frameIndex)
    {
        for (int i = 0; i < trackItemList.Count; i++)
        {
            trackItemList[i].TickView(frameIndex);
        }
    }
}
