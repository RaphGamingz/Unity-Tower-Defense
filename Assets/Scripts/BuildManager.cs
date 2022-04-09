using System.Collections.Generic;
using UnityEngine;
public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;
    public List<TowerBlueprint> towers;
    public GameObject buildEffect;
    public GameObject sellEffect;
    public List<Tower> towerList = new List<Tower>();
    [Header("Tower")]
    public Transform towersParent;
    public Camera mainCamera;
    public GameObject ghost;
    public Ghost ghostScript;
    private TowerBlueprint selectedBuildTower = null;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one build manager in a scene");
            return;
        }
        instance = this;
    }
    public bool canBuild { get { return selectedBuildTower != null; } }
    public GameObject selectedTower { get { return selectedBuildTower.tower; } }
    public TowerBlueprint selectedBlueprint { get { return selectedBuildTower; } }
    public void SelectBuildTower(int towerNum)
    {
        selectedBuildTower = towers[towerNum]; //Select build tower
    }
    public void RemoveSelectedBuildTower()
    {
        selectedBuildTower = null; //Unselect build tower
    }
    public void BuildTower(GameObject ghost)
    {
        if (PlayerStats.energy < selectedBuildTower.buyCost || PlayerStats.towers >= PlayerStats.maxTowers || GameManager.gameEnded)
            return;
        PlayerStats.ChangeEnergy(-selectedBuildTower.buyCost); //Reduce energy by how much it costs
        PlayerStats.ChangeTowerCount(1); //Increase the number of towers that are in the game
        GameObject tower = Instantiate(selectedBuildTower.tower, ghost.transform.position, ghost.transform.rotation, towersParent); //create the tower
        Tower t = tower.GetComponent<Tower>(); //Get the script of the tower
        t.SetTowerBlueprint(selectedBuildTower); //Set the blueprint of the tower
        towerList.Add(t); //Add the tower to the list of towers
        Destroy(Instantiate(buildEffect, ghost.transform.position, Quaternion.identity), 2f); //Create build effect and destroy 2 seconds later
        RemoveSelectedBuildTower(); //Unselect the selected tower
    }
    public Tower UpgradeTower(Tower tower)
    {
        if (tower == null)
            return null;
        TowerBlueprint tb = tower.GetTowerBlueprint();
        int nextTowerLevel = tower.TowerLevel; //Get next tower
        if (nextTowerLevel >= tb.upgradeCosts.Count)
            return null;
        if (PlayerStats.energy < tb.upgradeCosts[nextTowerLevel] || GameManager.gameEnded)
            return null;
        towerList.Remove(tower); //Remove the tower from the list of towers
        Destroy(tower.gameObject); //Remove the old tower
        PlayerStats.ChangeEnergy(-tb.upgradeCosts[nextTowerLevel]); //Reduce energy by how much it costs
        GameObject upgradedTower = Instantiate(tb.upgradedTowers[nextTowerLevel], tower.transform.position, tower.transform.rotation, towersParent); //Create the upgraded tower
        Tower t = upgradedTower.GetComponent<Tower>(); //Get the script of the upgraded tower
        t.SetTowerBlueprint(tb); //Set the blueprint of the upgraded tower
        towerList.Add(t); //Add the upgraded tower to the list of towers
        Destroy(Instantiate(buildEffect, tower.transform.position, Quaternion.identity), 2f); //Create build effect and destroy 2 seconds later
        return t;
    }
    public void SellTower(Tower tower)
    {
        if (!tower.GetDestroyed())
        {
            PlayerStats.ChangeTowerCount(-1); //Reduce number of towers
            PlayerStats.ChangeEnergy(tower.GetTowerBlueprint().buyCost / 2); //Give back half of the energy required to build the tower
            tower.SetDestroyed(true); //Set the tower to be destroyed
            Destroy(Instantiate(sellEffect, tower.transform.position, Quaternion.identity), 2f); //Create sell effect and destroy 2 seconds later
            return;
        }
    }
    public void WaveStart()
    {
        for (int i = towerList.Count - 1; i >= 0; i--) //Loop through every tower, backwards (as they may get removed)
        {
            Tower tl = towerList[i]; //Get tower
            if (tl != null) //If tower exists
                tl.WaveStart(); //Tell it that the wave is starting
            else
                towerList.Remove(tl); //Remove tower reference if it doesn't exist
        }
    }
}