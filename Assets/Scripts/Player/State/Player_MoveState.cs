using JKFrame;
using System;
using UnityEngine;

/// <summary>
/// 玩家移动状态
/// </summary>
public class Player_MoveState : PlayerStateBase
{
    private CharacterController characterController;
    private float runTransition;
    private bool applyRootMotionForMove;
    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        characterController = player.CharacterController;
        applyRootMotionForMove = player.CharacterConfig.ApplyRootMotionForMove;
    }
    public override void Enter()
    {
        // 播放待机动作
        runTransition = 0;
        Action<Vector3, Quaternion> rootMotionAction = null;
        if (applyRootMotionForMove) rootMotionAction = OnRootMotion;
        player.PlayBlendAnimation("Walk", "Run", rootMotionAction);
        animation.SetBlendWeight(1);
        animation.AddAniamtionEvent("FootStep", OnFootStep);
    }
    public override void Update()
    {
        if (UISystem.CheckMouseOnUI()) player.ChangeState(PlayerState.Idle); ;
        if (CheckAndEnterSkillState()) return;
        Vector2 moveInput = InputManager.Instance.GetMoveInput();
        if (moveInput.x == 0 && moveInput.y == 0)
        {
            // 切换状态
            player.ChangeState(PlayerState.Idle);
        }
        else
        {
            // 处理移动
            Vector3 input = new Vector3(moveInput.x, 0, moveInput.y);
            if (Input.GetKey(KeyCode.LeftShift))
                runTransition = Mathf.Clamp(runTransition + Time.deltaTime * player.CharacterConfig.Walk2RunTransitionSpeed, 0, 1);
            else
                runTransition = Mathf.Clamp(runTransition - Time.deltaTime * player.CharacterConfig.Walk2RunTransitionSpeed, 0, 1);

            animation.SetBlendWeight(1 - runTransition);

            // 获取相机的y旋转值
            float y = Camera.main.transform.rotation.eulerAngles.y;
            // 让input也旋转y角度
            // 四元数和向量相乘：表示这个向量按照这个四元数进行旋转之后得到新的向量
            Vector3 moveDir = Quaternion.Euler(0, y, 0) * input;
            if (!applyRootMotionForMove)
            {
                float speed = Mathf.Lerp(player.WalkSpeed, player.RunSpeed, runTransition);
                Vector3 motion = Time.deltaTime * speed * moveDir;
                motion.y = -9.8f * Time.deltaTime;
                characterController.Move(motion);
            }
            // 处理旋转
            player.Rotate(input);
        }
    }

    public override void Exit()
    {
        if (applyRootMotionForMove) animation.ClearRootMotionAction();
        animation.RemoveAnimationEvent("FootStep", OnFootStep);
    }
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        // 此时的速度是影响动画播放速度来达到实际位移速度的变化
        float speed = Mathf.Lerp(player.WalkSpeed, player.RunSpeed, runTransition);
        animation.Speed = speed;
        deltaPosition.y = -9.8f * Time.deltaTime;
        characterController.Move(deltaPosition);
    }


}
