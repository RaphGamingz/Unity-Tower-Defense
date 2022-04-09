using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Wave
{
    public EnemyType[] enemies;
    public float rate;
    public int bonusEnergy;
}
[System.Serializable]
public class EnemyType
{
    public Transform enemy;
    public int amount;
}