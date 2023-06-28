using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class areaAttack : MonoBehaviour
{
    private float damage = 1;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<newHealth>() != null)
        {
            newHealth health = collider.GetComponent<newHealth>();

            health.TakeDamage(damage);
        }
    }
}
