using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;

public class TowerInfo : MonoBehaviour
{
    public static TowerInfo instance;
    public Camera mainCamera;
    public Button upgradeButton;
    public TextMeshProUGUI upgradeText;
    public TextMeshProUGUI sellText;
    public TextMeshProUGUI aimText;
    public GameObject aim;
    private BuildManager buildManager;
    private Tower tower;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one tower info in a scene");
            return;
        }
        instance = this;
        gameObject.SetActive(false); //Hide UI
    }
    void Start()
    {
        buildManager = BuildManager.instance;
    }
    void Update()
    {
        transform.LookAt(mainCamera.transform); //Always face the camera
    }
    public void setTower(Tower _tower) //Sets the position of the UI onto the tower and stores the tower
    {
        tower = _tower;
        if (tower != null) //If tower is not null, set its position to the towers position and enable UI
        {
            transform.position = tower.transform.position;
            TowerBlueprint tb = tower.GetTowerBlueprint();
            if (tower.TowerLevel < tb.upgradeCosts.Count)
            {
                upgradeText.text = "<b>UPGRADE</b>\n" + tb.upgradeCosts[tower.TowerLevel] + " Energy";
                upgradeButton.interactable = true;
            }
            else
            {
                upgradeText.text = "<b>NO UPGRADES</b>";
                upgradeButton.interactable = false;
            }
            if (tower.TowerLevel == 0)
            {
                sellText.text = "<b>SELL</b>\n" + tb.buyCost / 2 + " Energy";
            } else
            {
                sellText.text = "<b>SELL</b>\n" + (tb.buyCost + tb.upgradeCosts.Take(tower.TowerLevel).Sum()) / 2 + " Energy";
            }
            aim.SetActive(tb.aimable);
            aimText.text = Regex.Replace(Regex.Replace(tower.aimtype.ToString(), @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2"); //Gets the enum's string and also splits camel case to multiple words
            gameObject.SetActive(true);
        } else //If tower is null, disable UI
        {
            gameObject.SetActive(false);
        }
    }
    public void Upgrade()
    {
        setTower(buildManager.UpgradeTower(tower)); //Upgrade tower and select it
    }
    public void Sell()
    {
        if (tower != null)
        {
            buildManager.SellTower(tower); //Sell tower
            setTower(null); //Deselect tower
        }
    }
    public void SetAimType(int aimtype)
    {
        tower.aimtype = (AimType) aimtype;
        aimText.text = Regex.Replace(Regex.Replace(tower.aimtype.ToString(), @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2"); //Gets the enum's string and also splits camel case to multiple words
    }
}