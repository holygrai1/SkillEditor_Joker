using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectTrackItem : TrackItemBase<EffectTrack>
{
    private SkillMultilineTrackStyle.ChildTrack childTrackStyle;
    private SkillEffectTrackItemStyle trackItemStyle;

    private SkillEffectEvent skillEffectEvent;
    public SkillEffectEvent SkillEffectEvent { get => skillEffectEvent; }
    public void Init(EffectTrack track, float frameUnitWidth, SkillEffectEvent skillEffectEvent, SkillMultilineTrackStyle.ChildTrack childTrack)
    {
        this.track = track;
        this.frameIndex = skillEffectEvent.FrameIndex;
        this.childTrackStyle = childTrack;
        this.skillEffectEvent = skillEffectEvent;
        normalColor = new Color(0.388f, 0.850f, 0.905f, 0.5f);
        selectColor = new Color(0.388f, 0.850f, 0.905f, 1f);
        trackItemStyle = new SkillEffectTrackItemStyle();
        itemStyle = trackItemStyle;

        childTrackStyle.trackRoot.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        childTrackStyle.trackRoot.RegisterCallback<DragExitedEvent>(DragExited);
        ResetView(frameUnitWidth);
    }

    public override void ResetView(float frameUnitWidth)
    {
        base.ResetView(frameUnitWidth);
        if (skillEffectEvent.Prefab != null)
        {
            if (!trackItemStyle.isInit)
            {
                trackItemStyle.Init(frameUnitWidth, skillEffectEvent, childTrackStyle);
                // 绑定事件
                trackItemStyle.mainDragArea.RegisterCallback<MouseDownEvent>(MouseDown);
                trackItemStyle.mainDragArea.RegisterCallback<MouseUpEvent>(MouseUp);
                trackItemStyle.mainDragArea.RegisterCallback<MouseOutEvent>(MouseOut);
                trackItemStyle.mainDragArea.RegisterCallback<MouseMoveEvent>(MouseMove);
            }
        }
        trackItemStyle.ResetView(frameUnitWidth, skillEffectEvent);

        // 强行重新生成预览
        CleanEffectPreviewObj();
        TickView(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
    }

    public void Destroy()
    {
        CleanEffectPreviewObj();
        childTrackStyle.Destroy();
    }

    public void CleanEffectPreviewObj()
    {
        if (effectPreviewObj != null)
        {
            GameObject.DestroyImmediate(effectPreviewObj);
            effectPreviewObj = null;
        }
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
            skillEffectEvent.FrameIndex = frameIndex;
            // 如果超过右侧边界，拓展边界
            // CheckFrameCount();
            // 刷新视图
            ResetView(frameUnitWidth);
        }
    }

    private void ApplyDrag()
    {
        if (startDragFrameIndex != frameIndex)
        {
            skillEffectEvent.FrameIndex = frameIndex;
            SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        }
    }
    #endregion

    #region 拖拽资源
    private void OnDragUpdate(DragUpdatedEvent evt)
    {
        // 监听用户拖拽的是否是动画
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        GameObject prefab = objs[0] as GameObject;
        if (prefab != null)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }
    }
    private void DragExited(DragExitedEvent evt)
    {
        // 监听用户拖拽的是否是动画
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        GameObject prefab = objs[0] as GameObject;
        if (prefab != null)
        {
            int selectFrameIndex = SkillEditorWindow.Instance.GetFrameIndexByPos(evt.localMousePosition.x);
            if (selectFrameIndex >= 0)
            {
                // 构建默认的特效数据
                skillEffectEvent.FrameIndex = selectFrameIndex;
                skillEffectEvent.Prefab = prefab;
                skillEffectEvent.Position = Vector3.zero;
                skillEffectEvent.Rotation = Vector3.zero;
                skillEffectEvent.Scale = Vector3.one;
                skillEffectEvent.AutoDestruct = true;

                ParticleSystem[] particleSystems = prefab.GetComponentsInChildren<ParticleSystem>();
                float max = -1;
                int curr = -1;
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    if (particleSystems[i].main.duration > max)
                    {
                        max = particleSystems[i].main.duration;
                        curr = i;
                    }
                }
                skillEffectEvent.Duration = (int)(particleSystems[curr].main.duration * SkillEditorWindow.Instance.SkillConfig.FrameRote);

                this.frameIndex = selectFrameIndex;
                ResetView();
                SkillEditorWindow.Instance.SaveConfig();
            }
        }
    }
    #endregion


    #region 预览
    private GameObject effectPreviewObj;
    public void TickView(int frameIndex)
    {
        if (skillEffectEvent.Prefab == null || SkillEditorWindow.Instance.PreviewCharacterObj == null) return;
        // 是不是在播放范围内
        int durationFrame = skillEffectEvent.Duration;

        if (skillEffectEvent.FrameIndex <= frameIndex && skillEffectEvent.FrameIndex + durationFrame > frameIndex)
        {
            GameObject.DestroyImmediate(effectPreviewObj);
            Transform characterRoot = SkillEditorWindow.Instance.PreviewCharacterObj.transform;
            Vector3 pos = characterRoot.TransformPoint(SkillEffectEvent.Position);
            Vector3 rot = characterRoot.eulerAngles + SkillEffectEvent.Rotation;
            // 实例化
            effectPreviewObj = GameObject.Instantiate(SkillEffectEvent.Prefab, pos, Quaternion.Euler(rot), EffectTrack.EffectParent);
            effectPreviewObj.name = skillEffectEvent.Prefab.name;
            effectPreviewObj.transform.localScale = SkillEffectEvent.Scale;
            // 粒子模拟
            ParticleSystem[] particleSystems = effectPreviewObj.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particleSystems.Length; i++)
            {
                int simulateFrame = frameIndex - skillEffectEvent.FrameIndex;
                particleSystems[i].Simulate((float)simulateFrame / SkillEditorWindow.Instance.SkillConfig.FrameRote);
            }
        }
        else
        {
            CleanEffectPreviewObj();
        }
    }

    public void ApplyModelTransformData()
    {
        if (effectPreviewObj != null)
        {
            Transform characterRoot = SkillEditorWindow.Instance.PreviewCharacterObj.transform;
            // 获取模拟坐标
            Vector3 rootPostion = SkillEditorWindow.Instance.GetPostionForRootMotion(skillEffectEvent.FrameIndex, true);
            Vector3 oldPos = characterRoot.position;

            // 把角色临时设置到播放坐标
            characterRoot.position = rootPostion;
            skillEffectEvent.Position = characterRoot.InverseTransformPoint(effectPreviewObj.transform.position);
            skillEffectEvent.Rotation = effectPreviewObj.transform.eulerAngles - characterRoot.eulerAngles;
            skillEffectEvent.Scale = effectPreviewObj.transform.localScale;
            characterRoot.position = oldPos;
        }
    }
    #endregion



}
