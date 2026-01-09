using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class EventTrack : SkillTrackBase
{
    private SkillSingleLineTrackStyle trackStyle;
    private Dictionary<int, EventTrackItem> trackItemDic = new Dictionary<int, EventTrackItem>();
    public SkillCustomEventData CustomEventData { get => SkillEditorWindow.Instance.SkillConfig.skillCustomEventData; }
    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWdith)
    {
        base.Init(menuParent, trackParent, frameWdith);
        trackStyle = new SkillSingleLineTrackStyle();
        trackStyle.Init(menuParent, trackParent, "事件配置");
        trackStyle.contentRoot.RegisterCallback<MouseDownEvent>(ContentRootMouseDown);
        ResetView();
    }

    private void ContentRootMouseDown(MouseDownEvent evt)
    {
        int frameIndex = SkillEditorWindow.Instance.GetFrameIndexByMousePos(evt.localMousePosition.x);
        if (CustomEventData.FrameData.ContainsKey(frameIndex)) return;
        if (EventTrackItem.currentSelectItem != null) // 变化位置
        {
            EventTrackItem.currentSelectItem.ChangeFrameIndex(frameIndex);
        }
        else // 添加轨道
        {
            SkillCustomEvent skillCustomEvent = new SkillCustomEvent()
            {
            };
            CustomEventData.FrameData.Add(frameIndex, skillCustomEvent);
            SkillEditorWindow.Instance.SaveConfig();
            CreateItem(frameIndex, skillCustomEvent);
        }
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);
        // 销毁当前已有
        foreach (var item in trackItemDic)
        {
            trackStyle.DeleteItem(item.Value.itemStyle.root);
        }
        trackItemDic.Clear();
        if (SkillEditorWindow.Instance.SkillConfig == null) return;

        // 根据数据绘制TrackItem
        foreach (var item in CustomEventData.FrameData)
        {
            CreateItem(item.Key, item.Value);
        }
    }

    private void CreateItem(int frameIndex, SkillCustomEvent skillCustomEvent)
    {
        EventTrackItem trackItem = new EventTrackItem();
        trackItem.Init(this, trackStyle, frameIndex, frameWidth, skillCustomEvent);
        trackItemDic.Add(frameIndex, trackItem);
    }


    /// <summary>
    /// 将oldIndex的数据变为newIndex
    /// </summary>
    public void SetFrameIndex(int oldIndex, int newIndex)
    {
        if (CustomEventData.FrameData.Remove(oldIndex, out SkillCustomEvent customEvent))
        {
            CustomEventData.FrameData.Add(newIndex, customEvent);
            trackItemDic.Remove(oldIndex, out EventTrackItem eventTrackItem);
            trackItemDic.Add(newIndex, eventTrackItem);
        }
    }

    public override void DeleteTrackItem(int frameIndex)
    {
        CustomEventData.FrameData.Remove(frameIndex);
        if (trackItemDic.Remove(frameIndex, out EventTrackItem item))
        {
            trackStyle.DeleteItem(item.itemStyle.root);
        }
    }


    public override void Destory()
    {
        trackStyle.Destory();
    }
}
