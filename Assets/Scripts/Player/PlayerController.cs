using UnityEngine;
using System.Collections;

/// <summary>
/// This is the controller script that handles user input and moves
/// the ship / tether nodes. 
/// </summary>

public class PlayerController : MonoBehaviour 
{
    // Settings
    public bool DebugPlayer = true; 
    public Transform NodeOne, NodeTwo, Ship; // ........................... Components of the ship
    public float NodeSpeedMultiplier = 1f; // ............................. How fast the nodes can move with player input. 
    public float ElasticityMultiplier = 1f; // ............................ The reactionary stretchiness of the nodes' distance from one another
    public float MaxNodeDistance = 900; // ................................ Max distance between the two nodes before horizontal message
    public Vector3 PositionalOffset = new Vector3(0, 0, 0); // ............ How far away from the center average the ship will rest at

    // Private
    private Vector3 CenterPoint = new Vector3(0,0,0);
    private float NodeDistance = 0f; 

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () 
    {
        // Get node distance
        NodeDistance = Vector3.Distance(NodeOne.position, NodeTwo.position); 

        // Get Center point & adjust y zero
        CenterPoint = (NodeOne.position + NodeTwo.position) / 2;
        CenterPoint = new Vector3(CenterPoint.x, 0, CenterPoint.z); 

        // Slerp ship towards center point & adjust y zero
        Ship.position = Vector3.Slerp(Ship.position, CenterPoint, .01f);
        Ship.position = new Vector3(Ship.position.x, 0, Ship.position.z); 

        // Call TestInput
        TestInput(DebugPlayer); 
	}

    // TestInput(enabled) :: Does testing input, won't as long as passed a false value. Subject to constant change
    public void TestInput(bool DoTests)
    {
        if (!DoTests) return;

        if (Input.GetKey(KeyCode.W))
        {
            NodeOne.transform.position += transform.forward;
            NodeTwo.transform.position += transform.forward; 
        }

        if (Input.GetKey(KeyCode.S))
        {
            NodeOne.transform.position -= transform.forward;
            NodeTwo.transform.position -= transform.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            NodeOne.transform.position -= transform.right;
            NodeTwo.transform.position -= transform.right;
        }

        if (Input.GetKey(KeyCode.D))
        {
            NodeOne.transform.position += transform.right;
            NodeTwo.transform.position += transform.right;
        }
    }

    // Debug only
    void OnGUI()
    {
        if (!DebugPlayer) return;

        Rect DebugRect = new Rect(0, 0, Screen.width, Screen.height);
        GUI.Label(DebugRect, "Center Point: " + CenterPoint.ToString() + ", Ship Point: " + Ship.transform.position.ToString() + "\n" +
                             "Node1 Point: " + NodeOne.transform.position.ToString() + ", Node2 Point: " + NodeTwo.position.ToString() + "\n" +
                             "Node Distance = " + NodeDistance.ToString()); 
    }
}
