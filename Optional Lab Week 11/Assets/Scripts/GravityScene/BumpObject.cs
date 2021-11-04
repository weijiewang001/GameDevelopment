using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpObject : MonoBehaviour {
    [SerializeField] private GravityManager.GravityDirection gravDirection;
    [SerializeField] private float bumpForce = 10.0f;

    private Rigidbody rb;

	private void Start () {
        if ((rb = GetComponent<Rigidbody>()) == null)
            rb = gameObject.AddComponent<Rigidbody>();

        if (gravDirection != GravityManager.GravityDirection.Up)
            rb.useGravity = false;

        if (gravDirection == GravityManager.GravityDirection.Up)
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;
        else if (gravDirection == GravityManager.GravityDirection.Down)
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        else
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
    }


    private void FixedUpdate() {
        if (gravDirection != GravityManager.GravityDirection.Up)
            rb.AddForce(GravityManager.GravityStength * GravityManager.GravityVectors[(int)gravDirection] , ForceMode.Acceleration);

        if (Input.GetKeyDown(KeyCode.Space))
            rb.AddForce(-GravityManager.GravityVectors[(int)gravDirection] * bumpForce, ForceMode.Impulse);
    }
}
