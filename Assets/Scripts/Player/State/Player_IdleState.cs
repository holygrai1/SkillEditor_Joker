using JKFrame;
using System;
using UnityEngine;

/// <summary>
/// 玩家待机状态
/// </summary>
public class Player_IdleState : PlayerStateBase
{
    public override void Enter()
    {
        // 播放待机动作
        player.PlayAnimation("Idle");
    }

    public override void Update()
    {
        if (UISystem.CheckMouseOnUI()) return;
        if (CheckAndEnterSkillState()) return;
        player.CharacterController.Move(new Vector3(0, -9.8f * Time.deltaTime, 0));
        // 检测玩家的输入
        Vector2 moveInput = InputManager.Instance.GetMoveInput();
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            // 切换状态
            player.ChangeState(PlayerState.Move);
        }

    }
}
