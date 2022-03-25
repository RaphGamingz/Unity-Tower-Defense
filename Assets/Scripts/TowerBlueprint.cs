using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TowerBlueprint
{
    public GameObject tower;
    public int buyCost;
    public List<int> upgradeCosts;
    public List<GameObject> upgradedTowers;
    [Tooltip("If the tower can be placed on ground")]
    public bool ground;
}