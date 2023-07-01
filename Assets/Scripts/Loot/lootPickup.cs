using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lootPickup : MonoBehaviour
{
    private int value = 1;

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
            TeemarCount.instance.IncreaseTeemar(value);
        }
    }
}
