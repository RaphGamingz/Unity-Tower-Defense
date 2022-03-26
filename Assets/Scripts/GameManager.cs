using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static bool gameEnded = true;
    public static string terrainLoaded;
    public static GameManager instance;
    public Material lighting;

    public GameObject gameEndEffect;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        } else
        {
            Debug.LogError("More than one game manager in a scene");
        }
    }
    public static void updateHealth(int health)
    {
        if (health <= 0 && !gameEnded)
        {
            endGame(); //end game if player's helath is less than or equal to 0
        }
        if (gameEnded)
            health = PlayerStats.startHealth; //If game has already ended, reset health
        instance.lighting.SetColor("_EmissionColor", Color.HSVToRGB((float)health / (float)PlayerStats.startHealth * 128 / 360, 1, 0.75f, true) * 4);
        DynamicGI.UpdateEnvironment(); //Set color of lighting and update the environment
    }
    public static void startGame(string mapID)
    {
        if (gameEnded == true) //If game has ended, start the game
        {
            gameEnded = false;
            if (terrainLoaded != mapID) //if terrain isn't the same terrain as before
            {
                TerrainLoader.sloadTerrain(Application.dataPath + "/Resources/Maps/" + mapID + ".txt"); //Load terrain
                terrainLoaded = mapID;
            }
            Waypoints.Initialise(); //Initialise waypoints to new terrain
        }
    }
    public static void endGame()
    {
        if (gameEnded == false) //If game hasn't ended, reset everything
        {
            Destroy(Instantiate(instance.gameEndEffect, Waypoints.points[Waypoints.points.Length - 1].position, Quaternion.Euler(-90, 0, 0)), 6);
            Waypoints.RemovePath();
            PlayerStats.Reset();
            WaveSpawner.Reset();
            TowerInfo.instance.setTower(null);
            gameEnded = true;
        }
    }

    public static void returnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}