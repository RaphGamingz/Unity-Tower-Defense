using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Unlocks : MonoBehaviour
{
    public UnlockType unlockType = UnlockType.Tower;
    public GameObject unlockPanel;
    public ScrollRect scrollRect;
    public GameObject towers;
    public GameObject upgrades;
    public TextMeshPro creditsText;
    private int credits;
    [ColorUsage(true, true)]
    public Color locked;
    [ColorUsage(true, true)]
    public Color unlocked;
    [ColorUsage(true, true)]
    public Color selected;

    public List<UnlockBlueprint> towerUnlocks = new List<UnlockBlueprint>();
    public List<UnlockBlueprint> unlocks = new List<UnlockBlueprint>();
    public List<UnlockBlueprint> selectedTowerUnlocks = new List<UnlockBlueprint>();

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
        credits = PlayerPrefs.GetInt("Credits", 0);
        creditsText.text = "Credits: " + credits;
        updateButtonColours();
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
        instance.credits = PlayerPrefs.GetInt("Credits", 0);
        instance.creditsText.text = "Credits: " + instance.credits;
    }
    private void updateButtonColours()
    {
        for (int i = 0; i < towerUnlocks.Count; i++)
        {
            UnlockBlueprint unlock = towerUnlocks[i];
            setColour(unlock.unlockButton, unlock.unlocked ? unlocked : locked);
        }
        for (int i = 0; i < unlocks.Count; i++)
        {
            UnlockBlueprint unlock = unlocks[i];
            setColour(unlock.unlockButton, unlock.unlocked ? unlocked : locked);
        }
    }
    public void unlock(Button button)
    {
        UnlockBlueprint unlockBlueprint = null;
        bool towerUnlock = false;
        for (int i = 0; i < towerUnlocks.Count; i++)
        {
            UnlockBlueprint unlock = towerUnlocks[i];
            if (unlock.unlockButton == button)
            {
                unlockBlueprint = unlock;
                towerUnlock = true;
            }
        }
        for (int i = 0; i < unlocks.Count; i++)
        {
            UnlockBlueprint unlock = unlocks[i];
            if (unlock.unlockButton == button)
            {
                unlockBlueprint = unlock;
                towerUnlock = false;
            }
        }
        if (unlockBlueprint == null)
        {
            return;
        }
        if (unlockBlueprint.unlocked)
        {
            if (towerUnlock)
            {
                if (selectedTowerUnlocks.Contains(unlockBlueprint))
                {
                    setColour(button, unlocked);
                    selectedTowerUnlocks.Remove(unlockBlueprint);
                }
                else
                {
                    setColour(button, selected);
                    selectedTowerUnlocks.Add(unlockBlueprint);
                }
            }
        } else
        {
            if (credits >= unlockBlueprint.creditCost)
            {
                credits -= unlockBlueprint.creditCost;
                PlayerPrefs.SetInt("Credits", credits);
                updateCredits();
                unlockBlueprint.unlocked = true;
                setColour(button, unlocked);
                processUnlockAction(unlockBlueprint);
            }
        }
    }
    public void processUnlockAction(UnlockBlueprint blueprint)
    {
        UnlockAction unlockAction = blueprint.unlockAction;
        if (unlockAction == UnlockAction.None) {
            return;
        }
        switch(unlockAction)
        {
            case UnlockAction.MaxTower:
                PlayerStats.maxTowers += 5;
                break;
            default:
                break;
        }
    }
    public void setColour(Button button, Color colour)
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = colour;
        button.colors = colorBlock;
    }
    public void setUnlockPanel(bool active)
    {
        unlockPanel.SetActive(active);
    }
    public enum UnlockType
    {
        Upgrade,
        Tower
    }
}
public enum UnlockAction
{
    None,
    MaxTower
}