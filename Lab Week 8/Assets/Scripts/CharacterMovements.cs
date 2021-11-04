using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovements : MonoBehaviour {    
    public Animator moveAnimator;
    public AudioSource footStepSource;
    public AudioClip[] footStepClips;
    public AudioSource backgroundMusic;

    public float walkSpeed = 1.5f;

    private Vector3 movement;
    private float movementSqrMagnitude; 
    private int currentClip = 0;
    private bool currentlyColliding = false;
	
	
	void Update () {
        GetMovementVector();
        if (!RayCastCheck())

            CharacterPostion();
        else
            movementSqrMagnitude = 0.0f;

        CharacterRotation();
        WalkingAnimation();
        FootstepAudio();
    }


    void GetMovementVector() {
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");
        movement = Vector3.ClampMagnitude(movement, 1.0f);
        movementSqrMagnitude = movement.sqrMagnitude;
    }


    void CharacterPostion() {
        transform.Translate(movement * walkSpeed * Time.deltaTime, Space.World);
    }


    void CharacterRotation() {
        if (movement != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
        }
    }


    void WalkingAnimation() {
        moveAnimator.SetFloat("MoveSpeed", movementSqrMagnitude);
    }


    void FootstepAudio() {
        if (movementSqrMagnitude > 0.3f) {
            if (!footStepSource.isPlaying) {
                currentClip = 1 - currentClip;
                footStepSource.clip = footStepClips[currentClip];
                footStepSource.volume = movementSqrMagnitude;
                footStepSource.Play();
                                
                backgroundMusic.volume = 0.5f;
            }
        } else {
            if (footStepSource.isPlaying) {
                footStepSource.Stop();
                backgroundMusic.volume = 1.0f;
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exit: " + other.name + " : " + other.transform.position);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Enter: " + collision.gameObject.name + " : " + collision.contacts[0].point);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Impassable"))
        {
            Debug.Log("Collision Stay: " + collision.gameObject.name);
        }
    }


    bool RayCastCheck()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, out hitInfo, 5.0f);
        if (hit)
        {
            Debug.Log("Raycast Hit: " + hitInfo.collider.name);
            if (hitInfo.collider.CompareTag("Freeze"))
                return true;
        }
        return false;
    }

}
