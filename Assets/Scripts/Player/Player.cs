using UnityEngine;
using System.Collections;

/// <summary>
/// This script manages players and their data. It has nothing to do with input
/// or controlling, only class interaction and actual data manipulation. 
/// </summary>

public enum PlayerNumber
{
    PlayerOne = 1,
    PlayerTwo = 2
}

public class Player : MonoBehaviour 
{
    // Settings
    public string name = "Player";
    public PlayerNumber IdentificationNumber = PlayerNumber.PlayerOne;
    public PlayerController ControllerObject = null;


    // Private
    int Score = 0;
    GameManager GM = null;


	// Use this for initialization
	void Start () 
    {
        // Check if the playerController object is null. 
        // If so, look for a playerController object attached to the parent of this object. 
        if (!ControllerObject) if (GetComponent<PlayerController>()) ControllerObject = GetComponent<PlayerController>();

        // Try and find the game manager object
        if (!GM)
        {
            if (FindObjectOfType<GameManager>())
            {
                GM = FindObjectOfType<GameManager>();
                // GM.AddPlayer(this); 
            }
            else Debug.LogError("WARNING ... Failed to find a GameManager object for this playerobject.");
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    // Accessors & Mutators
    public void IncreaseScore(int Amount) { Score += Amount; }
}
