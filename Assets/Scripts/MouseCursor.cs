using UnityEngine;
using UnityEngine.UI;
public class MouseCursor : MonoBehaviour
{
    public Vector2 offset;
    public Image image;
    void Start()
    {
        Cursor.visible = false; //Make original mouse invisible
    }
    void Update()
    {
        Vector3 pos = Input.mousePosition; //Get mouse position
        pos.Set(pos.x + offset.x, pos.y - offset.y, pos.z); //Offset position with image
        transform.position = pos; //Set position of mouse cursor at mouse position
        image.color = Color.HSVToRGB((Time.time * 0.1f) % 1, 1, 1); //Change mouse cursor color
    }
}
