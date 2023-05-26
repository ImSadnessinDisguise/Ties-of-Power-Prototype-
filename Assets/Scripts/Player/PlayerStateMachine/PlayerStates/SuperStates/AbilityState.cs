using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState : PlayerState
{

    private Vector2 input;
    public AbilityState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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

        if (input.x != 0 || input.y != 0)
        {
            stateMachine.ChangeState(player.moveState);
        }
        else if (input.x == 0 && input.y == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
