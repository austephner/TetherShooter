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

        public ControllerSettings() { /* empty constructor (on purpose) */ }
        public void SetControls(PlayerNumber pn)
        {
            string JoystickName = "Joystick";

            if (pn == PlayerNumber.PlayerOne) JoystickName += "1";
            else JoystickName += "2"; 

            LeftJoystickHorizontalNodeAxis = JoystickName + "_Horizontal_Left";
            LeftJoystickVerticalNodeAxis = JoystickName + "_Vertical_Left";

            RightJoystickHorizontalNodeAxis = JoystickName + "_Horizontal_Right";
            RightJoystickVerticalNodeAxis = JoystickName + "_Vertical_Right";

            FireJoystickWeaponButton = JoystickName + "_Fire1";
        }
    }

    // Settings
    public bool DebugPlayer = true; 
    public bool UseKeyboard = false; 
    public bool UseJoystick = true;
    public Weapon PlayerWeapon = null;
    public Transform NodeLeft, NodeRight, Ship, ElasticPosition; // ....... Components of the ship
    public float MaxNodeSpeed = 10f; // ................................... How fast a node can move
    public float NodeSpeedMultiplier = 1f; // ............................. How quickly nodes accelerate... 1 has no effect
    public float ElasticityMultiplier = .5f; // ........................... The reactionary stretchiness of the nodes' distance from one another
    public float ElasticityMaximum = .8f; // .............................. The highest percentage before max elasticity is achieved
    public float ElasticityMinimum = .2f; // .............................. The opposite of the above
    public float BaseShipSpeed = .01f; // ................................. How quickly the player's ship slerp's towards the nodes' average
    public float MaxNodeDistance = 900; // ................................ Max distance between the two nodes before horizontal message
    public float MinNodeDistance = 50; 
    public Vector3 PositionalOffset = new Vector3(0, 0, 0); // ............ How far away from the center average the ship will rest at


    // Private
    private Player PlayerObject = null;
    private Vector3 CenterPoint = new Vector3(0,0,0);
    private float NodeDistance = 0f;
    private float ElasticityModifier = 0f;
    private ControllerSettings CurrentControls = new ControllerSettings();
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

        // Other Debug stuff
        if (DebugPlayer)
        {
            Debug.Log(PlayerObject.IdentificationNumber.ToString()); 
        }
	}

    // CalculationUpdate() :: Does updates to all variables requiring calculations
    void CalculationUpdate()
    {
        // Get node distance
        NodeDistance = Vector3.Distance(NodeLeft.position, NodeRight.position);

        // Calculate the ElasticityModifier (the closer the NodeDistance is to max, the closer this number is to 1)
        // Then round off the number a bit to make the controls seem a little more substancial
        ElasticityModifier = NodeDistance / MaxNodeDistance;
        if (ElasticityModifier < ElasticityMinimum) ElasticityModifier = 0;
        if (ElasticityModifier > ElasticityMaximum) ElasticityModifier = 1;

        // Get Center point & adjust y zero
        CenterPoint = (NodeLeft.position + NodeRight.position) / 2;
        CenterPoint = new Vector3(CenterPoint.x, 0, CenterPoint.z);

        // Make the elastic position rapidly fly towards the centerpoint, giving its position to the ship's with a slerp
        ElasticPosition.LookAt(CenterPoint);
        ElasticPosition.GetComponent<Rigidbody>().AddForce(ElasticPosition.transform.forward * (ElasticityMultiplier * Vector3.Distance(ElasticPosition.position, CenterPoint) * 2), ForceMode.Acceleration);
        //ElasticPosition.GetComponent<Rigidbody>().AddForce(ElasticPosition.transform.forward, ForceMode.Acceleration); 
        Ship.position = Vector3.Slerp(Ship.position, ElasticPosition.position, BaseShipSpeed + (ElasticityModifier * ElasticityMultiplier));
        Ship.position = new Vector3(Ship.position.x, 0, Ship.position.z); // for maintaining a 0 y value
    }

    // KeyboardControls() :: Do keyboard control update
    void KeyboardControls()
    {
        Rigidbody nrb_left = NodeLeft.GetComponent<Rigidbody>();
        Rigidbody nrb_right = NodeRight.GetComponent<Rigidbody>();
        
        // Movement (left node)
        if (nrb_left.velocity.magnitude < MaxNodeSpeed)
        {
            // Left Node Vertical
            if (Input.GetAxis(CurrentControls.LeftKeyboardVerticalNodeAxis) != 0)
            {
                nrb_left.velocity += new Vector3(
                    0f,
                    0f,
                    Input.GetAxis(CurrentControls.LeftKeyboardVerticalNodeAxis) * NodeSpeedMultiplier);
            }

            // Left Node Horizontal
            if (Input.GetAxis(CurrentControls.LeftKeyboardHorizontalNodeAxis) != 0)
            {
                nrb_left.velocity += new Vector3(
                    Input.GetAxis(CurrentControls.LeftKeyboardHorizontalNodeAxis) * NodeSpeedMultiplier,
                    0f,
                    0f);
            }
        }

        // Movement (right node)
        if (nrb_right.velocity.magnitude < MaxNodeSpeed)
        {
            // Right Node Vertical
            if (Input.GetAxis(CurrentControls.RightKeyboardVerticalNodeAxis) != 0)
            {
                nrb_right.velocity += new Vector3(
                    0f,
                    0f,
                    Input.GetAxis(CurrentControls.RightKeyboardVerticalNodeAxis) * NodeSpeedMultiplier);
            }

            // Right Node Horizontal
            if (Input.GetAxis(CurrentControls.RightKeyboardHorizontalNodeAxis) != 0)
            {
                nrb_right.velocity += new Vector3(
                    Input.GetAxis(CurrentControls.RightKeyboardHorizontalNodeAxis) * NodeSpeedMultiplier,
                    0f,
                    0f);
            }
        }

        // Firing
        if (Input.GetButton(CurrentControls.FireKeyboardWeaponButton))
        {
            PlayerWeapon.Fire(); 
        }

    }

    // JoystickControls() :: Do joystick / handheld controller controls. 
    void JoystickControls()
    {
        Rigidbody nrb_left = NodeLeft.GetComponent<Rigidbody>();
        Rigidbody nrb_right = NodeRight.GetComponent<Rigidbody>();
        bool DoLeftHorizontalMovement = true, DoLeftVerticalMovement = true, DoRightMovement = true; 

        // Movement (left node)
        if (nrb_left.velocity.magnitude < MaxNodeSpeed)
        {
            // Left Node Vertical
            if (Input.GetAxis(CurrentControls.LeftJoystickVerticalNodeAxis) != 0)
            {
                //// Check vertical constraints (moving downwards and upwards)
                //if (Input.GetAxis(CurrentControls.LeftJoystickVerticalNodeAxis) < 0 && NodeDistance < MinNodeDistance ||
                //    Input.GetAxis(CurrentControls.LeftJoystickVerticalNodeAxis) > 0 && NodeDistance > MaxNodeDistance)
                //    DoLeftVerticalMovement = false; 

                // Move node vertical
                if (DoLeftVerticalMovement)
                nrb_left.velocity += new Vector3(
                    0f,
                    0f,
                    Input.GetAxis(CurrentControls.LeftJoystickVerticalNodeAxis) * NodeSpeedMultiplier);
            }

            // Left Node Horizontal
            if (Input.GetAxis(CurrentControls.LeftJoystickHorizontalNodeAxis) != 0)
            {
                //// Check vertical constraints (moving downwards and upwards)
                //if (Input.GetAxis(CurrentControls.LeftJoystickHorizontalNodeAxis) < 0 && NodeDistance < MinNodeDistance ||
                //    Input.GetAxis(CurrentControls.LeftJoystickHorizontalNodeAxis) > 0 && NodeDistance > MaxNodeDistance)
                //    DoLeftHorizontalMovement = false; 

                // Move Node Horizontal
                if (DoLeftHorizontalMovement)
                nrb_left.velocity += new Vector3(
                    Input.GetAxis(CurrentControls.LeftJoystickHorizontalNodeAxis) * NodeSpeedMultiplier,
                    0f,
                    0f);
            }
        }

        // Movement (right node)
        if (nrb_right.velocity.magnitude < MaxNodeSpeed)
        {
            // Right Node Vertical
            if (Input.GetAxis(CurrentControls.RightJoystickVerticalNodeAxis) != 0)
            {
                nrb_right.velocity += new Vector3(
                    0f,
                    0f,
                    Input.GetAxis(CurrentControls.RightJoystickVerticalNodeAxis) * NodeSpeedMultiplier);
            }

            // Right Node Horizontal
            if (Input.GetAxis(CurrentControls.RightJoystickHorizontalNodeAxis) != 0 && DoRightMovement)
            {
                nrb_right.velocity += new Vector3(
                    Input.GetAxis(CurrentControls.RightJoystickHorizontalNodeAxis) * NodeSpeedMultiplier,
                    0f,
                    0f);
            }
        }

        // Fire1
        if (Input.GetButton(CurrentControls.FireJoystickWeaponButton))
        {
            PlayerWeapon.Fire(); 
        }

    }

    // CheckMovementConstraints(leftNodeAxis, rightNodeAxis) :: returns true if the node can move in the direction without conflicting any of the rules (min/max distances)
    bool CheckMovementConstraints(float leftNodeAxis, float rightNodeAxis)
    {
        // Check left node

        return true; 
    }

    // TestInput(enabled) :: Does testing input, won't as long as passed a false value. Subject to constant change
    public void TestInput(bool DoTests)
    {
        if (!DoTests) return;

        if (Input.GetKey(KeyCode.W))
        {
            NodeLeft.transform.position += transform.forward;
            NodeRight.transform.position += transform.forward; 
        }

        if (Input.GetKey(KeyCode.S))
        {
            NodeLeft.transform.position -= transform.forward;
            NodeRight.transform.position -= transform.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            NodeLeft.transform.position -= transform.right;
            NodeRight.transform.position -= transform.right;
        }

        if (Input.GetKey(KeyCode.D))
        {
            NodeLeft.transform.position += transform.right;
            NodeRight.transform.position += transform.right;
        }
    }

    // Debug only
    void OnGUI()
    {
        if (!DebugPlayer) return;

        Rect DebugRect = new Rect(0, 0, Screen.width, Screen.height);
        GUI.Label(DebugRect, "Center Point: " + CenterPoint.ToString() + ", Ship Point: " + Ship.transform.position.ToString() + "\n" +
                             "Node1 Point: " + NodeLeft.transform.position.ToString() + ", Node2 Point: " + NodeRight.position.ToString() + "\n" +
                             "Node Distance = " + NodeDistance.ToString() + "\n" +
                             "LeftJoystickVerticalNodeAxis: " + Input.GetAxis(CurrentControls.LeftJoystickVerticalNodeAxis).ToString() + "\n" +
                             "LeftJoystickHorizontalNodeAxis: " + Input.GetAxis(CurrentControls.LeftJoystickHorizontalNodeAxis).ToString() + "\n" +
                             "RightJoystickVerticalNodeAxis: " + Input.GetAxis(CurrentControls.RightJoystickVerticalNodeAxis).ToString() + "\n" +
                             "RightJoystickHorizontalNodeAxis: " + Input.GetAxis(CurrentControls.RightJoystickHorizontalNodeAxis).ToString() + "\n" +
                             "(LN) Magnitude of " + NodeLeft.GetComponent<Rigidbody>().velocity.magnitude.ToString() + "\n" +
                             "(RN) Magnitude of " + NodeRight.GetComponent<Rigidbody>().velocity.magnitude.ToString()); 
    }
}
