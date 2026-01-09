using UnityEditor;
using UnityEngine.UIElements;

public class SkillAttackDetectionTrackItemStyle : SkillTrackItemStyleBase
{
    private const string trackItemAssetPath = "Assets/SkillEditor/Editor/Track/Assets/TrackItem/AudioTrackItem.uxml";
    private Label titleLabel;
    public VisualElement mainDragArea { get; private set; }
    public bool isInit { get; private set; }
    public void Init(float frameUnitWidth, SkillAttackDetectionEvent skillAttackDetectionEvent, SkillMultilineTrackStyle.ChildTrack childTrack)
    {
        if (!isInit)
        {
            titleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackItemAssetPath).Instantiate().Query<Label>();
            root = titleLabel;
            mainDragArea = root.Q<VisualElement>("Main");
            childTrack.InitContent(root);
            isInit = true;
        }
    }

    public void ResetView(float frameUnitWidth, SkillAttackDetectionEvent skillAttackDetectionEvent)
    {
        if (!isInit) return;
        SetTitle("");
        SetWidth(frameUnitWidth * skillAttackDetectionEvent.DurationFrame);
        SetPosition(frameUnitWidth * skillAttackDetectionEvent.FrameIndex);
    }

    public void SetTitle(string title)
    {
        titleLabel.text = title;
    }
}