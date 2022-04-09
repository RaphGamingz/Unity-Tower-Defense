using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
public class TerrainLoader : MonoBehaviour
{
    [Header("Gameobjects")]
    public GameObject waypoint;
    public GameObject terrain;
    public TerrainScript terrainScript;
    [Header("Prefabs")]
    public GameObject waypointPrefab;
    public GameObject terrainPrefab;
    public static TerrainLoader instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        } else
        {
            Debug.LogError("More than one terrain loader in a scene");
        }
    }
    void Update()
    {
        if (Application.isEditor) //Allow user to load and save files with a key if it is in editor
        {
            if (Input.GetKeyDown(KeyCode.Pause))
            {
                print("SAVING");
                saveTerrain();
            }
            if (Input.GetKeyDown(KeyCode.ScrollLock))
            {
                print("Loading");
                loadTerrain(Application.dataPath + "/Resources/Maps/map1.txt");
            }
        }
    }
    public void saveTerrain()
    {
        if (Application.isEditor) //Only allow saving if it is in an editor
        {
            //Get waypoint data
            int waypointChildren = waypoint.transform.childCount;
            Transform[] waypoints = new Transform[waypointChildren];
            for (int i = 0; i < waypointChildren; i++)
            {
                waypoints[i] = waypoint.transform.GetChild(i);
            }
            //Get terrain data
            int terrainChildren = terrain.transform.childCount;
            GameObject[] terrains = new GameObject[terrainChildren];
            for (int i = 0; i < terrainChildren; i++)
            {
                terrains[i] = terrain.transform.GetChild(i).gameObject;
            }
            //Combine data as a class
            TerrainData td = new TerrainData(waypoints, terrains);
            BinaryFormatter formatter = new BinaryFormatter();
            //Get time to use as the file name
            DateTime time = DateTime.Now;
            string path = Application.dataPath + "/Resources/Maps/map" + time.Day as string + time.Hour as string + time.Minute as string + time.Second as string + ".txt";
            FileStream stream = new FileStream(path, FileMode.Create); //Set up stream
            formatter.Serialize(stream, td); //Use stream to serialize terrain data
            stream.Close();
            print("SAVED TO: " + path);
        }
        else
        {
            Debug.LogError("Cannot save terrain in an app thats illegal'");
        }
    }
    public static void sloadTerrain(string path)
    {
        instance.loadTerrain(path);
    }
    public void loadTerrain(string path)
    {
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open); //Set up stream
            TerrainData td = (TerrainData) formatter.Deserialize(stream); //Deserialize the file in the stream as terrain data
            stream.Close();
            //Clear waypoints
            int waypointChildren = waypoint.transform.childCount;
            for (int i = 0; i < waypointChildren; i++)
            {
                Destroy(waypoint.transform.GetChild(i).gameObject);
            }
            waypoint.transform.DetachChildren();
            //Create waypoints
            for (int j = 0; j < td.waypointsPos.Length; j++)
            {
                Vector3 position = new Vector3(td.waypointsPos[j][0], td.waypointsPos[j][1], td.waypointsPos[j][2]); //Get position
                Instantiate(waypointPrefab, position, Quaternion.identity, waypoint.transform); //Create a waypoint at position
            }
            //Clear terrain
            int terrainChildren = terrain.transform.childCount;
            for (int i = 0; i < terrainChildren; i++)
            {
                Destroy(terrain.transform.GetChild(i).gameObject);
            }
            terrain.transform.DetachChildren();
            //Create terrain
            for (int j = 0; j < td.terrainObs.Length; j++)
            {
                ObjectData objectData = td.terrainObs[j];
                Vector3 position = new Vector3(objectData.objPos[0], objectData.objPos[1], objectData.objPos[2]); //Get position
                Quaternion rotation = Quaternion.Euler(objectData.objRot[0], objectData.objRot[1], objectData.objRot[2]); //Get rotation
                Vector3 scale = new Vector3(objectData.objScl[0], objectData.objScl[1], objectData.objScl[2]); //Get scale
                GameObject terrainObject = Instantiate(terrainPrefab, position, rotation, terrain.transform); //Create a game object with the position and rotation
                terrainObject.transform.localScale = scale; //Set scale of game object
                TerrainScript terrainS = terrainObject.GetComponent<TerrainScript>(); //Get terrain script of game object
                //Set values of script
                terrainS.YOffset = terrainScript.YOffset;
            }
        }
        else
        {
            Debug.LogError("File not found: " + path);
        }
    }
}
[Serializable]
public class TerrainData
{
    public float[][] waypointsPos;
    public ObjectData[] terrainObs;
    public TerrainData(Transform[] waypoints, GameObject[] terrains)
    {
        //Waypoints
        int waypointLength = waypoints.Length;
        waypointsPos = new float[waypointLength][];
        for (int i = 0; i < waypointLength; i++)
        {
            Vector3 waypoint = waypoints[i].position; //Convert waypoint positions to 3 floats which can be serialized
            waypointsPos[i] = new float[3];
            waypointsPos[i][0] = waypoint.x;
            waypointsPos[i][1] = waypoint.y;
            waypointsPos[i][2] = waypoint.z;
        }
        //Terrain
        int terrainsLength = terrains.Length;
        terrainObs = new ObjectData[terrainsLength];
        for (int j = 0; j < terrainsLength; j++)
        {
            terrainObs[j] = new ObjectData(terrains[j]); //Pass information to object data class
        }
    }
}
[Serializable]
public class ObjectData
{
    public float[] objPos = new float[3];
    public float[] objRot = new float[3];
    public float[] objScl = new float[3];
    public ObjectData(GameObject gameObject)
    {
        Transform transform = gameObject.transform; //Get transform of object
        Vector3 position = transform.position; //Get position and split it to 3 floats
        objPos[0] = position.x;
        objPos[1] = position.y;
        objPos[2] = position.z;
        Vector3 rotation = transform.rotation.eulerAngles; //Get rotation as a vector3 and split it to 3 floats
        objRot[0] = rotation.x;
        objRot[1] = rotation.y;
        objRot[2] = rotation.z;
        Vector3 scale = transform.localScale; //Get scale and split it to 3 floats
        objScl[0] = scale.x;
        objScl[1] = scale.y;
        objScl[2] = scale.z;
    }
}