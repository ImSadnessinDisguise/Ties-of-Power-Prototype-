using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("MovementDetailsSO scriptable object containing movespeed details such as speed")]
    #endregion 
    [SerializeField] private MovementDetailsSO movementDetails;
    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public float moveSpeed;
    private bool chasePlayer = false;
    [HideInInspector] public int updateFrameNumber = 1; //default value set by enemy spawner

    private void Awake()
    {
        //load component
        enemy = GetComponent<Enemy>();

        moveSpeed = movementDetails.GetMoveSpeed();

    }

    private void Start()
    {
        //create wait for fixed update for use in coroutine
        waitForFixedUpdate = new WaitForFixedUpdate();

        //reset player reference position
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();
    }

    /// <summary>
    /// use Astar pathfinding to build a path to the player and then move the enemy to each grid location on the path
    /// </summary>
    private void MoveEnemy()
    {
        //Movement cooldown timer
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        //Check distance to player to see if enemy should start chasing
        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) < enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }

        //if not close enough to chase player then return
        if (!chasePlayer)
            return;

        //Only process A star path rebuild on certain frames to spread load between enemies
        if (Time.frameCount % Settings.targetFrameRateToSpreadPathFindingOver != updateFrameNumber) return;

        //If the movement cooldown timer reached or player has moved more than required distance
        //rebuild the enemy path and move away
        if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition, GameManager.Instance.GetPlayer().GetPlayerPosition()) >
            Settings.playerMoveDistanceToRebuildPath))
        {
            //reset path rebuild cooldown timer
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            //reset player reference position
            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            //move enemy using AStar pathfinding - trigger to rebuild path to player
            CreatePath();

            //if path has been found move enemy
            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    //trigger idle event
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }
                //move enemy along the path using a coroutine
                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }
        }
    }

    /// <summary>
    /// Coroutine to move the enemy
    /// </summary>
    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();

            //while not very close continue to move - when close move onto the next step
            while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                //Trigger Movement Event
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position, moveSpeed, (nextPosition -
                    transform.position).normalized);

                yield return waitForFixedUpdate; //move the enemy using 2d physics so wait until next fixed update
            }

            yield return waitForFixedUpdate;
        }

        //end of path steps - trigger the enemy idle event
        enemy.idleEvent.CallIdleEvent();
    }

    /// <summary>
    /// Use the Astar static class to create a path for the enemy
    /// </summary>
    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();
        Grid grid = currentRoom.instantiatedRoom.grid;

        //enemy position on grid
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        //player position on the grid
        Vector3Int playerGridPosition = GetNearestNonObstacleToPlayerPosition(currentRoom);
        
        //build a path for the enemy 
        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        //take off first step on the path - this is the grid square the enemy is on
        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
        else
        {
            //Trigger idle event
            enemy.idleEvent.CallIdleEvent(); 
        }
    }

    /// <summary>
    /// Set the frame number that the enemy path will be recaluclated on - avoid performance spikes
    /// </summary>
    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }

    /// <summary>
    /// get the nearest position to the player that isnt an obstacle
    /// </summary>
    private Vector3Int GetNearestNonObstacleToPlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        Vector3Int playercellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerCellPosition = new Vector2Int(playercellPosition.x - currentRoom.templateLowerBounds.x, playercellPosition.y -
            currentRoom.templateLowerBounds.y);

        int obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y];

        //if the player isnt on a cell marked as an obstacle then return that position
        if (obstacle != 0)
        {
            return playercellPosition;
        }
        //find surrounding that isnt an obstacle
        //the player can be on a grid square that is marked as an obstacle
        else
        {
            for(int i = -1; i <= 1; i ++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0) continue;

                    try
                    {
                        obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x + i, adjustedPlayerCellPosition.y + j];
                        if (obstacle != 0) return new Vector3Int(playercellPosition.x + i, playercellPosition.y + j, 0);
                    }
                    catch
                    {
                        continue;
                    }
                }

            }

            //No non obstacle cells surrounding the player so just return the player position
            return playercellPosition;

        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
    #endregion
}
