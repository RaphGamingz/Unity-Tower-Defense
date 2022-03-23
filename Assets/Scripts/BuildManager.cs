using System;
using System.Collections.Generic;
using UnityEngine;
public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;
    public List<TowerBlueprint> towers;
    public GameObject buildEffect;
    public List<Tower> towerList = new List<Tower>();
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
    public bool hasEnergy { get { return PlayerStats.energy >= selectedBuildTower.cost; } }
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
    public void BuildTower(GameObject ghost, Transform parent)
    {
        if (!hasEnergy || PlayerStats.towers >= PlayerStats.maxTowers || GameManager.gameEnded)
            return;
        PlayerStats.ChangeEnergy(-selectedBuildTower.cost); //Reduce energy by how much it costs
        PlayerStats.towers++; //Increase the number of towers that are in the game
        GameObject tower = Instantiate(selectedBuildTower.tower, ghost.transform.position, ghost.transform.rotation, parent); //create the tower
        Tower t = tower.GetComponent<Tower>(); //Get the script of the tower
        t.SetTowerCost(selectedBuildTower.cost); //Set the cost of the tower
        towerList.Add(t); //Add the tower to the list of towers
        Destroy(Instantiate(buildEffect, ghost.transform.position, Quaternion.identity), 2f); //Create build effect and destroy 2 seconds later
        RemoveSelectedBuildTower(); //Unselect the selected tower
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