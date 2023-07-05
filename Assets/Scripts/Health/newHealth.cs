using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3;
    private float currentHealth;

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
            Instantiate(GameResources.Instance.bloodSplat, transform.position, Quaternion.Euler(-45 , 0, 0));
        }
        else
        {
            EnemyDestroyed();
        }
        
    }

    private void EnemyDestroyed()
    {
        DestroyedEvent destroyedEvent = GetComponent<DestroyedEvent>();
        destroyedEvent.CallDestroyedEvent(false);
    }


}
