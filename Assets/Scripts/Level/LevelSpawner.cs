using UnityEngine;
using System.Collections;

/// <summary>
/// Used to spawn obstacles and enemies inside the level. It will also 
/// spawn background items (like distant planets, asteroids, etc.) if 
/// enabled. 
/// </summary>

public enum SpawnFrequency
{
    NEVER = 0,
    RARE = 1, 
    UNCOMMON = 2, 
    COMMON = 3, 
    FREQUENT = 4, 
    LUDICROUS = 5, 
}

public class LevelSpawner : MonoBehaviour 
{
    // Settings
    public Transform[] EnemyPrefabs; // ................................... All of the enemies that'll be used to spawn in the game
    public Transform[] ObstaclePrefabs; // ................................ All of the obstacles (like immediate-level asteroids or junk)
    public Transform[] DistantObjectsPrefabs; // .......................... Objects that'll be spawned into the background like planets.
    public bool EnableDistantObjects = true;
    public SpawnFrequency EnemySpawnFrequency = SpawnFrequency.UNCOMMON;
    public SpawnFrequency ObstacleSpawnFrequency = SpawnFrequency.COMMON;
    public SpawnFrequency DistantObjectSpawnFrequency = SpawnFrequency.RARE;


	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
