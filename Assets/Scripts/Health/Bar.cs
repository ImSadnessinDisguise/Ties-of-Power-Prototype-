using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public GameObject heartPrefab;
    private int startingHealth;
    public Health health;

    private List<HealthBar> hearts = new List<HealthBar>();

    public void Awake()
    {
        health = GetComponent<Health>();
        startingHealth = health.GetStartingHealth();
    }

    public void Start()
    {
        DrawHearts();
    }

    public void DrawHearts()
    {
        
    }


    

    public void ClearHearts()
    {
        
    }

}
