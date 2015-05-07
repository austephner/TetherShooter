using UnityEngine;
using System.Collections;

/// <summary>
/// This is the controller class that handles user input and moves
/// the ship / tether nodes. There's also an internal class which 
/// is used to determine the axes and buttons for the controller ... 
/// just in case there's ever secondary controllers added.
/// 
/// Austin's Wishlist (things I'd like to do but haven't figured out yet): 
/// 1) Make movement more elastic ... right now the ship just approaches the nodes' average location, there 
///     is no "overshot", which is what I would imagine happening when you pull something by two ends and
///     have excess force. I don't think physically an object would approach something then slow down as it
///     got there, it'd go as fast as it could, then struggle to get back into the center. I'll figure it
///     out eventually. 
/// 
/// </summary>

public class PlayerController : MonoBehaviour 
{
    // Internals
    internal class ControllerSettings
    {
        // Keyboard
        public string LeftKeyboardHorizontalNodeAxis = "Keyboard_Horizontal_Left", LeftKeyboardVerticalNodeAxis = "Keyboard_Vertical_Left",
                      RightKeyboardHorizontalNodeAxis = "Keyboard_Horizontal_Right", RightKeyboardVerticalNodeAxis = "Keyboard_Vertical_Right",
                      PauseKeyboardButton = "Keyboard_Pause",
                      FireKeyboardWeaponButton = "Keyboard_Fire1";

        // Joystick
        public string LeftJoystickHorizontalNodeAxis = "", LeftJoystickVerticalNodeAxis = "",
                      RightJoystickHorizontalNodeAxis = "", RightJoystickVerticalNodeAxis = "",
                      PauseJoystickButton = "", 
                      FireJoystickWeaponButton = "";

        public void SetControls(PlayerNumber pn)
        {
            string JoystickName = "Joystick";

            if (pn == PlayerNumber.PlayerOne) JoystickName += "1";
            else JoystickName += "2"; 

            LeftJoystickHorizontalNodeAxis = JoystickName + "_Horizontal_Left";
            LeftJoystickVerticalNodeAxis = JoystickName + "_Vertical_Left";

            RightJoystickHorizontalNodeAxis = JoystickName + "_Horizontal_Right";
            RightJoystickVerticalNodeAxis = JoystickName + "_Vertical_Right"; 
        }
    }

    // Settings
    public bool DebugPlayer = true; 
    public bool UseKeyboard = false; 
    public bool UseJoystick = true;
    public Player PlayerObject = null;
    public Transform NodeOne, NodeTwo, Ship; // ........................... Components of the ship
    public float NodeSpeedMultiplier = 1f; // ............................. How fast the nodes can move with player input
    public float ElasticityMultiplier = .5f; // ........................... The reactionary stretchiness of the nodes' distance from one another
    public float ElasticityMaximum = .8f; // .............................. The highest percentage before max elasticity is achieved
    public float ElasticityMinimum = .2f; // .............................. The opposite of the above
    public float BaseShipSpeed = .01f; // ................................. How quickly the player's ship slerp's towards the nodes' average
    public float MaxNodeDistance = 900; // ................................ Max distance between the two nodes before horizontal message
    public Vector3 PositionalOffset = new Vector3(0, 0, 0); // ............ How far away from the center average the ship will rest at

    // Private
    private Vector3 CenterPoint = new Vector3(0,0,0);
    private float NodeDistance = 0f;
    private float ElasticityModifier = 0f;
    private ControllerSettings CurrentControls;
    private GameManager GM = null; 

	// Use this for initialization
	void Start () 
    {
	    // Check if the player object is null. 
        // If so, look for a player object attached to the parent of this object. 
        if (!PlayerObject)
        {
            // Try to get game object
            if (GetComponent<Player>()) 
            {
                PlayerObject = GetComponent<Player>();

                // Set controls based on player number
                CurrentControls.SetControls(PlayerObject.IdentificationNumber); 
            }

            // Failed to find a player game object
            else Debug.LogError("WARNING ... Failed to find a player object for this controller object."); 
        }

        // Try and find the game manager object
        if (!GM)
        {
            if (FindObjectOfType<GameManager>()) GM = FindObjectOfType<GameManager>();
            else Debug.LogError("WARNING ... Failed to find a GameManager object for this controller object."); 
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        // Calculation Update
        CalculationUpdate(); 

        // Keyboard Movements
        if (UseKeyboard) KeyboardControls(); 

        // Joystick Movements
        if (UseJoystick) JoystickControls(); 

        // Call TestInput
        TestInput(DebugPlayer); 
	}

    // CalculationUpdate() :: Does updates to all variables requiring calculations
    void CalculationUpdate()
    {
        // Get node distance
        NodeDistance = Vector3.Distance(NodeOne.position, NodeTwo.position);

        // Calculate the ElasticityModifier (the closer the NodeDistance is to max, the closer this number is to 1)
        // Then round off the number a bit to make the controls seem a little more substancial
        ElasticityModifier = NodeDistance / MaxNodeDistance;
        if (ElasticityModifier < ElasticityMinimum) ElasticityModifier = 0;
        if (ElasticityModifier > ElasticityMaximum) ElasticityModifier = 1;

        // Get Center point & adjust y zero
        CenterPoint = (NodeOne.position + NodeTwo.position) / 2;
        CenterPoint = new Vector3(CenterPoint.x, 0, CenterPoint.z);

        // Slerp ship towards center point & adjust y zero
        Ship.position = Vector3.Slerp(Ship.position, CenterPoint, BaseShipSpeed + (ElasticityModifier * ElasticityMultiplier));
        Ship.position = new Vector3(Ship.position.x, 0, Ship.position.z); 
    }

    // KeyboardControls() :: Do keyboard control update
    void KeyboardControls()
    {
        // Left Node Vertical 
        if (Input.GetAxis(CurrentControls.LeftKeyboardVerticalNodeAxis) != 0)
        {

        }

        // Left Node Horizontal
        if (Input.GetAxis(CurrentControls.LeftKeyboardHorizontalNodeAxis) != 0)
        {

        }

        // Right Node Vertical
        if (Input.GetAxis(CurrentControls.RightKeyboardVerticalNodeAxis) != 0)
        {

        }

        // Right Node Horizontal
        if (Input.GetAxis(CurrentControls.RightKeyboardHorizontalNodeAxis) != 0)
        {

        }
    }

    // JoystickControls() :: Do joystick / handheld controller controls. 
    void JoystickControls()
    {
        // Left Node Vertical 
        if (Input.GetAxis(CurrentControls.LeftJoystickVerticalNodeAxis) != 0)
        {

        }

        // Left Node Horizontal
        if (Input.GetAxis(CurrentControls.LeftJoystickHorizontalNodeAxis) != 0)
        {

        }

        // Right Node Vertical
        if (Input.GetAxis(CurrentControls.RightJoystickVerticalNodeAxis) != 0)
        {

        }

        // Right Node Horizontal
        if (Input.GetAxis(CurrentControls.RightJoystickHorizontalNodeAxis) != 0)
        {

        }

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
