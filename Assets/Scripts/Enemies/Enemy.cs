using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MaterializeEffect))]
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public BoxCollider2D boxCollider2D;
    [HideInInspector] public Animator animator;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;

    public GameObject EnemyPrefab;
    private LootBag lootBag;
    private MaterializeEffect materializeEffect;
    private EnemyMovementAI enemyMovementAI;

    private void Awake()
    {
        //Load Component
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        materializeEffect = GetComponent<MaterializeEffect>();
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
        lootBag.InstantiateLoot(transform.position);
    }

    private IEnumerator MaterializeEnemy()
    {
        EnemyEnable(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor,
            enemyDetails.enemyMaterializeTime, spriteRendererArray, enemyDetails.enemyStandardMaterial));

        EnemyEnable(true);
    }

    private void EnemyEnable(bool isEnabled)
    {
        boxCollider2D.enabled = isEnabled;

        enemyMovementAI.enabled = isEnabled;
    }
}
