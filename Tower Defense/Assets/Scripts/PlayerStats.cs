using UnityEngine;
public class PlayerStats : MonoBehaviour
{
    public static int energy;
    public static int startEnergy = 600; //Energy player is given at the start
    public static int health;
    public static int startHealth = 100; //Health player is given at the start
    public static int maxHealth = 100;

    public static int maxTowers = 20; //Towers player is allowed
    public static int towers;

    void Start()
    {
        Reset();
    }
    public static void Reset() //Reset all variables to their original values and update UI
    {
        energy = startEnergy;
        health = startHealth;
        towers = 0;
        UIManager.UpdateEnergy(energy);
        UIManager.UpdateHealth(health);
        GameManager.updateHealth(health);
    }
    public static void ChangeEnergy(int amount)
    {
        energy += amount;
        UIManager.UpdateEnergy(energy);
    }
    public static void ChangeHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UIManager.UpdateHealth(health);
        GameManager.updateHealth(health);
    }
}