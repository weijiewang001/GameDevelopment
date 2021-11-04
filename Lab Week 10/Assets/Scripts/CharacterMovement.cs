using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {    
    public Animator moveAnimator;

    private Vector3 movement;
    private float movementSqrMagnitude; 
    
	
	void Update () {
        GetMovementInput();
        CharacterRotation();
        WalkingAnimation();
        CameraRotate();
	}


    void GetMovementInput() {
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");
        movement = Vector3.ClampMagnitude(movement, 1.0f);
        movementSqrMagnitude = movement.sqrMagnitude;
    }


    void CharacterRotation() {
        if (movement != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
        }
    }


    void WalkingAnimation() {
        moveAnimator.SetFloat("MoveSpeed", movementSqrMagnitude);
    }

    void CameraRotate() {
        float angle = Input.GetKey(KeyCode.J) ? -20.0f : 0.0f;
        angle = Input.GetKey(KeyCode.L) ? 20.0f : angle;
        Camera.main.transform.Rotate(Vector3.up, angle * Time.deltaTime, Space.World);
    }
}
