using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float moveSpeedForward, moveSpeedBackWard, moveSpeedLeft, moveSpeedRight;

    private Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new(horizontalInput == 1 ? moveSpeedRight * horizontalInput : moveSpeedLeft * horizontalInput, 0, verticalInput == 1 ? moveSpeedForward * verticalInput : -moveSpeedBackWard * verticalInput);
        moveDirection = transform.TransformDirection(moveDirection);
        if (moveDirection != Vector3.zero) { rb.velocity = moveDirection; }
    }
}
