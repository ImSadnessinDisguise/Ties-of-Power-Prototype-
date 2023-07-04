using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    public int startingHealth;
    public int currentHealth;
    private HealthEvent healthEvent;
    private mainPlayer player;

    [HideInInspector] public bool isDamageable = true;
    [HideInInspector] public Enemy enemy;
    private void Awake()
    {
        //Load Component
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        //Trigger a healthEvent UI Update
        CallHealthEvent(0);

        //Attempt to load player and enemy
        player = GetComponent<mainPlayer>();
        enemy = GetComponent<Enemy>();
    }

    public void TakeDamage (int damageAmount)
    {
        bool isRolling = false;

        if (player != null)
            isRolling = player.playerControl.isRolling;

        if (isDamageable && !isRolling)
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);
        }

        if (isDamageable && isRolling)
        {
            Debug.Log("dodged a bullet there");
        }
    }

    private void CallHealthEvent(int damageAmount)
    {
        //trigger health event
        healthEvent.CallHealthChangedEvent(((float)currentHealth / (float)startingHealth), currentHealth, damageAmount);
    }

    ///<summary>
    ///Set Starting Health
    /// </summary>
    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    ///<summary>
    ///Get Starting Health
    /// </summary>
    public int GetStartingHealth()
    {
        return startingHealth;
    }

    public void AddHealth(int healthPercent)
    {
        int healthIncrease = Mathf.RoundToInt((startingHealth * healthPercent) / 100f);

        int totalHealth = currentHealth + healthIncrease;

        if (totalHealth > startingHealth)
        {
            currentHealth = startingHealth;
        }
        else
        {
            currentHealth = totalHealth;
        }
        CallHealthEvent(0);
    }
}

