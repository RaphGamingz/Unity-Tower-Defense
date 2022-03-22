using UnityEngine;

public class Shop : MonoBehaviour
{
    private BuildManager buildManager;
    void Start()
    {
        buildManager = BuildManager.instance;
    }
    public void SelectTower(int towerNum)
    {
        buildManager.SelectBuildTower(towerNum); //Select the build tower
    }
}