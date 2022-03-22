using UnityEngine;
using System.Collections.Generic;
public class Ghost : MonoBehaviour
{
    public Transform range;
    public Transform tower;
    GameObject currentTower = null;
    GameObject ogTower = null;
    public Material holo;
    public Material noZone;
    Vector3 size = new Vector3();
    public List<Material> hologramList = new List<Material>();
    public void setGameobject(GameObject go)
    {
        if (ogTower == go) //Return if same tower selected
        {
            return;
        }
        if (currentTower != null)
        {
            Destroy(currentTower); //Destroy tower that is being shown
            currentTower = null;
        }
        ogTower = go; //Set tower as selected
        currentTower = Instantiate(go, tower); //Set current tower
        MeshRenderer[] meshes = currentTower.GetComponentsInChildren<MeshRenderer>(false); //Get meshes of current tower
        for (int i = 0; i < meshes.Length; i++)
        {
            if (meshes[i].sharedMaterial != noZone) //If material is not from no zone
            {
                Material[] mats = meshes[i].materials; //Get materials
                hologramList.Clear(); //Clear hologram list
                for (int j = 0; j < mats.Length; j++) //Add the number of materials needed
                {
                    hologramList.Add(holo);
                }
                meshes[i].materials = hologramList.ToArray(); //Set the materials all to hologram
            } else
            {
                meshes[i].gameObject.SetActive(false); //If material is from no zone, set no zone to inactive
            }
        }
        Tower t = currentTower.GetComponent<Tower>();
        float r = 0; //Range
        if (t != null)
        {
            r = t.range * 2;
            t.enabled = false;
        }
        MonoBehaviour[] scripts = currentTower.GetComponents<MonoBehaviour>(); //Get all scripts in tower
        for (int i = 0; i < scripts.Length; i++)
        {
            scripts[i].enabled = false; //Disable all scripts
        }
        size.Set(r, 0.1f, r); //Set size of range to range
        range.localScale = size; // Assign size to the scale
    }
}