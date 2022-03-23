using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;
    public Transform cam;
    [Header("Attributes")]
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    void Update()
    {
        //Get inputs
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized; //Get direction based on inputs

        if (direction.magnitude >= 0.1f) //If the player should move
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y; //Get direction player should face
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //Get direction player will face (smoothing transition between direction and direction it should be at)
            transform.rotation = Quaternion.Euler(0, angle, 0); //Setting rotation (direction)

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward; //Getting forward direction
            characterController.Move(speed * moveDir.normalized * Time.deltaTime); //Moving player forward
        }
    }
}