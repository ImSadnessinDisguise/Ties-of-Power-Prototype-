
public enum Orientation
{ 
  north,
  east,
  west,
  south,
  none
}

public enum ChestSpawnEvent
{
    onRoomEntry,
    OnEnemiesDefeated

}

public enum ChestSpawnPosition
{
    atSpawnerPosition,
    atPlayerPosition
}

public enum ChestState
{
    closed,
    healthItem,
    temmarItem,
    empty
}

public enum Gamestate
{
    gameStarted,
    playingLevel,
    engagingEnemies,
    bossStage,
    engagingBoss,
    levelCompleted,
    gameWon,
    gameLost,
    gamePause,
    dungeonOverviewMap,
    restartGame,
}
