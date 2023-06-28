using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3;
    private float currentHealth;
    private LootBag lootBag;


    public void Awake()
    {
        currentHealth = startingHealth;
    }

    public void Update()
    {
        //
    }
    
    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            //hurt
        }
        else
        {
            EnemyDestroyed();
        }
        
    }

    private void EnemyDestroyed()
    {
        DestroyedEvent destroyedEvent = GetComponent<DestroyedEvent>();
        destroyedEvent.CallDestroyedEvent();
    }


}
