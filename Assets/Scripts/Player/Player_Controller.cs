using JKFrame;
using System;
using UnityEngine;

public class Player_Controller : SingletonMono<Player_Controller>, IStateMachineOwner, ICharacter
{
    [SerializeField] private Player_SkillBrainBase skillBrain;
    [SerializeField] private Player_View view;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private BuffController buffController;
    [SerializeField] private CharacterProperties characterProperties;
    public CharacterController CharacterController { get => characterController; }
    public Player_SkillBrainBase SkillBrain { get => skillBrain; }
    public CharacterConfig CharacterConfig { get => characterConfig; }
    public Animation_Controller Animation_Controller { get => view.Animation; }
    public Transform ModelTransform { get => view.transform; }
    public CharacterProperties CharacterProperties { get => characterProperties; }
    public float WalkSpeed { get => characterConfig.WalkSpeed; }
    public float RunSpeed { get => characterConfig.RunSpeed; }
    public float RotateSpeed { get => characterConfig.RotateSpeed; }

    private StateMachine stateMachine;
    private PlayerState playerState; // 玩家的当前状态
    private CharacterConfig characterConfig;

    public void Init(CharacterConfig characterConfig, GameData gameData)
    {
        this.characterConfig = characterConfig;
        view.InitOnGame(gameData);
        characterProperties.Init(characterConfig);
        skillBrain.Init(this, gameData.SkillLearnedDatas);
        // 初始化状态机
        stateMachine = ResSystem.GetOrNew<StateMachine>();
        stateMachine.Init(this);
        // 默认状态为待机
        ChangeState(PlayerState.Idle);
    }

    /// <summary>
    /// 修改状态
    /// </summary>
    public void ChangeState(PlayerState playerState, bool reCurrstate = false)
    {
        this.playerState = playerState;
        switch (playerState)
        {
            case PlayerState.Idle:
                stateMachine.ChangeState<Player_IdleState>(reCurrstate);
                break;
            case PlayerState.Move:
                stateMachine.ChangeState<Player_MoveState>(reCurrstate);
                break;
            case PlayerState.Skill:
                stateMachine.ChangeState<Player_SkillState>(reCurrstate);
                break;
        }
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void PlayAnimation(string animationClipName, Action<Vector3, Quaternion> rootMotionAction = null, float speed = 1, bool refreshAnimation = false, float transitionFixedTime = 0.25f)
    {
        if (rootMotionAction != null)
        {
            view.Animation.SetRootMotionAction(rootMotionAction);
        }
        view.Animation.PlaySingleAniamtion(characterConfig.GetAnimationByName(animationClipName), speed, refreshAnimation, transitionFixedTime);
    }

    /// <summary>
    /// 播放混合动画
    /// </summary>
    public void PlayBlendAnimation(string clip1Name, string clip2Name, Action<Vector3, Quaternion> rootMotionAction = null, float speed = 1, float transitionFixedTime = 0.25f)
    {
        if (rootMotionAction != null)
        {
            view.Animation.SetRootMotionAction(rootMotionAction);
        }
        AnimationClip clip1 = characterConfig.GetAnimationByName(clip1Name);
        AnimationClip clip2 = characterConfig.GetAnimationByName(clip2Name);
        view.Animation.PlayBlendAnimation(clip1, clip2, speed, transitionFixedTime);
    }

    public void Rotate(Vector3 input, float rotateSpeed = 0)
    {
        if (rotateSpeed == 0) rotateSpeed = RotateSpeed;

        // 获取相机的y旋转值
        float y = Camera.main.transform.rotation.eulerAngles.y;
        // 让input也旋转y角度
        // 四元数和向量相乘：表示这个向量按照这个四元数进行旋转之后得到新的向量
        Vector3 moveDir = Quaternion.Euler(0, y, 0) * input;
        // 处理旋转
        ModelTransform.rotation = Quaternion.Slerp(ModelTransform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * rotateSpeed);
    }

    public void AddBuff(BuffConfig buffConfig, int layer)
    {
        buffController.AddBuff(buffConfig, layer);
    }

    public float GetAttackValue(SkillAttackDetectionEvent detectionEvent)
    {
        return characterProperties.attack.Total * detectionEvent.AttackHitConfig.AttackMultiply;
    }

    public void BeHit(AttackData attackData)
    {
        // TODO:
    }

    public void OnSkillRotate()
    {
        Vector2 moveInput = InputManager.Instance.GetMoveInput();
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            Rotate(new Vector3(moveInput.x, 0, moveInput.y));
        }
    }

    public void ChangeToIdleState()
    {
        ChangeState(PlayerState.Idle);
    }

    public void OnSkillMove(Vector3 deltaPosition)
    {
        CharacterController.Move(deltaPosition);
    }

    public void OnSkillRotate(Quaternion deltaRotation)
    {
        ModelTransform.rotation *= deltaRotation;
    }
}
