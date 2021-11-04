using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintAndHide : MonoBehaviour {
    public Renderer rend;
    int i, randValue;

	// Use this for initialization
	void Start () {
        i = 3;
        
        randValue = Random.Range(200, 251); 
	}
	
	// Update is called once per frame
	void Update () {
        i++;
        Debug.Log(gameObject.name + ":" + i);

        if (gameObject.CompareTag("Red") && i == 100)
            gameObject.SetActive(false);
        if (gameObject.CompareTag("Blue") && i == randValue)
            rend.enabled = false;
	}
}
