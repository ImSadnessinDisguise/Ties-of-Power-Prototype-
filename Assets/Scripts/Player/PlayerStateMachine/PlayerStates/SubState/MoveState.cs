using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : MovementState
{
    protected Vector2 direction;
    public MoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {

    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (input.x == 0 && input.y == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }

        player.MovePlayer();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
