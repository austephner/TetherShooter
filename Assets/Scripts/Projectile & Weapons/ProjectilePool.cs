using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Acts as a resource pool for objects that need to fire projectiles. Objects
/// will "rent" a projectile from this pool, which'll get values based on the
/// renting object. It'll reset once the projectile has met a destination
/// point. 
/// </summary>

public class ProjectilePool : MonoBehaviour 
{

    // Settings
    public Projectile ProjectilePrefab = null;
    public int PoolSize = 20; // ........................................... This is how many objects will be instantiated when this pool is created.

    // Private Member Data
    private List<Projectile> Projectiles = new List<Projectile>(); 

	// Use this for initialization
	void Start () 
    {
        // Instantiate the pool
        for (int i = 0; i < PoolSize; i++)
        {
            Projectiles.Add(Instantiate(ProjectilePrefab));
            Debug.Log("ProjectilePool instantiating " + i.ToString() + " of " + PoolSize.ToString()); 
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    // RentProjectile(weapon) :: Returns a useable projectile
    public Projectile RentProjectile()
    {
        // Sift through pool for active projectile
        foreach (Projectile proj in Projectiles) if (!proj.CheckActive()) return proj; 

        // Went through entire pool and all were actve ... return null
        return null; 
    }
}
