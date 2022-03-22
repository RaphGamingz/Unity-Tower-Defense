using UnityEngine;
public class Waypoints : MonoBehaviour
{
    public static Transform[] points = new Transform[0]; //List of all points in waypoints
    public WaveSpawner waveSpawner;
    public Transform pathParent;
    public GameObject path;
    public GameObject endPrefab;
    public static Waypoints instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            print("Only one waypoint per scene please");
        }
    }
    public static void Initialise()
    {
        RemovePath(); //Remove any existing path
        instance.LoadPoints(); //Load points in waypoints
        if (points.Length > 0)
        {
            instance.waveSpawner.spawnPoint = points[0]; //Set spawn of enemies
            instance.GeneratePath(); //Generate the path enemies move on
        }
    }
    public void LoadPoints()
    {
        points = new Transform[transform.childCount]; //Initialise list
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = transform.GetChild(i); //Load all childs of waypoints
        }
    }
    public void GeneratePath()
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            Transform a = points[i]; //Get first point
            Vector3 dir = a.position - points[i + 1].position; //Get direction from first point to second point
            GameObject pathInstance = Instantiate(path, a.position - dir / 2, Quaternion.LookRotation(dir), pathParent); //Create path, facing second point from first point
            Vector3 scale = pathInstance.transform.localScale; //Get path scale
            scale.z = dir.magnitude + scale.x; //Change path scale
            pathInstance.transform.localScale = scale; //Set path scale
        }
        Transform end = points[points.Length - 1]; //Get last point
        Instantiate(endPrefab, end.position, Quaternion.LookRotation(end.position - points[points.Length - 2].position), pathParent); //Create the end object (where enemies dissolve)
    }
    public static void RemovePath()
    {
        for (int i = instance.pathParent.childCount - 1; i > -1; i--)
        {
            Destroy(instance.pathParent.GetChild(i).gameObject); //Remove all paths in path gameobject
        }
    }
}