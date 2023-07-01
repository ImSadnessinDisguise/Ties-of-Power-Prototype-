using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonoBehaviour<EnemySpawner>
{
    private int enemiesToSpawn;
    private int currentEnemyCount;
    private int enemiesSpawnedSoFar;
    private int enemyMaxConcurrentSpawnNumber;
    private Room currentRoom;
    private RoomEnemySpawnParameters roomEnemySpawnParameters;

    private void OnEnable()
    {
        //Subscribe to room changed event
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        //Unsubscribe to room changed event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    /// <summary>
    /// process change in a room
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        enemiesSpawnedSoFar = 0;
        currentEnemyCount = 0;

        currentRoom = roomChangedEventArgs.room;

        //if room is a corridor or entrance then return
        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
            return;

        //if the room has been cleared then return
        if (currentRoom.isClearedOfEnemies) return;

        //get random enemies to spawn
        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

        //get enemy spawn parameter
        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());

        //if no enemies to spawn then return
        if (enemiesToSpawn == 0)
        {
            //mark the room as cleared
            currentRoom.isClearedOfEnemies = true;

            return;
        }

        //Get concurrent number enemeis to spawn
        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        //Lock doors
        currentRoom.instantiatedRoom.LockDoors();

        //Spawn Enemies
        SpawnEnemies();
    }

    /// <summary>
    /// spawn the enemies
    /// </summary>
    private void SpawnEnemies()
    {
        if (GameManager.Instance.gameState == Gamestate.engagingBoss)
        {
            GameManager.Instance.previousGamestate = Gamestate.bossStage;
            GameManager.Instance.gameState = Gamestate.engagingBoss;
        }    

        //set gamestate engaging enemies 
        else if (GameManager.Instance.gameState == Gamestate.playingLevel)
        {
            GameManager.Instance.previousGamestate = Gamestate.playingLevel;
            GameManager.Instance.gameState = Gamestate.engagingEnemies;


        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    //coroutine
    private IEnumerator SpawnEnemiesRoutine()
    {
        Grid grid = currentRoom.instantiatedRoom.grid;

        //create an instance of the helper class used to select a random enemy
        RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

        //check we have somewhere to to spawn the enemies
        if (currentRoom.spawnPositionArray.Length > 0)
        {
            //Loop through to create all enemies
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
                {
                    yield return null;
                }

                Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));

                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }


    //get a random spawn interval between maximum and minimum
    private float GetEnemySpawnInterval()
    {
        return (Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval));
    }

    //get concurrent enemies
    private int GetConcurrentEnemies()
    {
        return (Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies));
    }

    /// <summary>
    /// Create an enemy in the specified zone
    /// </summary>
    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
    {
        //keep track of the enemies so far
        enemiesSpawnedSoFar++;

        //Add one to the current enemy count - this is reduced when an enemy is destroyed
        currentEnemyCount++;

        //get current dungeon level
        DungeonLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        //Instantiate enemy
        GameObject enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.Euler(-45,0,0), transform);

        //Initialize the enemy
        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, enemiesSpawnedSoFar, dungeonLevel);

        //subscribe to destroyed event
        enemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
    }

    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        //Unsubscribe
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;

        //reduce enemycount
        currentEnemyCount--;

        if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
        {
            currentRoom.isClearedOfEnemies = true;

            //Set the game state
            if (GameManager.Instance.gameState == Gamestate.engagingEnemies)
            {
                GameManager.Instance.gameState = Gamestate.playingLevel;
                GameManager.Instance.previousGamestate = Gamestate.engagingEnemies;
            }
            else if (GameManager.Instance.gameState == Gamestate.engagingBoss)
            {
                GameManager.Instance.gameState = Gamestate.bossStage;
                GameManager.Instance.previousGamestate = Gamestate.engagingBoss;
            }

            //unlock door
            currentRoom.instantiatedRoom.UnlockDoors(Settings.doorUnlockDelay);
        }
    }
}
