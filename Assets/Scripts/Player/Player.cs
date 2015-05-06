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
    string name = "Player";
    PlayerNumber IdentificationNumber = PlayerNumber.PlayerOne; 


    // Private
    int Score = 0; 


	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    // Accessors & Mutators
    public void IncreaseScore(int Amount) { Score += Amount; }
}
