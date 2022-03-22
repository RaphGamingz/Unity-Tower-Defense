using UnityEngine;
public class TerrainScript : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject ghost;
    public Ghost ghostScript;
    public Transform towersParent;
    public float YOffset;
    private BuildManager buildManager;
    private float clickTime;
    private Vector3 clickPos;
    void OnMouseDown()
    {
        if (!buildManager.canBuild || !buildManager.hasEnergy || GameManager.gameEnded || PlayerStats.towers >= PlayerStats.maxTowers)
            return;
        if (Input.GetMouseButtonDown(0)) //If mouse is pressed and a tower can is selected
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (clickTime < 0.5f && Vector3.SqrMagnitude(clickPos - hit.point) < 1) //If thehit point is very close and clicked twice very quickly
                {
                    ghost.SetActive(false); //Hide ghost
                    buildManager.BuildTower(ghost, towersParent); //Try build tower
                } else
                {
                    if (hit.point.y + YOffset >= 1 && !buildManager.selectedBlueprint.ground || hit.point.y + YOffset < 1 && buildManager.selectedBlueprint.ground)
                    {
                        clickTime = 0; //Change click time to 0
                        clickPos = hit.point; //Set position where the mouse was
                        ghost.SetActive(true); //Show ghost of tower
                        ghostScript.setGameobject(buildManager.selectedTower); //Set the tower the ghost is showing to the selected tower
                        ghost.transform.position = hit.point; //Set position of ghost to the mouse position
                    }
                }
            }
        }
    }
    void Start()
    {
        buildManager = BuildManager.instance;
    }
    void Update()
    {
        if (!buildManager.canBuild || !buildManager.hasEnergy || GameManager.gameEnded || PlayerStats.towers >= PlayerStats.maxTowers)
        {
            if (ghost.activeInHierarchy)
            {
                ghost.SetActive(false); //If the tower cannot be built but the ghost is still shown, hide it
            }
            return;
        }
        clickTime += Time.deltaTime; //Increase click time by time
    }
}