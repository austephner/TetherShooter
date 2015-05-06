using UnityEngine;
using System.Collections;

/// <summary>
/// This is the controller script that handles user input and moves
/// the ship / tether nodes. 
/// </summary>

public class PlayerController : MonoBehaviour 
{
    // Settings
    public Transform NodeOne, NodeTwo, Ship; // ........................... Components of the ship
    public float ShipMovementDampening = .8f; // .......................... Rate at which the ship's movement slows down
    public float NodeSpeedMultiplier = 1f; // ............................. How fast the nodes can move with player input. 

    // Private
    private Vector3 CenterPoint; 

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
