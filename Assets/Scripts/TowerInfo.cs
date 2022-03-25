using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TowerInfo : MonoBehaviour
{
    public static TowerInfo instance;
    public Camera mainCamera;
    public Button upgradeButton;
    public TextMeshProUGUI upgradeText;
    public TextMeshProUGUI sellText;
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
            sellText.text = "<b>SELL</b>\n" + tb.buyCost / 2 + " Energy";
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
    }
}