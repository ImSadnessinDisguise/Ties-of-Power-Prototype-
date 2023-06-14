using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(mainPlayer))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private mainPlayer player;

    private void Awake()
    {
        //Load component
        player = GetComponent<mainPlayer>();
    }

    private void OnEnable()
    {
        //Subscribe to idle event
        player.idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        //Unsubsribe from idle event
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    /// <summary>
    /// Onn idle event handler
    /// </summary>
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        SetIdleAnimationParameters();
    }

    private void SetIdleAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
    }
} 
