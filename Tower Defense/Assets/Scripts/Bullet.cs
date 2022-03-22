using UnityEngine;
public class Bullet : MonoBehaviour
{
    private Transform target;
    private Enemy targetEnemy;
    public GameObject effect;
    public float speed = 10f;
    [Tooltip("The blast radius (range) of the bullet")]
    public float range = 0f;
    private int damage;
    public void SetTarget(Enemy _target)
    {
        targetEnemy = _target;
        target = _target.gameObject.transform;
    }
    public void SetDamage(int _damage)
    {
        damage = _damage;
    }
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); //Destroy game object if it has no target
            return;
        }
        Vector3 dir = target.position - transform.position; //Get direction to target
        float moveDistance = speed * Time.deltaTime; //Get distance bullet can move

        if (dir.sqrMagnitude <= moveDistance * moveDistance) //If the distance the bullet can move is more than the distance of target
        {
            HitTarget();
            return;
        }
        transform.Translate(dir.normalized * moveDistance, Space.World); //Move towards target
        transform.LookAt(target); //Look towards target
    }
    void HitTarget()
    {
        Destroy(Instantiate(effect, transform.position, transform.rotation), 7f); //Create particle effect and destroy after 7 seconds
        if (range > 0f) //If bullet's blast radius is more than 0
        {
            Explode(); //Explode, potentially hurting other enemies around it
        } else
        {
            Damage(targetEnemy); //Only hurt the target enemy
        }
        Destroy(gameObject); //Destroy itself
    }
    void Damage(Transform enemy)
    {
        try
        {
            enemy.GetComponent<Enemy>().takeDamage(damage); //Try getting enemy script and damage it
        }
        catch (System.Exception) {}
    }
    void Damage(Enemy enemy)
    {
        try
        {
            enemy.takeDamage(damage); //Try getting enemy and damage it
        }
        catch (System.Exception) {}
    }
    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range); //Get overlapping colliders in the bullets range
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
            catch (System.Exception)
            {

            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range); //Draw a green sphere to show range of bullet
    }
}