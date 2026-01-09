using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackDetectionTrackItem : TrackItemBase<AttackDetectionTrack>
{
    private SkillMultilineTrackStyle.ChildTrack childTrackStyle;
    private SkillAttackDetectionTrackItemStyle trackItemStyle;

    private SkillAttackDetectionEvent skillAttackDetectionEvent;
    public SkillAttackDetectionEvent SkillAttackDetectionEvent { get => skillAttackDetectionEvent; }

    public void Init(AttackDetectionTrack track, float frameUnitWidth, SkillAttackDetectionEvent skillAttackDetectionEvent, SkillMultilineTrackStyle.ChildTrack childTrack)
    {
        this.track = track;
        this.frameIndex = skillAttackDetectionEvent.FrameIndex;
        this.childTrackStyle = childTrack;
        this.skillAttackDetectionEvent = skillAttackDetectionEvent;
        if (skillAttackDetectionEvent.AttackHitConfig == null) skillAttackDetectionEvent.AttackHitConfig = new AttackHitConfig();
        normalColor = new Color(0.388f, 0.850f, 0.905f, 0.5f);
        selectColor = new Color(0.388f, 0.850f, 0.905f, 1f);
        trackItemStyle = new SkillAttackDetectionTrackItemStyle();
        itemStyle = trackItemStyle;
        ResetView(frameUnitWidth);
    }

    public override void ResetView(float frameUnitWidth)
    {
        base.ResetView(frameUnitWidth);
        if (!trackItemStyle.isInit)
        {
            trackItemStyle.Init(frameUnitWidth, skillAttackDetectionEvent, childTrackStyle);
            // 绑定事件
            trackItemStyle.mainDragArea.RegisterCallback<MouseDownEvent>(MouseDown);
            trackItemStyle.mainDragArea.RegisterCallback<MouseUpEvent>(MouseUp);
            trackItemStyle.mainDragArea.RegisterCallback<MouseOutEvent>(MouseOut);
            trackItemStyle.mainDragArea.RegisterCallback<MouseMoveEvent>(MouseMove);
        }
        trackItemStyle.ResetView(frameUnitWidth, skillAttackDetectionEvent);
    }

    public void Destroy()
    {
        childTrackStyle.Destroy();
    }

    public void SetTrackName(string name)
    {
        childTrackStyle.SetTrackName(name);
    }

    #region 鼠标交互
    private bool mouseDrag = false;
    private float startDargPosX;
    private int startDragFrameIndex;
    private void MouseDown(MouseDownEvent evt)
    {
        startDargPosX = evt.mousePosition.x;
        startDragFrameIndex = frameIndex;
        mouseDrag = true;
        Select();
    }
    private void MouseUp(MouseUpEvent evt)
    {
        if (mouseDrag) ApplyDrag();
        mouseDrag = false;
    }

    private void MouseOut(MouseOutEvent evt)
    {
        if (mouseDrag) ApplyDrag();
        mouseDrag = false;
    }

    private void MouseMove(MouseMoveEvent evt)
    {
        if (mouseDrag)
        {
            float offsetPos = evt.mousePosition.x - startDargPosX;
            int offsetFrame = Mathf.RoundToInt(offsetPos / frameUnitWidth);
            int targetFrameIndex = startDragFrameIndex + offsetFrame;

            // 不考虑拖拽到负数的情况 以及 没有便宜
            if (targetFrameIndex < 0 || offsetFrame == 0) return;

            // 确定修改的数据
            frameIndex = targetFrameIndex;
            skillAttackDetectionEvent.FrameIndex = frameIndex;
            // 如果超过右侧边界，拓展边界
            // CheckFrameCount();
            // 刷新视图
            ResetView(frameUnitWidth);
        }
    }

    public void CheckFrameCount()
    {
        int frameCount = (int)(skillAttackDetectionEvent.DurationFrame * SkillEditorWindow.Instance.SkillConfig.FrameRote);
        // 如果超过右侧边界，拓展边界
        if (frameIndex + frameCount > SkillEditorWindow.Instance.SkillConfig.FrameCount)
        {
            // 保存配置导致对象无效，重新引用
            SkillEditorWindow.Instance.CurrentFrameCount = frameIndex + frameCount;
        }
    }

    private void ApplyDrag()
    {
        if (startDragFrameIndex != frameIndex)
        {
            skillAttackDetectionEvent.FrameIndex = frameIndex;
            SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        }
    }
    #endregion

    public void DrawGizmos()
    {
        SkillGizmosTool.DrawDetection(skillAttackDetectionEvent, SkillEditorWindow.Instance.PreviewCharacterObj.GetComponent<Skill_Player>());
    }

    public void OnSceneGUI()
    {
        Transform previewCharacterObj = SkillEditorWindow.Instance.PreviewCharacterObj.transform;
        switch (skillAttackDetectionEvent.AttackDetectionType)
        {
            case AttackDetectionType.Box:
                AttackBoxDetectionData boxDetectionData = (AttackBoxDetectionData)skillAttackDetectionEvent.AttackDetectionData;
                Quaternion boxQuaternion = previewCharacterObj.rotation * Quaternion.Euler(boxDetectionData.Rotation);
                Vector3 boxPos = previewCharacterObj.TransformPoint(boxDetectionData.Position);
                EditorGUI.BeginChangeCheck();
                Handles.TransformHandle(ref boxPos, ref boxQuaternion, ref boxDetectionData.Scale);
                if (EditorGUI.EndChangeCheck()) // 如果发生了修改
                {
                    boxDetectionData.Position = previewCharacterObj.InverseTransformPoint(boxPos);
                    boxDetectionData.Rotation = (Quaternion.Inverse(previewCharacterObj.rotation) * boxQuaternion).eulerAngles;
                    SkillEditorInspector.SetTrackItem(this, track);
                }
                break;
            case AttackDetectionType.Sphere:
                AttackSphereDetectionData sphereDetectionData = (AttackSphereDetectionData)skillAttackDetectionEvent.AttackDetectionData;
                Vector3 oldPosition = previewCharacterObj.TransformPoint(sphereDetectionData.Position);
                Vector3 newPosition = Handles.PositionHandle(oldPosition, Quaternion.identity);
                float newRadius = Handles.ScaleSlider(sphereDetectionData.Radius, newPosition, Vector3.up, Quaternion.identity, sphereDetectionData.Radius + 0.5f, 0.1f);
                if (oldPosition != newPosition || sphereDetectionData.Radius != newRadius)
                {
                    sphereDetectionData.Position = previewCharacterObj.InverseTransformPoint(newPosition);
                    sphereDetectionData.Radius = newRadius;
                    SkillEditorInspector.SetTrackItem(this, track);
                }
                break;
            case AttackDetectionType.Fan:
                AttackFanDetectionData fanDetectionData = (AttackFanDetectionData)skillAttackDetectionEvent.AttackDetectionData;
                Quaternion fanQuaternion = previewCharacterObj.rotation * Quaternion.Euler(fanDetectionData.Rotation);
                Vector3 fanPos = previewCharacterObj.TransformPoint(fanDetectionData.Position);
                // X:角度 Y:高度 Z:外圈半径
                Vector3 fanScale = new Vector3(fanDetectionData.Angle, fanDetectionData.Height, fanDetectionData.Radius);
                EditorGUI.BeginChangeCheck();
                Handles.TransformHandle(ref fanPos, ref fanQuaternion, ref fanScale);
                float insideRadiusHandle = Handles.ScaleSlider(fanDetectionData.InsideRadius, fanPos, -previewCharacterObj.forward, Quaternion.identity, 1.5f, 0.1f);
                if (EditorGUI.EndChangeCheck()) // 如果发生了修改
                {
                    fanDetectionData.Position = previewCharacterObj.InverseTransformPoint(fanPos);
                    fanDetectionData.Rotation = (Quaternion.Inverse(previewCharacterObj.rotation) * fanQuaternion).eulerAngles;
                    fanDetectionData.Angle = fanScale.x;
                    fanDetectionData.Height = fanScale.y;
                    fanDetectionData.Radius = fanScale.z;
                    fanDetectionData.InsideRadius = insideRadiusHandle;
                    SkillEditorInspector.SetTrackItem(this, track);
                }
                break;
        }
    }
}