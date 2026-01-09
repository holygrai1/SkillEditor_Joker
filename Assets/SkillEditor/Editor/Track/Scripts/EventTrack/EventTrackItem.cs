using System;
using UnityEngine;
using UnityEngine.UIElements;

public class EventTrackItem : TrackItemBase<EventTrack>
{
    private SkillCustomEvent customEvent;
    public SkillCustomEvent CustomEvent { get => customEvent; }

    private SkillCustomEventTrackItemStyle trackItemStyle;
    public static EventTrackItem currentSelectItem;
    public void Init(EventTrack eventTrack, SkillTrackStyleBase parentTrackStyle, int startFrameIndex, float frameUnitWidth, SkillCustomEvent customEvent)
    {
        this.frameUnitWidth = frameUnitWidth;
        this.frameIndex = startFrameIndex;
        track = eventTrack;
        this.customEvent = customEvent;

        trackItemStyle = new SkillCustomEventTrackItemStyle();
        itemStyle = trackItemStyle;
        trackItemStyle.Init(parentTrackStyle);

        normalColor = new Color(0.388f, 0.850f, 0.905f, 0.5f);
        selectColor = new Color(0.388f, 0.850f, 0.905f, 1f);
        OnUnSelect();
        trackItemStyle.root.RegisterCallback<MouseDownEvent>(MouseDown);
        ResetView(frameUnitWidth);
    }

    private void MouseDown(MouseDownEvent evt)
    {
        if (currentSelectItem == this) OnUnSelect();
        else Select();
    }

    public override void OnSelect()
    {
        currentSelectItem = this;
        base.OnSelect();
    }
    public override void OnUnSelect()
    {
        currentSelectItem = null;
        base.OnUnSelect();
    }

    public override void ResetView(float frameUnitWidth)
    {
        this.frameUnitWidth = frameUnitWidth;
        // 位置计算
        trackItemStyle.SetPosition(frameIndex * frameUnitWidth - frameUnitWidth / 2);
        trackItemStyle.SetWidth(frameUnitWidth);
    }

    public void ChangeFrameIndex(int newIndex)
    {
        track.SetFrameIndex(frameIndex, newIndex);
        frameIndex = newIndex;
        SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        ResetView();
    }

}
