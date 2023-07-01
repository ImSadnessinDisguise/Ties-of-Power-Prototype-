using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [System.Serializable]
    private struct RangeByLevel
    {
        public DungeonLevelSO dungeonLevel;
        [Range(0, 100)] public int min;
        [Range(0, 100)] public int max;

    }

    #region Header Chest Prefab
    [Space(10)]
    [Header("Chest Prefab")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with Chest Prefab")]
    #endregion
    [SerializeField] private GameObject chestPrefab;

    #region Header Spawn Chest Chance
    [Space(10)]
    [Header("Chest Spawn Chance")]
    #endregion
    #region Tooltip
    [Tooltip("The minimum possibility of spawning a chest")]
    #endregion
    [SerializeField] [Range(0, 100)] private int chestSpawnChanceMin;
    #region Tooltip
    [Tooltip("The maximum possibility of spawning a chest")]
    #endregion
    [SerializeField] [Range(0, 100)] private int chestSpawnChanceMax;
    #region Tooltip
    [Tooltip("You can override the chest spawn chance by dungeon level")]
    #endregion
    [SerializeField] private List<RangeByLevel> chestSpawnChanceByLevelList;

    #region Header Chest Spawn Details
    [Space(10)]
    [Header("CHEST SPAWN DETAILS")]
    #endregion 
    [SerializeField] private ChestSpawnEvent chestSpawnEvent;
    [SerializeField] private ChestSpawnPosition chestSpawnPosition;
    #region Tooltip
    [Tooltip("The minimum number of items to spawn (note that a maximum of 1 of each type of ammo, health, and weapon will be spawned")]
    #endregion Tooltip
    [SerializeField] [Range(0, 3)] private int numberOfItemsToSpawnMin;
    #region Tooltip
    [Tooltip("The maximum number of items to spawn (note that a maximum of 1 of each type of ammo, health, and weapon will be spawned")]
    #endregion Tooltip
    [SerializeField] [Range(0, 3)] private int numberOfItemsToSpawnMax;

    #region Header Chest Content Details
    [Space(10)]
    [Header("CHEST CONTENT DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("The range of health to spawn for each level")]
    #endregion
    [SerializeField] private List<RangeByLevel> healthSpawnByLevelList;

    private bool chestSpawned = false;
    private Room chestRoom;

    private void OnEnable()
    {
        // Subscribe to room changed event
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        // Subscribe to room enemies defeated event
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void OnDisable()
    {
        // Unsubscribe from room changed event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        // Unsubscribe from room enemies defeated event
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
    }

    /// <summary>
    /// Handle room changed event
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        // Get the room the chest is in if we don't already have it
        if (chestRoom == null)
        {
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }

        // If the chest is spawned on room entry then spawn chest
        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onRoomEntry && chestRoom == roomChangedEventArgs.room)
        {
            SpawnChest();
        }
    }

    /// <summary>
    /// Handle room enemies defeated event
    /// </summary>
    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        // Get the room the chest is in if we don't already have it
        if (chestRoom == null)
        {
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }

        // If the chest is spawned when enemies are defeated and the chest is in the room that the
        // enemies have been defeated
        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.OnEnemiesDefeated && chestRoom == roomEnemiesDefeatedArgs.room)
        {
            SpawnChest();
        }
    }

    /// <summary>
    /// Spawn the chest prefab
    /// </summary>
    private void SpawnChest()
    {
        chestSpawned = true;

        // Should chest be spawned based on specified chance? If not return.
        if (!RandomSpawnChest()) return;

        // Get Number Of Ammo,Health, & Weapon Items To Spawn (max 1 of each)
        GetItemsToSpawn(out int healthNum);

        // Instantiate chest
        GameObject chestGameObject = Instantiate(chestPrefab, this.transform);

        // Position chest
        if (chestSpawnPosition == ChestSpawnPosition.atSpawnerPosition)
        {
            chestGameObject.transform.position = this.transform.position;
        }
        else if (chestSpawnPosition == ChestSpawnPosition.atPlayerPosition)
        {
            // Get nearest spawn position to player
            Vector3 spawnPosition = HelperUtilities.GetSpawnPositionNearestToPlayer(GameManager.Instance.GetPlayer().transform.position);

            // Calculate some random variation
            Vector3 variation = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

            chestGameObject.transform.position = spawnPosition + variation;
        }

        // Get Chest component
        Chest chest = chestGameObject.GetComponent<Chest>();

        // Initialize chest
        if (chestSpawnEvent == ChestSpawnEvent.onRoomEntry)
        {
            // Don't use materialize effect
            chest.Initialize(false, GetHealthPercentToSpawn(healthNum));
        }
        else
        {
            // use materialize effect
            chest.Initialize(true, GetHealthPercentToSpawn(healthNum));
        }
    }

    /// <summary>
    /// Check if a chest should be spawned based on the chest spawn chance - returns true if chest should be spawned false otherwise
    /// </summary>
    private bool RandomSpawnChest()
    {
        int chancePercent = Random.Range(chestSpawnChanceMin, chestSpawnChanceMax + 1);

        // Check if an override chance percent has been set for the current level
        foreach (RangeByLevel rangeByLevel in chestSpawnChanceByLevelList)
        {
            if (rangeByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                chancePercent = Random.Range(rangeByLevel.min, rangeByLevel.max + 1);
                break;
            }
        }

        // get random value between 1 and 100
        int randomPercent = Random.Range(1, 100 + 1);

        if (randomPercent <= chancePercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Get the number of items to spawn - max 1 of each - max 3 in total
    /// </summary>
    private void GetItemsToSpawn(out int health)
    {

        health = 0;

        int numberOfItemsToSpawn = Random.Range(numberOfItemsToSpawnMin, numberOfItemsToSpawnMax + 1);

        int choice;

        if (numberOfItemsToSpawn == 1)
        {
            choice = Random.Range(0, 3);
            if (choice == 1) { health++; return; }
            return;
        }
    }


    /// <summary>
    /// Get health percent to spawn
    /// </summary>
    private int GetHealthPercentToSpawn(int healthNumber)
    {
        if (healthNumber == 0) return 0;

        // Get ammo spawn percent range for level
        foreach (RangeByLevel spawnPercentByLevel in healthSpawnByLevelList)
        {
            if (spawnPercentByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                return Random.Range(spawnPercentByLevel.min, spawnPercentByLevel.max);
            }
        }

        return 0;
    }


}
