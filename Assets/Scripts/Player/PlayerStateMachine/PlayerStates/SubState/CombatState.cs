using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState : AbilityState
{
    public CombatState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {

    }
}
