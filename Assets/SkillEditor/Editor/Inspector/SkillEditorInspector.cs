using System;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(SkillEditorWindow))]
public class SkillEditorInspector : Editor
{
    public static SkillEditorInspector Instance;
    public static TrackItemBase currentTrackItem { get; private set; }
    private static SkillTrackBase currentTrack;
    public static void SetTrackItem(TrackItemBase trackItem, SkillTrackBase track)
    {
        if (currentTrackItem != null)
        {
            currentTrackItem.OnUnSelect();
        }
        currentTrackItem = trackItem;
        currentTrackItem.OnSelect();
        currentTrack = track;
        // 避免已经打开了Inspector，不刷新数据
        if (Instance != null) Instance.Show();

    }
    private void OnDestroy()
    {
        // 说明窗口卸载
        if (currentTrackItem != null)
        {
            currentTrackItem.OnUnSelect();
            currentTrackItem = null;
            currentTrack = null;
        }
    }
    private VisualElement root;
    public override VisualElement CreateInspectorGUI()
    {
        Instance = this;
        root = new VisualElement();
        Show();
        return root;
    }

    private SkillEventDataInspectorBase eventDataInspector;

    public void Show()
    {
        Clean();
        if (currentTrackItem == null) return;
        trackItemFrameIndex = currentTrackItem.FrameIndex;
        Type itemType = currentTrackItem.GetType();
        eventDataInspector = null;
        if (itemType == typeof(AnimationTrackItem))
        {
            eventDataInspector = new SkillAnimationEventInspector();
        }
        else if (itemType == typeof(AudioTrackItem))
        {
            eventDataInspector = new SkillAudioEventInspector();
        }
        else if (itemType == typeof(EffectTrackItem))
        {
            eventDataInspector = new SkillEffectEventInspector();
        }
        else if (itemType == typeof(AttackDetectionTrackItem))
        {
            eventDataInspector = new SkillAttackDetectionEventInspector();
        }
        else if (itemType == typeof(EventTrackItem))
        {
            eventDataInspector = new SkillCustomEventInspector();
        }
        eventDataInspector?.Draw(root, currentTrackItem, currentTrack);
    }

    private void Clean()
    {
        if (root != null)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                root.RemoveAt(i);
            }
        }
    }
    private int trackItemFrameIndex;
    public void SetTrackItemFrameIndex(int trackItemFrameIndex)
    {
        this.trackItemFrameIndex = trackItemFrameIndex;
        eventDataInspector.SetFrameIndex(trackItemFrameIndex);
    }
}
