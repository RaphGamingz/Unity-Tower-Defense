using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Unlocks : MonoBehaviour
{
    public UnlockType unlockType = UnlockType.Tower;
    public ScrollRect scrollRect;
    public GameObject towers;
    public GameObject upgrades;
    public TextMeshPro creditsText;

    public static Unlocks instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }
        else
        {
            Debug.LogError("More than one unlocks in a scene");
        }
    }
    void Start()
    {
        creditsText.text = "Credits: " + PlayerPrefs.GetInt("Credits", 0);
    }
    public void changeUnlockType(int _unlockType)
    {
        unlockType = (UnlockType) _unlockType;
        if (unlockType == UnlockType.Upgrade)
        {
            towers.SetActive(false);
            upgrades.SetActive(true);
            scrollRect.content = (RectTransform) upgrades.transform;
        }
        else if (unlockType == UnlockType.Tower)
        {
            towers.SetActive(true);
            upgrades.SetActive(false);
            scrollRect.content = (RectTransform) towers.transform;
        }
    }
    public static void updateCredits()
    {
        instance.creditsText.text = "Credits: " + PlayerPrefs.GetInt("Credits", 0);
    }
    public enum UnlockType
    {
        Upgrade,
        Tower
    }
}