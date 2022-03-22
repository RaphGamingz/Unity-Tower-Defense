using UnityEngine;
[System.Serializable]
public class TowerBlueprint
{
    public GameObject tower;
    public int cost;
    [Tooltip("If the tower can be placed on ground")]
    public bool ground;
}