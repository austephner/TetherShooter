using UnityEngine;
using System.Collections;

/// <summary>
/// Script controls how a level plays out. Where and what will spawn, 
/// how the player wins, etc. 
/// </summary>

public enum LevelDifficulty
{
    EASY = 0, 
    NORMAL = 1, 
    HARD = 2,
    EXTREME = 3
}

public class LevelManager : MonoBehaviour 
{
    // Settings
    string LevelName = "Level";
    LevelDifficulty Difficulty = LevelDifficulty.NORMAL;
    int TimeLimit = 10; // ................................................ Measured in minutes

    // Private


	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
