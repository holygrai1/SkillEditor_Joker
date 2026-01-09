using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillEditorWindow : EditorWindow
{
    public static SkillEditorWindow Instance;
    [MenuItem("SkillEditor/SkillEditorWindow")]
    public static void ShowExample()
    {
        SkillEditorWindow wnd = GetWindow<SkillEditorWindow>();
        wnd.titleContent = new GUIContent("技能编辑器");
    }
    private VisualElement root;
    public void CreateGUI()
    {
        SkillClip.SetValidateAction(ResetView);
        Instance = this;
        root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SkillEditor/Editor/EditorWindow/SkillEditorWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
        InitTopMenu();
        InitTimerShaft();
        InitConsole();
        InitContent();
        if (skillConfig != null)
        {
            SkillConfigObjectField.value = skillConfig;
            CurrentFrameCount = skillConfig.FrameCount;
        }
        else
        {
            CurrentFrameCount = 100;
        }
        if (currentPreviewCharacterPrefab != null)
        {
            PreviewCharacterPrefabObjectField.value = currentPreviewCharacterPrefab;
        }
        if (currentPreviewCharacterObj != null)
        {
            PreviewCharacterObjectField.value = currentPreviewCharacterObj;
        }
        CurrentSelectFrameIndex = 0;
    }

    private void ResetView()
    {
        //ResetTrackData();
        //UpdateContentSize();
        //ResetTrack();
        SkillClip tempSkillConfig = skillConfig;
        SkillConfigObjectField.value = null;
        SkillConfigObjectField.value = tempSkillConfig;
    }

    // 窗口销毁会调用，但是直接关闭Unity不会调用
    // private void OnDestroy()

    private void OnEnable()
    {
        SceneView.beforeSceneGui += OnSceneGUI;
    }
    private void OnDisable()
    {
        if (skillConfig != null) SaveConfig();
        SceneView.beforeSceneGui -= OnSceneGUI;
    }

    #region TopMenu
    private const string skillEditorScenePath = "Assets/SkillEditor/SkillEditorScene.unity";
    private const string previewCharacterParentPath = "PreviewCharacterRoot";
    private string oldScenePath;

    private Button LoadEditorSceneButton;
    private Button LoadOldSceneButton;
    private Button SkillBasicButton;

    private ObjectField PreviewCharacterPrefabObjectField;
    private ObjectField PreviewCharacterObjectField;
    private ObjectField SkillConfigObjectField;
    private GameObject currentPreviewCharacterPrefab;
    private GameObject currentPreviewCharacterObj;
    public GameObject PreviewCharacterObj { get => currentPreviewCharacterObj; }

    private void InitTopMenu()
    {
        LoadEditorSceneButton = root.Q<Button>(nameof(LoadEditorSceneButton));
        LoadEditorSceneButton.clicked += LoadEditorSceneButtonClick;

        LoadOldSceneButton = root.Q<Button>(nameof(LoadOldSceneButton));
        LoadOldSceneButton.clicked += LoadOldSceneButtonClick;

        SkillBasicButton = root.Q<Button>(nameof(SkillBasicButton));
        SkillBasicButton.clicked += SkillBasicButtonClick;

        PreviewCharacterPrefabObjectField = root.Q<ObjectField>(nameof(PreviewCharacterPrefabObjectField));
        PreviewCharacterPrefabObjectField.RegisterValueChangedCallback(PreviewCharacterPrefabObjectFieldValueChanged);

        PreviewCharacterObjectField = root.Q<ObjectField>(nameof(PreviewCharacterObjectField));
        PreviewCharacterObjectField.RegisterValueChangedCallback(PreviewCharacterObjectFieldValueChanged);

        SkillConfigObjectField = root.Q<ObjectField>(nameof(SkillConfigObjectField));
        SkillConfigObjectField.objectType = typeof(SkillClip);
        SkillConfigObjectField.RegisterValueChangedCallback(SkillConfigObjectFieldValueChanged);
    }

    public bool OnEditorScene
    {
        get
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            return currentScenePath == skillEditorScenePath;
        }
    }

    // 加载编辑器场景
    private void LoadEditorSceneButtonClick()
    {
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        // 当前是编辑器场景，但是玩家依然点击了加载编辑器场景，没有意义
        if (currentScenePath == skillEditorScenePath) return;
        oldScenePath = currentScenePath;
        EditorSceneManager.OpenScene(skillEditorScenePath);
    }
    // 回归旧场景
    private void LoadOldSceneButtonClick()
    {
        if (!string.IsNullOrEmpty(oldScenePath))
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            // 当前场景和旧场景是同一个场景，没有切换意义
            if (currentScenePath == oldScenePath) return;
            EditorSceneManager.OpenScene(oldScenePath);
        }
        else Debug.LogWarning("场景不存在！");
    }
    // 查看技能基本信息
    private void SkillBasicButtonClick()
    {
        if (skillConfig != null)
        {
            Selection.activeObject = skillConfig;
        }
    }

    // 角色预制体修改
    private void PreviewCharacterPrefabObjectFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        // 避免在其他场景实例化
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        if (currentScenePath != skillEditorScenePath)
        {
            PreviewCharacterPrefabObjectField.value = null;
            return;
        }

        // 值相等，设置无效
        if (evt.newValue == currentPreviewCharacterPrefab) return;

        currentPreviewCharacterPrefab = (GameObject)evt.newValue;

        // 销毁旧的
        if (currentPreviewCharacterObj != null) DestroyImmediate(currentPreviewCharacterObj);
        Transform parent = GameObject.Find(previewCharacterParentPath).transform;
        if (parent != null && parent.childCount > 0)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
        }
        // 实例化新的
        if (evt.newValue != null)
        {
            currentPreviewCharacterObj = Instantiate(evt.newValue as GameObject, Vector3.zero, Quaternion.identity, parent);
            currentPreviewCharacterObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            PreviewCharacterObjectField.value = currentPreviewCharacterObj;
            if (currentPreviewCharacterObj.GetComponent<Skill_Player>() == null)
            {
                currentPreviewCharacterObj.AddComponent<Skill_Player>();
            }
        }
    }

    // 角色预览对象修改
    private void PreviewCharacterObjectFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        currentPreviewCharacterObj = (GameObject)evt.newValue;
    }

    // 技能配置修改
    private void SkillConfigObjectFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        SaveConfig();
        skillConfig = evt.newValue as SkillClip;
        CurrentSelectFrameIndex = 0;
        if (skillConfig == null)
        {
            CurrentFrameCount = 100;
        }
        else
        {
            CurrentFrameCount = skillConfig.FrameCount;
        }
        // 刷新轨道
        ResetTrack();
    }

    #endregion

    #region TimerShaft
    private IMGUIContainer timerShaft;
    private IMGUIContainer selectLine;
    private VisualElement contentContainer;
    private VisualElement contentViewPort;

    private int currentSelectFrameIndex = -1;
    public int CurrentSelectFrameIndex
    {
        get => currentSelectFrameIndex;
        private set
        {
            int old = currentSelectFrameIndex;
            // 如果超出范围，更新最大帧
            if (value > CurrentFrameCount) CurrentFrameCount = value;
            currentSelectFrameIndex = Mathf.Clamp(value, 0, CurrentFrameCount);
            CurrentFrameField.value = currentSelectFrameIndex;

            if (old != currentSelectFrameIndex)
            {
                UpdateTimerShaftView();
                TickSkill();
            }
        }
    }

    private int currentFrameCount;
    public int CurrentFrameCount
    {
        get => currentFrameCount;
        set
        {
            currentFrameCount = value;
            FrameCountField.value = currentFrameCount;
            // 同步给SkillConfig
            if (skillConfig != null)
            {
                skillConfig.FrameCount = currentFrameCount;
            }
            // Content区域的尺寸变化
            UpdateContentSize();
        }
    }

    // 当前内容区域的偏移坐标
    private float contentOffsetPos { get => Mathf.Abs(contentContainer.transform.position.x); }
    private float currentSlectFramePos { get => currentSelectFrameIndex * skillEditorConfig.frameUnitWidth; }
    private bool timerShaftIsMouseEnter = false;
    private void InitTimerShaft()
    {
        ScrollView MainContentView = root.Q<ScrollView>("MainContentView");
        contentContainer = MainContentView.Q<VisualElement>("unity-content-container");
        contentViewPort = MainContentView.Q<VisualElement>("unity-content-viewport");

        timerShaft = root.Q<IMGUIContainer>("TimerShaft");
        timerShaft.onGUIHandler = DrawTimerShaft;
        timerShaft.RegisterCallback<WheelEvent>(TimerShaftWheel);
        timerShaft.RegisterCallback<MouseDownEvent>(TimerShaftMouseDown);
        timerShaft.RegisterCallback<MouseMoveEvent>(TimerShaftMouseMove);
        timerShaft.RegisterCallback<MouseUpEvent>(TimerShaftMouseUp);
        timerShaft.RegisterCallback<MouseOutEvent>(TimerShaftMouseOut);

        selectLine = root.Q<IMGUIContainer>("SelectLine");
        selectLine.onGUIHandler = DrawSelectLine;
    }

    private void DrawTimerShaft()
    {
        Handles.BeginGUI();
        Handles.color = Color.white;
        Rect rect = timerShaft.contentRect;
        // 起始索引
        int index = Mathf.CeilToInt(contentOffsetPos / skillEditorConfig.frameUnitWidth);
        // 计算绘制起点的偏移
        float startOffset = 0;
        if (index > 0) startOffset = skillEditorConfig.frameUnitWidth - (contentOffsetPos % skillEditorConfig.frameUnitWidth);

        int tickStep = SkillEditorConfig.maxFrameWidthLV + 1 - (skillEditorConfig.frameUnitWidth / SkillEditorConfig.standFrameUnitWidth);
        tickStep = tickStep / 2; // 可能 1 / 2 = 0的情况
        if (tickStep == 0) tickStep = 1; // 避免为0
        for (float i = startOffset; i < rect.width; i += skillEditorConfig.frameUnitWidth)
        {
            // 绘制长线、文本
            if (index % tickStep == 0)
            {
                Handles.DrawLine(new Vector3(i, rect.height - 10), new Vector3(i, rect.height));
                string indexStr = index.ToString();
                GUI.Label(new Rect(i - indexStr.Length * 4.5f, 0, 35, 20), indexStr);
            }
            else Handles.DrawLine(new Vector3(i, rect.height - 5), new Vector3(i, rect.height));
            index += 1;
        }
        Handles.EndGUI();
    }

    private void TimerShaftWheel(WheelEvent evt)
    {
        int delta = (int)evt.delta.y;
        skillEditorConfig.frameUnitWidth = Mathf.Clamp(skillEditorConfig.frameUnitWidth - delta, SkillEditorConfig.standFrameUnitWidth, SkillEditorConfig.maxFrameWidthLV * SkillEditorConfig.standFrameUnitWidth);
        UpdateTimerShaftView();
        UpdateContentSize();
        ResetTrack();
    }

    private void TimerShaftMouseDown(MouseDownEvent evt)
    {
        // 让选中线得位置卡在帧的位置上
        timerShaftIsMouseEnter = true;
        IsPlaying = false;
        int newValue = GetFrameIndexByMousePos(evt.localMousePosition.x);
        if (newValue != CurrentSelectFrameIndex)
        {
            CurrentSelectFrameIndex = newValue;
        }
    }
    private void TimerShaftMouseMove(MouseMoveEvent evt)
    {
        if (timerShaftIsMouseEnter)
        {
            int newValue = GetFrameIndexByMousePos(evt.localMousePosition.x);
            if (newValue != CurrentSelectFrameIndex)
            {
                CurrentSelectFrameIndex = newValue;
            }
        }
    }
    private void TimerShaftMouseUp(MouseUpEvent evt)
    {
        timerShaftIsMouseEnter = false;
    }
    private void TimerShaftMouseOut(MouseOutEvent evt)
    {
        timerShaftIsMouseEnter = false;
    }

    /// <summary>
    /// 根据鼠标坐标获取帧索引
    /// </summary>
    public int GetFrameIndexByMousePos(float x)
    {
        return GetFrameIndexByPos(x + contentOffsetPos);
    }

    public int GetFrameIndexByPos(float x)
    {
        return Mathf.RoundToInt(x / skillEditorConfig.frameUnitWidth);
    }

    private void DrawSelectLine()
    {
        // 判断当前选中帧是否在视图范围内
        if (currentSlectFramePos >= contentOffsetPos)
        {
            Handles.BeginGUI();
            Handles.color = Color.white;
            float x = currentSlectFramePos - contentOffsetPos;
            Handles.DrawLine(new Vector3(x, 0), new Vector3(x, contentViewPort.contentRect.height + timerShaft.contentRect.height));
            Handles.EndGUI();
        }
    }

    private void UpdateTimerShaftView()
    {
        timerShaft.MarkDirtyLayout(); // 标志为需要重新绘制的
        selectLine.MarkDirtyLayout(); // 标志为需要重新绘制的
    }

    #endregion

    #region Console
    private Button PreviouFrameButton;
    private Button PlayButton;
    private Button NextFrameButton;
    private IntegerField CurrentFrameField;
    private IntegerField FrameCountField;
    private void InitConsole()
    {
        PreviouFrameButton = root.Q<Button>(nameof(PreviouFrameButton));
        PreviouFrameButton.clicked += PreviouFrameButtonClick;

        PlayButton = root.Q<Button>(nameof(PlayButton));
        PlayButton.clicked += PlayButtonClick;

        NextFrameButton = root.Q<Button>(nameof(NextFrameButton));
        NextFrameButton.clicked += NextFrameButtonClick;

        CurrentFrameField = root.Q<IntegerField>(nameof(CurrentFrameField));
        CurrentFrameField.RegisterValueChangedCallback(CurrentFrameFieldValueChanged);

        FrameCountField = root.Q<IntegerField>(nameof(FrameCountField));
        FrameCountField.RegisterValueChangedCallback(FrameCountValueChanged);
    }

    private void PreviouFrameButtonClick()
    {
        IsPlaying = false;
        CurrentSelectFrameIndex -= 1;
    }
    private void PlayButtonClick()
    {
        IsPlaying = !IsPlaying;
    }
    private void NextFrameButtonClick()
    {
        IsPlaying = false;
        CurrentSelectFrameIndex += 1;
    }
    private void CurrentFrameFieldValueChanged(ChangeEvent<int> evt)
    {
        if (CurrentSelectFrameIndex != evt.newValue) CurrentSelectFrameIndex = evt.newValue;
    }
    private void FrameCountValueChanged(ChangeEvent<int> evt)
    {
        if (CurrentFrameCount != evt.newValue) CurrentFrameCount = evt.newValue;
    }


    #endregion

    #region Config
    private SkillClip skillConfig;
    public SkillClip SkillConfig { get => skillConfig; }
    private SkillEditorConfig skillEditorConfig = new SkillEditorConfig();

    public void SaveConfig()
    {
        if (skillConfig != null)
        {
            EditorUtility.SetDirty(skillConfig);
            AssetDatabase.SaveAssetIfDirty(skillConfig);
            ResetTrackData();
        }
    }

    private void ResetTrackData()
    {
        // 重新引用一下数据
        for (int i = 0; i < trackList.Count; i++)
        {
            trackList[i].OnConfigChanged();
        }
    }

    #endregion

    #region Track
    private VisualElement trackMenuParent;
    private VisualElement contentListView;
    private List<SkillTrackBase> trackList = new List<SkillTrackBase>();
    private void InitContent()
    {
        contentListView = root.Q<VisualElement>("ContentListView");
        trackMenuParent = root.Q<VisualElement>("TrackMenuList");

        ScrollView trackMneuScrollView = root.Q<ScrollView>("TrackMenuScrollView");
        ScrollView mainContentView = root.Q<ScrollView>("MainContentView");

        trackMneuScrollView.verticalScroller.valueChanged += (value) =>
        {
            mainContentView.verticalScroller.value = value;
        };
        mainContentView.verticalScroller.valueChanged += (value) =>
        {
            trackMneuScrollView.verticalScroller.value = value;
        };
        // mainContentView.verticalScroller.valueChanged += ContentVerticalScorllerValueChanged;
        UpdateContentSize();
        InitTrack();
    }


    private void InitTrack()
    {
        // 如果没有配置，也不需要初始化轨道
        if (skillConfig == null) return;
        InitEventTrack();
        InitAnimationTrack();
        InitAudioTrack();
        InitEffectTrack();
        InitAttackDetectionTrack();
        // ....
    }


    private void InitEventTrack()
    {
        EventTrack eventTrack = new EventTrack();
        eventTrack.Init(trackMenuParent, contentListView, skillEditorConfig.frameUnitWidth);
        trackList.Add(eventTrack);
    }

    private void InitAnimationTrack()
    {
        AnimationTrack animationTrack = new AnimationTrack();
        animationTrack.Init(trackMenuParent, contentListView, skillEditorConfig.frameUnitWidth);
        trackList.Add(animationTrack);
        getPostionForRootMotion = animationTrack.GetPostionForRootMotion;
    }

    private void InitAudioTrack()
    {
        AudioTrack audioTrack = new AudioTrack();
        audioTrack.Init(trackMenuParent, contentListView, skillEditorConfig.frameUnitWidth);
        trackList.Add(audioTrack);
    }

    private void InitEffectTrack()
    {
        EffectTrack effectTrack = new EffectTrack();
        effectTrack.Init(trackMenuParent, contentListView, skillEditorConfig.frameUnitWidth);
        trackList.Add(effectTrack);
    }

    private void InitAttackDetectionTrack()
    {
        AttackDetectionTrack attackDetectionTrack = new AttackDetectionTrack();
        attackDetectionTrack.Init(trackMenuParent, contentListView, skillEditorConfig.frameUnitWidth);
        trackList.Add(attackDetectionTrack);
    }

    private void ResetTrack()
    {
        // 如果配置文件是Null，清理掉所有的轨道
        if (skillConfig == null)
        {
            DestoryTracks();
        }
        else
        {
            // 如果轨道列表里面没有数据，说明没有轨道，当时当前用户是有配置的，需要初始化轨道
            if (trackList.Count == 0)
            {
                InitTrack();
            }
            // 更新视图
            for (int i = 0; i < trackList.Count; i++)
            {
                trackList[i].ResetView(skillEditorConfig.frameUnitWidth);
            }
        }
    }

    private void DestoryTracks()
    {
        for (int i = 0; i < trackList.Count; i++)
        {
            trackList[i].Destory();
        }
        trackList.Clear();
    }

    private void UpdateContentSize()
    {
        contentListView.style.width = skillEditorConfig.frameUnitWidth * CurrentFrameCount;
    }



    public void ShowTrackItemOnInspecotr(TrackItemBase trackItem, SkillTrackBase track)
    {
        SkillEditorInspector.SetTrackItem(trackItem, track);
        Selection.activeObject = this;
    }
    #endregion

    #region Preview
    private bool isPlaying;
    public bool IsPlaying
    {
        get => isPlaying;
        set
        {
            isPlaying = value;
            if (isPlaying)
            {
                startTime = DateTime.Now;
                startFrameIndex = currentSelectFrameIndex;
                // OnPlay
                for (int i = 0; i < trackList.Count; i++)
                {
                    trackList[i].OnPlay(currentSelectFrameIndex);
                }
            }
            else
            {
                // OnStop
                for (int i = 0; i < trackList.Count; i++)
                {
                    trackList[i].OnStop();
                }
            }
        }
    }

    private DateTime startTime;
    private int startFrameIndex;

    private void Update()
    {
        if (IsPlaying)
        {
            // 得到时间差
            float time = (float)DateTime.Now.Subtract(startTime).TotalSeconds;

            // 确定时间轴的帧率
            float frameRote;
            if (skillConfig != null) frameRote = skillConfig.FrameRote;
            else frameRote = skillEditorConfig.defaultFrameRote;

            // 根据时间差计算当前的选中帧
            CurrentSelectFrameIndex = (int)((time * frameRote) + startFrameIndex);
            // 到达最后一帧自动暂停
            if (CurrentSelectFrameIndex == CurrentFrameCount)
            {
                IsPlaying = false;
            }
        }
    }
    public void TickSkill()
    {
        // 驱动技能表现
        if (skillConfig != null && currentPreviewCharacterObj != null)
        {
            for (int i = 0; i < trackList.Count; i++)
            {
                trackList[i].TickView(currentSelectFrameIndex);
            }
        }
    }

    private Func<int, bool, Vector3> getPostionForRootMotion;
    public Vector3 GetPostionForRootMotion(int frameIndex, bool recove = false) => getPostionForRootMotion(frameIndex, recove);
    #endregion

    #region Gizmo和SceneGUI
    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    private static void DrawGizmos(Skill_Player skill_Player, GizmoType gizmoType)
    {
        if (Instance == null || Instance.currentPreviewCharacterObj == null || Instance.currentPreviewCharacterObj.GetComponent<Skill_Player>() != skill_Player) return;
        for (int i = 0; i < Instance.trackList.Count; i++)
        {
            Instance.trackList[i].DrawGizmos();
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (currentPreviewCharacterObj == null) return;
        for (int i = 0; i < Instance.trackList.Count; i++)
        {
            Instance.trackList[i].OnSceneGUI();
        }
    }

    #endregion

}

public class SkillEditorConfig
{
    public const int standFrameUnitWidth = 10;  // 标准帧单位宽度
    public const int maxFrameWidthLV = 10;      //  当前帧单位宽度
    public int frameUnitWidth = 10;             //  当前帧单位宽度
    public float defaultFrameRote = 10;         //  默认帧率
}