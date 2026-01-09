using UnityEngine;

public abstract class TrackItemBase
{
    protected int frameIndex;
    public int FrameIndex { get => frameIndex; }
    protected float frameUnitWidth;
    public abstract void Select();
    public abstract void OnSelect();
    public abstract void OnUnSelect();
    public virtual void OnConfigChanged() { }
    public virtual void ResetView()
    {
        ResetView(frameUnitWidth);
    }
    public virtual void ResetView(float frameUnitWidth)
    {
        this.frameUnitWidth = frameUnitWidth;
    }
}
public abstract class TrackItemBase<T> : TrackItemBase where T : SkillTrackBase
{
    protected T track;
    protected Color normalColor;
    protected Color selectColor;
    public SkillTrackItemStyleBase itemStyle { get; protected set; }

    public override void Select()
    {
        SkillEditorWindow.Instance.ShowTrackItemOnInspecotr(this, track);
    }
    public override void OnSelect()
    {
        itemStyle.SetBGColor(selectColor);
    }
    public override void OnUnSelect()
    {
        itemStyle.SetBGColor(normalColor);
    }
}
