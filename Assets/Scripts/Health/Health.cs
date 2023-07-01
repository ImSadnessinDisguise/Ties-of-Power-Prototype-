using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    public int startingHealth;
    public int currentHealth;
    private HealthEvent healthEvent;
    private Player player;

    [HideInInspector] public bool isDamageable = true;
    private void Awake()
    {
        //Load Component
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        //Trigger a healthEvent UI Update
        CallHealthEvent(0);

        //Attempt to load player
        player = GetComponent<Player>();
    }

    public void TakeDamage (int damageAmount)
    {
        if (isDamageable)
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);
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

