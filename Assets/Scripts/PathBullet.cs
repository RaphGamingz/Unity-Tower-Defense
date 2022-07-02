using UnityEngine;
public class PathBullet : MonoBehaviour
{
    private Transform target;
    private int waypointIndex = 0;
    public GameObject effect;
    private float speed;
    private float turnSpeed;
    private float range;
    private float health;
    private float deathDamage;
    void Start()
    {
        waypointIndex = Waypoints.points.Length - 2;
        target = Waypoints.points[waypointIndex];
    }
    public void SetInfo(float _health, float damage, float _range, float _speed, float _turnSpeed)
    {
        health = _health;
        deathDamage = damage;
        range = _range;
        speed = _speed;
        turnSpeed = _turnSpeed;
    }
    void Update()
    {
        if (GameManager.gameEnded)
        {
            Destroy(gameObject);
            return;
        }
        float distance = speed * Time.deltaTime; //Distance it can move
        for (int i = 0; i < WaveSpawner.enemyList.Count; i++)
        {
            Transform enemyT = WaveSpawner.enemyList[i];
            if (enemyT)
            {
                if ((enemyT.position - transform.position).sqrMagnitude < distance * distance + 0.01f * speed)
                {
                    try
                    {
                        Enemy enemy = WaveSpawner.enemyList[i].GetComponent<Enemy>();
                        if (enemy.health > 0)
                        {
                            if (health > enemy.health)
                            {
                                health -= enemy.health;
                                enemy.takeDamage(enemy.health);
                            }
                            else
                            {
                                enemy.takeDamage(health);
                                health = 0;
                                if (deathDamage > 0 && range > 0)
                                {
                                    Explode();
                                }
                                Destroy(Instantiate(effect, transform.position, transform.rotation), 7f); //Create particle effect and destroy after 7 seconds
                                Destroy(gameObject);
                            }
                        }
                    }
                    catch (System.Exception) { }
                }
            }
        }
        Vector3 dir = target.position - transform.position; //Get direction to move at
        dir.y = 0; //Don't move up or down
        transform.Translate(dir.normalized * distance, Space.World); //Move to that direction

        if (dir.normalized != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dir.normalized); //Get direction to look at
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, lookRotation, turnSpeed * Time.deltaTime); //Look at direction, slerping it
        }
        if ((transform.position - target.position).sqrMagnitude <= 0.01f * speed)
        {
            SetNextWaypoint(); //Set it's new target when this target is reached
        }
    }
    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range); //Get overlapping colliders in the spawned range
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            try
            {
                if (collider.transform.parent.CompareTag("Enemy")) //If collider is an enemy
                {
                    Damage(collider.transform.parent); //Hurt the enemy
                }
            }
            catch (System.Exception) {}
        }
    }
    void Damage(Transform enemy)
    {
        try
        {
            enemy.GetComponent<Enemy>().takeDamage(deathDamage); //Try getting enemy script and damage it
        }
        catch (System.Exception) { }
    }
    private void SetNextWaypoint()
    {
        if (waypointIndex <= 0) //If it has reached the end waypoint
        {
            Destroy(Instantiate(effect, transform.position, transform.rotation), 7f); //Create particle effect and destroy after 7 seconds
            Destroy(gameObject);
            return;
        }
        waypointIndex--; //Decrease index
        target = Waypoints.points[waypointIndex]; //Get next target
    }
}
