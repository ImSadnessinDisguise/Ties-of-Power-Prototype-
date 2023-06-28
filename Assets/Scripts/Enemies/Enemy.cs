using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector]public EnemyDetailsSO enemyDetails;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;

    public GameObject EnemyPrefab;
    private LootBag lootBag;
    private EnemyMovementAI enemyMovementAI;

    private void Awake()
    {
        //Load Component
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        lootBag = GetComponent<LootBag>();
    }

    //Initialize the Enemy
    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
    {
        this.enemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);
    }

    //Set enemy movement update frame
    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        //set frame number that enemy should process its updates
        enemyMovementAI.SetUpdateFrameNumber(enemySpawnNumber % Settings.targetFrameRateToSpreadPathFindingOver);
    }

    private void OnDestroy()
    {
        if (EnemyPrefab.gameObject == null)
        {
            lootBag.InstantiateLoot(transform.position);
        }
    }
}
