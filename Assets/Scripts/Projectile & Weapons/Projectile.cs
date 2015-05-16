using UnityEngine;
using System.Collections;

/// <summary>
/// Projectile has its own enumeration which basically gives the subtype of this 
/// object. Projectiles are instantiated by ProjectilePools and "rented" by Weapon
/// objects attached to units and other objects. 
/// </summary>

public class Projectile : MonoBehaviour 
{

    // Settings
    public float SpeedMultiplier = 1.0f; // ................................ Multiplies the final speed value by this. Drastically causes fluctuation in speed.
    public TrailRenderer BodyTrail = null; // .............................. The trail left behind by the projectile as it flies through whatever.
    public ParticleSystem BodyPS = null; // ................................ The PS which acts as the main body of the projectile's appearance.
    public ParticleSystem[] Impacts; // .................................... The Impact PS's that get played ... upon impact. 
    public float LifeSeconds = 3; 

    // Private Member Data
    private Weapon Renter = null;
    private bool isActive = false;
    private BoxCollider BoundingBox = null;
    private Rigidbody RigBod = null;
    private float LifeTimer = 0;
    private float TrailTimer = 0; 

	// Use this for initialization
	void Start () 
    {
	    // Get sibling components & data
        BoundingBox = GetComponent<BoxCollider>();
        RigBod = GetComponent<Rigidbody>();
        TrailTimer = BodyTrail.time; 

        // Disable
        SetInactive();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    // Move the projectile
        if (isActive)
        {
            if (LifeTimer < LifeSeconds) LifeTimer += Time.deltaTime;
            else if (LifeTimer > LifeSeconds)
            {
                SetInactive();
                LifeTimer = 0; 
                return; 
            }

            RigBod.velocity = transform.forward * SpeedMultiplier; 
        }
        else RigBod.velocity = Vector3.zero; 
	}

    // SetActive(Weapon) :: Activates the projectile and fills it with the passed Weapon's data
    public void SetActive(Weapon weapon, Transform Barrel)
    {
        Renter = weapon;

        // Enabling
        BodyPS.enableEmission = true;
        BodyPS.Play();
        BoundingBox.enabled = true;
        BodyTrail.enabled = true;
        BodyTrail.time = TrailTimer; 
        isActive = true; 

        // Set Rotation / position of weapon
        transform.position = Barrel.transform.position;
        transform.rotation = Barrel.transform.rotation; 
    }

    // SetInactive() :: Disables the projectile and hides it, then clears data
    public void SetInactive()
    {
        Renter = null; 

        // Disabling
        BodyPS.enableEmission = false;
        BodyPS.Stop();
        BoundingBox.enabled = false;
        BodyTrail.time = -1;
        BodyTrail.enabled = false; 
        RigBod.velocity = Vector3.zero;
        isActive = false; 
    }

    // CheckActive() :: returns isAcitve
    public bool CheckActive() { return isActive; }
}
