using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WaveSpawner : MonoBehaviour
{
    [HideInInspector] public Transform spawnPoint;
    public Transform enemyPrefab;
    public Transform enemyPrefab2;
    public Transform bossPrefab;

    public Transform enemyParent;

    [Tooltip("Time in seconds between each wave")]
    public float timeBetweenWaves = 5f;
    private float countdown = 5f;
    private int waveIndex = 0;

    public static List<Transform> enemyList = new List<Transform>();
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
        if (countdown <= 0f) //Start spawning enemies if countdown reaches 0
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }
        countdown -= Time.deltaTime; //Reduce countdown by time
        countdown = Mathf.Clamp(countdown, 0, Mathf.Infinity); //Clamp countdown so that it doesn't show negatives
        UIManager.UpdateCount(countdown); //Update UI
    }
    IEnumerator SpawnWave()
    {
        waveIndex++;
        BuildManager.instance.WaveStart(); //Tell buildmanager that the wave has started
        UIManager.UpdateWave(waveIndex); //Update UI
        for (int i = 0; i < waveIndex; i++)
        {
            SpawnEnemy(enemyPrefab);
            yield return new WaitForSecondsRealtime(0.5f);
        }
        if (waveIndex % 2 == 0)
        {
            for (int i = 0; i < waveIndex / 5; i++)
            {
                SpawnEnemy(enemyPrefab2);
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
        if (waveIndex % 10 == 0)
        {
            for (int i = 0; i < waveIndex / 10; i++)
            {
                SpawnEnemy(bossPrefab);
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
        if (waveIndex == 50)
        {
            for (int i = 0; i < 50; i++)
            {
                SpawnEnemy(bossPrefab);
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
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
}