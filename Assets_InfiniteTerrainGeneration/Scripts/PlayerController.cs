using UnityEngine;

public class PlayerController : MonoBehaviour {
    public GameObject player;
    public Rigidbody rb;
    public float speed;
    public float sensitivity = 10f;
    public float maxYAngle = 80f;
    private Vector2 currentRotation;
    void Start() {
        rb = player.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void FixedUpdate() {
        if (Input.GetKey(KeyCode.W)) { rb.AddRelativeForce(Vector3.forward * speed); } 
        else if (Input.GetKey(KeyCode.S)) { rb.AddRelativeForce(Vector3.back * speed); }
        if (Input.GetKey(KeyCode.A)) { rb.AddRelativeForce(Vector3.left * speed); } 
        else if (Input.GetKey(KeyCode.D)) { rb.AddRelativeForce(Vector3.right * speed); }
        if(Input.GetKey(KeyCode.Space)) { rb.AddRelativeForce(Vector3.up * speed); }
        currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
        currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
        player.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
    }
}
