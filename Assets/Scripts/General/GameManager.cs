using UnityEngine;
using System.Collections;

/// <summary>
/// Script maintains flow and control of the game. Organizes all current players, network connections, 
/// levels progressions, etc. 
/// </summary>

public class GameManager : MonoBehaviour 
{
    // Settings
    public string[] LevelLoadNames;  // .................................. The order in which Application.LoadLevel() is used
    public Player[] Players; // .......................................... Current players in the game for easy / quick reference.


	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
