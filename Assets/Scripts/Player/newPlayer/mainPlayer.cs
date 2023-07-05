using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region REQUIRED COMPONENTS
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
#endregion
[DisallowMultipleComponent]
public class mainPlayer : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public Health health;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public DestroyedEvent destroyedEvent;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Controller playerControl;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        destroyedEvent = GetComponent<DestroyedEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerControl = GetComponent<Controller>();
    }

    public void Initialize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        //set player starting health
        SetPlayerHealth();
    }

    private void OnEnable()
    {
        //Subscribe to player health event
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        //Unsubscribe to player health event
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        Debug.Log("Health Amount = " + healthEventArgs.healthAmount);

        if (healthEventArgs.healthAmount <= 0)
        {
            GameManager.Instance.GetPlayer().animator.SetTrigger("die");

            new WaitForSeconds(2);

            destroyedEvent.CallDestroyedEvent(true);
        }
    }


    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetails.playerHealthAmount);
    }

    ///<summary>
    ///returns player position
    ///</summary>
    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }
}
