using UnityEngine;
using UnityEngine.EventSystems;

public class Tower : MonoBehaviour
{
    public TowerType towerType = TowerType.None;
    public int TowerLevel = 0;
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
    public float bulletDamage = 1f;
    public float bulletRange = 0f;
    public float bulletSpeed = 0f;
    public float fireRate = 1f;
    private float fireCountdown = 1f;
    [Header("Laser")]
    public AnimationCurve damageOverTime;
    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;
    public Vector3 offset;
    private float timeOnEnemy;
    private Enemy lastEnemy;
    [Header("Spawner")]
    public GameObject spawnPrefab;
    public float spawnRate = 0.02f;
    public float spawnHealth = 50f;
    public float spawnDamage = 0f;
    [Tooltip("Range of the damage caused when the spawned dies")]
    public float spawnRange = 0.1f;
    public float spawnSpeed = 10f;
    private float spawnCountdown = 1f;
    //Script variables
    private bool isDestroyed = false;
    private TowerBlueprint towerBlueprint;
    private TowerInfo towerInfo;
    void Start()
    {
        towerInfo = TowerInfo.instance;
        noZone.AddComponent<NoZone>().parent = this; //Add nozone script to tower's no zone
        if (towerType != TowerType.None && towerType != TowerType.Spawner)
            InvokeRepeating("UpdateTarget", 0f, 0.5f); //Update the target of the tower
    }
    void Update()
    {
        if (GameManager.gameEnded || isDestroyed)
        {
            Destroy(gameObject); //Destroy tower if game has ended
            return;
        }
        if (towerType == TowerType.None)
        {
            return;
        }
        if (towerType == TowerType.Spawner)
        {
            spawnCountdown -= Time.deltaTime;
            if (spawnRate == 0) //Return if tower's spawn rate is 0
                return;
            if (spawnCountdown <= 0)
            {
                Spawn(); //Try to spawn
                spawnCountdown = 1 / spawnRate; //Set spawn rate countdown
            }
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
    void Spawn()
    {
        Transform spawnTrans = Waypoints.points[Waypoints.points.Length - 1];
        GameObject spawnObject = Instantiate(spawnPrefab, spawnTrans.position, spawnTrans.rotation); //Spawn
        PathBullet spawned = spawnObject.GetComponent<PathBullet>(); //Get spawned's script

        if (spawned != null)
        {
            spawned.SetInfo(spawnHealth, spawnDamage, spawnRange, spawnSpeed, spawnSpeed * 5); //Set info of spawned
        }
    }
    void Laser()
    {
        if (lastEnemy == targetEnemy)
        {
            timeOnEnemy += Time.deltaTime;
        } else
        {
            lastEnemy = targetEnemy;
            timeOnEnemy = 0;
        }
        targetEnemy.takeDamage(damageOverTime.Evaluate(timeOnEnemy) * Time.deltaTime);
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

        if (bullet != null && targetEnemy.health > 0)
        {
            bullet.SetInfo(targetEnemy, bulletDamage, bulletRange, bulletSpeed); //Set info of bullet
        }
    }
    public void WaveStart()
    {
        PlayerStats.ChangeEnergy(EnergyGenerated); //Change the amount of energy
        PlayerStats.ChangeHealth(HealthGenerated); //Change the amount of health
    }
    public void SetTowerBlueprint(TowerBlueprint blueprint)
    {
        towerBlueprint = blueprint;
    }
    public TowerBlueprint GetTowerBlueprint()
    {
        return towerBlueprint;
    }
    public void SetDestroyed(bool destroyed)
    {
        isDestroyed = destroyed;
    }
    public bool GetDestroyed()
    {
        return isDestroyed;
    }
    void UpdateTarget()
    {
        if (towerType == TowerType.Laser)
        {
            if (target && Vector3.SqrMagnitude(transform.position - target.position) < range * range)
            {
                if (targetEnemy.health > 0)
                {
                    return;
                }
            }
        }
        if (aimtype == AimType.Closest)
        {
            float closestDistance = range * range + 1; //initialise closest distance of enemy
            Transform nearestEnemy = null; //Initialise nearest enemy
            for (int i = 0; i < WaveSpawner.enemyList.Count; i++) //Loop through every enemy
            {
                Transform element = WaveSpawner.enemyList[i];
                if (element != null)
                {
                    float distance = Vector3.SqrMagnitude(transform.position - element.position); //Get distance of enemy
                    if (distance < closestDistance && distance <= range * range) //if distance is less than closest distance of enemy
                    {
                        closestDistance = distance; //Set new closest distance
                        nearestEnemy = element; //Set the enemy which is closest
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
            float furthestDistance = float.MaxValue; //initialise furthest distance of enemy
            Transform furthestEnemy = null; //Initialise furthest enemy
            Enemy furthestEnemyScript = null; //Initialise furthest enemy script
            for (int i = 0; i < WaveSpawner.enemyList.Count; i++) //Loop through every enemy
            {
                Transform element = WaveSpawner.enemyList[i];
                if (element != null)
                {
                    float distance = Vector3.SqrMagnitude(transform.position - element.position); //Get distance of enemy
                    if (distance <= range * range) //if distance is in range
                    {
                        try
                        {
                            Enemy enemy = element.GetComponent<Enemy>(); // Get the enemy script of the enemy
                            if (enemy.pos < furthestDistance && enemy.health > 0) // If enemy is closer to the end of track
                            {
                                furthestDistance = enemy.pos; // Set new pos
                                furthestEnemy = element; // Set new most health enemy
                                furthestEnemyScript = enemy;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            if (furthestEnemy != null)
            {
                target = furthestEnemy; //If there is an enemy in range, set it as target
                targetEnemy = furthestEnemyScript;
            }
            else
            {
                target = null; //Do nothing
            }
        } else if (aimtype == AimType.MostHealth)
        {
            float mostHealth = 0; //initialise health of enemy
            Transform mostHealthEnemy = null; //Initialise the most health enemy
            Enemy mostHealthEnemyScript = null; //Initialise the most health enemy script
            for (int i = 0; i < WaveSpawner.enemyList.Count; i++) //Loop through every enemy
            {
                Transform element = WaveSpawner.enemyList[i];
                if (element != null)
                {
                    float distance = Vector3.SqrMagnitude(transform.position - element.position); //Get distance of enemy
                    if (distance <= range * range) //if distance is in range
                    {
                        try
                        {
                            Enemy enemy = element.GetComponent<Enemy>(); // Get the enemy script of the enemy
                            if (enemy.health > mostHealth) // If enemies health is higher than the ones before
                            {
                                mostHealth = enemy.health; // Set new most health
                                mostHealthEnemy = element; // Set new most health enemy
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
        else if (aimtype == AimType.LeastHealth)
        {
            float leastHealth = float.MaxValue; //initialise health of enemy
            Transform leastHealthEnemy = null; //Initialise the least health enemy
            Enemy leastHealthEnemyScript = null; //Initialise the least health enemy script
            for (int i = 0; i < WaveSpawner.enemyList.Count; i++) //Loop through every enemy
            {
                Transform element = WaveSpawner.enemyList[i];
                if (WaveSpawner.enemyList[i] != null)
                {
                    float distance = Vector3.SqrMagnitude(transform.position - element.position); //Get distance of enemy
                    if (distance <= range * range) //if distance is in range
                    {
                        try
                        {
                            Enemy enemy = element.GetComponent<Enemy>(); // Get the enemy script of the enemy
                            if (enemy.health <= leastHealth && enemy.health > 0) // If enemies health is lower than the ones before
                            {
                                leastHealth = enemy.health; // Set new least health
                                leastHealthEnemy = element; // Set new least health enemy
                                leastHealthEnemyScript = enemy;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            if (leastHealthEnemy != null)
            {
                target = leastHealthEnemy; //If there is an enemy in range, set it as target
                targetEnemy = leastHealthEnemyScript;
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
            if (!towerInfo.gameObject.activeSelf || towerInfo.transform.position != transform.position)
            {
                towerInfo.setTower(this); // Show tower stats if it is selected and wasn't previously selected
            }
            else
            {
                towerInfo.setTower(null); // Hide tower if it is already selected
            }
            return;
        } else if (mouseButton == 1) {}
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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; //Stops the tower from being selected if there is UI in front of the tower
        }
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
    Laser,
    Spawner
}
public enum AimType
{
    Closest,
    Furthest,
    MostHealth,
    LeastHealth,
}