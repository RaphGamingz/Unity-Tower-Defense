using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TextMeshPro countText;
    public TextMeshPro waveText;
    public TextMeshPro healthText;
    public TextMeshPro energyText;

    public GameObject trollObject;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one ui manager in a scene");
            return;
        }
        instance = this;
    }
    public static void UpdateEnergy(int energy) //Energy to build towers
    {
        instance.energyText.text = "Energy:\n" + energy;
        if (!instance.trollObject.activeSelf && energy >= 2000)
        {
            instance.trollObject.SetActive(true);
        } else if (instance.trollObject.activeSelf && energy < 2000)
        {
            instance.trollObject.SetActive(false);
        }
    }
    public static void UpdateHealth(int health) //Health of game
    {
        instance.healthText.text = "Health:\n" + health;
    }
    public static void UpdateCount(float count) //Countdown of spawner
    {
        instance.countText.text = "Countdown:\n" + count.ToString("0.00");
    }
    public static void UpdateWave(int wave) //Wave of game
    {
        instance.waveText.text = "Wave:\n" + wave;
    }
}