using UnityEngine;
using Cinemachine;
public class PlayerCamera : MonoBehaviour
{
    public CinemachineFreeLook cam;
    public float dragSpeed = .5f;
    private Vector3 dragOrigin;
    private Vector2 axisValues;
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition; //Setting the location where mouse is held down
            return;
        }
        if (!Input.GetMouseButton(1)) return; //Return if mouse isn't still held down
        Vector3 pos = Input.mousePosition - dragOrigin; //Get difference in position
        dragOrigin = Input.mousePosition; //Setting new origin to where mouse is
        axisValues = new Vector2(axisValues.x + pos.x * dragSpeed, axisValues.y - pos.y * dragSpeed / 200); //Setting value of rotation for camera
        cam.m_XAxis.Value = axisValues.x; //Assigning these values to camera
        cam.m_YAxis.Value = axisValues.y;
        axisValues.y = Mathf.Clamp(axisValues.y, 0, 1); //Clamping y value of camera
    }
}