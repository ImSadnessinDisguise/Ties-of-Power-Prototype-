using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }
    public bool isRolling { get; private set; }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnRollInput(InputAction.CallbackContext context)
    {

    }
    public void OnSprintInput(InputAction.CallbackContext context)
    {

    }

    public void useRoll() => isRolling = false;

}
