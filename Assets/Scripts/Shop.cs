using UnityEngine;

public class Shop : MonoBehaviour
{
    private BuildManager buildManager;
    private TowerInfo towerInfo;
    void Start()
    {
        towerInfo = TowerInfo.instance;
        buildManager = BuildManager.instance;
    }
    public void SelectTower(int towerNum)
    {
        towerInfo.setTower(null); // Hide the stats of towers
        buildManager.SelectBuildTower(towerNum); //Select the build tower
    }
}