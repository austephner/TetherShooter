using UnityEngine;
using System.Collections;

/// <summary>
/// Script that enables the object it's attached to to fire projectiles
/// from a pre-filled projectile pool. 
/// </summary>

public class Weapon : MonoBehaviour 
{
    internal enum WeaponStatus
    {
        FIRING, // ....... Occurs after/when a projectile has been shot
        RELOADING, // .... Whenever a reload is necessary (not implemented)
        COOLDOWN, // ..... If the weapon has a cool-down attribute, this is called during overheat periods
        IDLE // .......... Weapon din do nuttin
    }

    // Settings
    public ProjectilePool PoolResource = null; // .......................... Where the unit rents projectiles from
    public Transform[] Barrels; // ......................................... The locations a unit can fire projectiles from
    public bool AlternateBarrels = true; // ................................ Alternates between the given barrels while firing. Turning this off causes every barrel to fire at the same time.
    public float SecondsBetweenShots = .05f; // ............................ The amount of time that is required to pass before firing another projectile. 

    // Data
    private WeaponStatus status = WeaponStatus.IDLE;
    private float ShotTimer = 0f;
    private int BarrelIndex = 0; 

	// Use this for initialization
	void Start () 
	{
        if (PoolResource == null)
        {
            Debug.LogError("Weapon doesn't have a pool resource attached to it. Disabling weapon.");
            this.enabled = false;
            return; 
        }

        if (Barrels.Length == 0)
        {
            Debug.LogError("Weapon doesn't have any barrels assigned to it. Disabling weapon.");
            this.enabled = false;
            return; 
        }
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (status == WeaponStatus.FIRING)
        {
            ShotTimer += Time.deltaTime;

            if (ShotTimer > SecondsBetweenShots)
            {
                ShotTimer = 0;
                status = WeaponStatus.IDLE; 
            }
        }
	}

    // Fire() :: activates a rented projectile from the designated barrel 
    public void Fire()
    {
        // Prevent from firing outside of the ROF
        if (status != WeaponStatus.IDLE) return;

        // Rent projectile and check if it's not null && alternating barrels is enabled
        if (AlternateBarrels)
        {
            Projectile projectile = PoolResource.RentProjectile();
            if (projectile != null)
            {
                // Determine active barrel
                Transform ActiveBarrel = Barrels[BarrelIndex];

                // Increment barrel index
                BarrelIndex += 1;
                if (BarrelIndex == Barrels.Length) BarrelIndex = 0;

                projectile.SetActive(this, ActiveBarrel);
                status = WeaponStatus.FIRING;
            }
            else Debug.LogError("Pool resource returned null ... may be out of useable projectiles.");
        }

        // otherwise every barrel gets a projectile
        else
        {
            foreach (Transform barrel in Barrels)
            {
                Projectile projectile = PoolResource.RentProjectile();
                if (projectile) projectile.SetActive(this, barrel); 
                else Debug.LogError("Pool resource returned null ... may be out of useable projectiles.");
            }
            
            status = WeaponStatus.FIRING;
        }
    }
}
