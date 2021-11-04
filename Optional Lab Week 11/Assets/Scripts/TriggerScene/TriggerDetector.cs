using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour {
	
	private void OnTriggerEnter(Collider otherObj) {
        if (otherObj.CompareTag("Red"))
            Debug.Log(otherObj.name);
    }

    private void OnTriggerExit(Collider otherObj) {
        if (otherObj.CompareTag("Blue"))
            Debug.Log(otherObj.name);
    }
}
