using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine 
{
    public PlayerState Currentstate { get; private set; }

    public void Initialize(PlayerState startingState)
    {
        Currentstate = startingState;
        Currentstate.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        Currentstate.Exit();
        Currentstate = newState;
        Currentstate.Enter();
    }
}
