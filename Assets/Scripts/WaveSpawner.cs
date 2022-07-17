using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WaveSpawner : MonoBehaviour
{
    public static int EnemiesAlive = 0;
    public Wave[] waves;
    [HideInInspector] public Transform spawnPoint;
    public Transform enemyParent;
    [Tooltip("Time in seconds between each wave")]
    public float timeBetweenWaves = 5f;
    private float countdown = 15f;
    private int waveIndex = 0;
    private bool spawning = false;
    public static List<Transform> enemyList = new List<Transform>();
    private static int currentEnemyPos = 0;
    public static WaveSpawner instance = null;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one wave spwaner in a scene");
            return;
        }
        instance = this;
    }
    void Start()
    {
        InvokeRepeating("RemoveMissingEntries", 0f, 5f); //Remove missing entries of enemies every 5 seconds
    }
    void Update()
    {
        if (GameManager.gameEnded) //If game has ended, reset wave, reset countdown and update UI
        {
            waveIndex = 0;
            countdown = timeBetweenWaves;
            UIManager.UpdateCount(countdown);
            return;
        }
        if (EnemiesAlive > 0 || spawning)
        {
            return;
        }
        if (countdown <= 0f) //Start spawning enemies if countdown reaches 0
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
            return;
        }
        countdown -= Time.deltaTime; //Reduce countdown by time
        countdown = Mathf.Clamp(countdown, 0, Mathf.Infinity); //Clamp countdown so that it doesn't show negatives
        UIManager.UpdateCount(countdown); //Update UI
    }
    IEnumerator SpawnWave()
    {
        spawning = true;
        if (waveIndex < waves.Length)
            waveIndex++;
        BuildManager.instance.WaveStart(); //Tell buildmanager that the wave has started
        UIManager.UpdateWave(waveIndex); //Update UI
        Wave wave = waves[waveIndex - 1];
        currentEnemyPos = 0;
        for (int i = 0; i < wave.enemies.Length; i++)
        {
            EnemyType enemyType = wave.enemies[i];
            EnemiesAlive += enemyType.amount;
            for (int num = 0; num < enemyType.amount; num++)
            {
                SpawnEnemy(enemyType.enemy);
                yield return new WaitForSecondsRealtime(1 / wave.rate);
            }
        }
        PlayerStats.ChangeEnergy(wave.bonusEnergy);
        spawning = false;
    }
    void SpawnEnemy(Transform prefab)
    {
        if (spawnPoint != null) //Spawn enemy at spawn point and add it to the list of enemies
        {
            Transform enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, enemyParent);
            enemyList.Add(enemy);
        }
    }
    void RemoveMissingEntries()
    {
        for (int i = enemyList.Count - 1; i > -1; i--)
        {
            if (enemyList[i] == null)
            {
                enemyList.RemoveAt(i); //Remove any destroyed enemies from list
            }
        }
    }
    public static void Reset() //Reset UI to 0
    {
        UIManager.UpdateWave(0);
        UIManager.UpdateCount(0);
    }
    public static int GetWave()
    {
        return instance.waveIndex;
    }
    public static int getEnemyPos()
    {
        return currentEnemyPos++;
    }
}