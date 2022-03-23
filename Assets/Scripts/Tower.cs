using System;
using UnityEngine;
public class Tower : MonoBehaviour
{
    public TowerType towerType = TowerType.None;
    protected Transform target;
    protected Enemy targetEnemy;
    [Header("General")]
    [Tooltip("How much energy the tower produces each wave")]
    public int EnergyGenerated;
    [Tooltip("How much health the tower produces each wave")]
    public int HealthGenerated;
    [Header("Attributes")]
    public float range = 15f;
    public float turnSpeed = 0.5f;
    public AimType aimtype = AimType.Closest;
    [Header("Setup Fields")]
    public Transform rotationPoint;
    public Transform firePoint;
    public GameObject noZone;
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public int bulletDamage = 1;
    public float fireRate = 1f;
    private float fireCountdown = 0f;
    [Header("Laser")]
    public float damageOverTime = 1;
    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;
    public Vector3 offset;
    //Script variables
    private bool isDestroyed = false;
    private float clickTime = 1f;
    private int towerCost;
    void Start()
    {
        noZone.AddComponent<NoZone>().parent = this; //Add nozone script to tower's no zone
        if (towerType != TowerType.None)
            InvokeRepeating("UpdateTarget", 0f, 0.5f); //Update the target of the tower
    }
    void Update()
    {
        clickTime += Time.deltaTime;
        if (GameManager.gameEnded || isDestroyed)
        {
            Destroy(gameObject); //Destroy tower if game has ended
            return;
        }
        fireCountdown -= Time.deltaTime; //Decrease fire countdown
        if (target == null) //If there is no target
        {
            if (towerType == TowerType.Laser) //Hide line renderer if there is no target
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                    impactEffect.Stop();
                    impactLight.enabled = false;
                }
            }
            return;
        }
        Quaternion lookRotation = RotateToTarget();
        if (towerType == TowerType.Laser) //Laser
        {
            if (Quaternion.Angle(rotationPoint.rotation, lookRotation) < 5)
            {
                Laser();
            } else
            {
                if (lineRenderer.enabled) //Hide line renderer if it isn't directly facing target
                {
                    lineRenderer.enabled = false;
                    impactEffect.Stop();
                    impactLight.enabled = false;
                }
            }
        }
        else if (towerType == TowerType.Bullet) //Bullet
        {
            if (fireRate == 0) //Return if tower's fire rate is 0
                return;
            if (fireCountdown <= 0 && Quaternion.Angle(rotationPoint.rotation, lookRotation) < 10)
            {
                Shoot(); //Try to shoot at the enemy
                fireCountdown = 1 / fireRate; //Set fire rate countdown
            }
        }
    }
    Quaternion RotateToTarget()
    {
        Vector3 dir = target.position - transform.position; //Get direction to the enemy
        dir.y = 0; //Set the y rotation to 0, to prevent the tower from looking up and down
        Quaternion lookRotation = Quaternion.LookRotation(dir.normalized); //Get quaternion of direction
        rotationPoint.rotation = Quaternion.SlerpUnclamped(rotationPoint.rotation, lookRotation, turnSpeed * Time.deltaTime); //Rotate tower to look at enemy
        return lookRotation; //Return lookrotation
    }
    void Laser()
    {
        targetEnemy.takeDamage(damageOverTime * Time.deltaTime);
        if (!lineRenderer.enabled) //Show line renderer if it is hidden
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }
        lineRenderer.SetPosition(0, firePoint.position); //Set start to the fire point
        lineRenderer.SetPosition(1, target.position + offset); //Set end to target
        Vector3 dir = firePoint.position - target.position - offset;
        impactEffect.transform.rotation = Quaternion.LookRotation(dir);
        impactEffect.transform.position = target.position + offset + dir.normalized * 0.5f;
    }
    void Shoot()
    {
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); //Create a bullet
        Bullet bullet = bulletObject.GetComponent<Bullet>(); //Get bullet's script

        if (bullet != null)
        {
            bullet.SetTarget(targetEnemy); //Set target of bullet
            bullet.SetDamage(bulletDamage); //Set damage of bullet
        }
    }
    public void WaveStart()
    {
        PlayerStats.ChangeEnergy(EnergyGenerated); //Change the amount of energy
        PlayerStats.ChangeHealth(HealthGenerated); //Change the amount of health
    }
    public void SetTowerCost(int cost)
    {
        towerCost = cost;
    }
    void UpdateTarget()
    {
        if (aimtype == AimType.Closest)
        {
            float closestDistance = range * range + 1; //initialise closest distance of enemy
            Transform nearestEnemy = null; //Initialise nearest enemy
            for (int i = 0; i < WaveSpawner.enemyList.Count; i++) //Loop through every enemy
            {
                if (WaveSpawner.enemyList[i] != null)
                {
                    float distance = Vector3.SqrMagnitude(transform.position - WaveSpawner.enemyList[i].position); //Get distance of enemy
                    if (distance < closestDistance && distance <= range * range) //if distance is less than closest distance of enemy
                    {
                        closestDistance = distance; //Set new closest distance
                        nearestEnemy = WaveSpawner.enemyList[i]; //Set the enemy which is closest
                    }
                }
            }
            if (nearestEnemy != null)
            {
                target = nearestEnemy; //If there is an enemy in range, set it as target
                targetEnemy = nearestEnemy.GetComponent<Enemy>();
            }
            else
            {
                target = null; //Do nothing
            }
        } else if (aimtype == AimType.Furthest)
        {
            float furthestDistance = -1; //initialise furthest distance of enemy
            Transform furthestEnemy = null; //Initialise furthest enemy
            for (int i = 0; i < WaveSpawner.enemyList.Count; i++) //Loop through every enemy
            {
                if (WaveSpawner.enemyList[i] != null)
                {
                    float distance = Vector3.SqrMagnitude(transform.position - WaveSpawner.enemyList[i].position); //Get distance of enemy
                    if (distance > furthestDistance && distance <= range * range) //if distance is more than furthest distance of enemy
                    {
                        furthestDistance = distance; //Set new furthest distance
                        furthestEnemy = WaveSpawner.enemyList[i]; //Set the enemy which is furthest
                    }
                }
            }
            if (furthestEnemy != null)
            {
                target = furthestEnemy; //If there is an enemy in range, set it as target
                targetEnemy = furthestEnemy.GetComponent<Enemy>();
            }
            else
            {
                target = null; //Do nothing
            }
        } else if (aimtype == AimType.MostHealth)
        {
            float mostHealth = -1; //initialise health of enemy
            Transform mostHealthEnemy = null; //Initialise the most health enemy
            Enemy mostHealthEnemyScript = null; //Initialise the most health enemy script
            for (int i = 0; i < WaveSpawner.enemyList.Count; i++) //Loop through every enemy
            {
                if (WaveSpawner.enemyList[i] != null)
                {
                    float distance = Vector3.SqrMagnitude(transform.position - WaveSpawner.enemyList[i].position); //Get distance of enemy
                    if (distance <= range * range) //if distance is in range
                    {
                        try
                        {
                            Enemy enemy = WaveSpawner.enemyList[i].GetComponent<Enemy>(); // Get the enemy script of the enemy
                            if (enemy.health > mostHealth) // If enemies health is higher than the ones before
                            {
                                mostHealth = enemy.health; // Set new most health
                                mostHealthEnemy = WaveSpawner.enemyList[i]; // Set new most health enemy
                                mostHealthEnemyScript = enemy;
                            }
                        } catch
                        {

                        }
                    }
                }
            }
            if (mostHealthEnemy != null)
            {
                target = mostHealthEnemy; //If there is an enemy in range, set it as target
                targetEnemy = mostHealthEnemyScript;
            }
            else
            {
                target = null; //Do nothing
            }
        }
    }
    public void selected(int mouseButton)
    {
        if (mouseButton == 0)
        {
            print("WIP");
            return;
        } else if (mouseButton == 1)
        {
            if (!isDestroyed && clickTime <= 0.5f)
            {
                PlayerStats.towers--; //Reduce number of towers
                PlayerStats.ChangeEnergy(towerCost / 2); //Give back half of the energy required to build the tower
                isDestroyed = true; //Set the tower to be destroyed
                return;
            }
            clickTime = 0f;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range); //Draw a green wire sphere to show range of tower
    }
}
public class NoZone : MonoBehaviour
{
    public Tower parent;
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            parent.selected(0); //Call the tower's selected function
        }
        if (Input.GetMouseButtonDown(1))
        {
            parent.selected(1); //Call the tower's selected function
        }
    }
}

public enum TowerType
{
    None,
    Bullet,
    Laser
}
public enum AimType
{
    Closest,
    Furthest,
    MostHealth,
}