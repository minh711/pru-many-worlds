public static class BattleConstants
{
    public enum BattleState
    {
        LOADING,
        IN_TURN,
        WAITING_FOR_NEXT_TURN,
        PLAYING,
        PAUSED
    }

    public enum SpawnPoints : int
    {
        ALLY_SPAWN_POINTS = 0,
        ENEMY_SPAWN_POINTS = 1
    }
}
