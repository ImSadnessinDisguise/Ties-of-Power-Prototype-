using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO enemyDetailsSO;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;

    private void Awake()
    {
        //Load Component
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>(); 
    }

}
