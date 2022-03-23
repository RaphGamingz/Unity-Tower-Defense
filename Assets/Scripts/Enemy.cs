using System;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    private Transform target;
    private int waypointIndex = 0;
    [Header("Attributes")]
    public float speed = 10f;
    public float turnSpeed = 50f;
    public float health = 5;
    public int energy = 5; // How much energy is given when killed
    [Header("Rendering")]
    public MeshRenderer renderedObject;
    public float dissolveRate = 0.1f;

    private float dissolveValue = 0;
    private Boolean dissolving = false;
    private Boolean appearing = true;
    void Start()
    {
        target = Waypoints.points[waypointIndex];
    }
    void Update()
    {
        if (GameManager.gameEnded)
        {
            dissolving = true; //Dissolve enemy if game has ended
        }
        if (dissolving)
        {
            dissolve(); //Dissolve enemy
        } else
        {
            if (appearing)
                appear(); //Appear enemy
            Vector3 dir = target.position - transform.position; //Get direction to move at
            dir.y = 0; //Don't move up or down
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World); //Move to that direction

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
    }
    public void takeDamage(float amount)
    {
        PlayerStats.ChangeEnergy((int)Mathf.Clamp(amount, 0, health)); //Give player energy by how much health is taken off
        health -= amount; //Reduce health
        if (health <= 0) //If the enemy is dead
        {
            PlayerStats.ChangeEnergy(energy); //Give player extra energy
            dissolving = true; // Set to die
        }
    }
    private void SetNextWaypoint()
    {
        if (waypointIndex >= Waypoints.points.Length - 1) //If it has reached the end waypoint
        {
            dissolving = true; //Start to dissolve
            PlayerStats.ChangeHealth((int)-health); //Reduce player's health by how much health is remaining
            return;
        }
        waypointIndex++; //Increase index
        target = Waypoints.points[waypointIndex]; //Get next target
    }
    private void dissolve()
    {
        if (dissolveValue <= 0)
        {
            Destroy(gameObject); //Destroy enemy if it is fully dissolved
        }
        else
        {
            animateShader(-dissolveRate); //Update the shader
        }
    }
    private void appear()
    {
        if (dissolveValue >= 1)
        {
            appearing = false; //Set the enemy to stop appearing when it is fully appeared
        }
        else
        {
            animateShader(dissolveRate); //Update the shader
        }
    }
    private void animateShader(float dissolveRate)
    {
        dissolveValue += dissolveRate * Time.deltaTime; //Increase or decrease dissolve value
        renderedObject.material.SetFloat("Time", dissolveValue); //Set value to the shader of the material
    }
}