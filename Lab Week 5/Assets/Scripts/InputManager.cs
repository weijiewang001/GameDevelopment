using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    private Transform[] transArray;
    private Vector3 rotateAngle;

	// Use this for initialization
	void Start () {
        transArray = new Transform[2];
        transArray[0] = GameObject.FindWithTag("Red").transform;
        transArray[1] = GameObject.FindWithTag("Blue").transform;

        rotateAngle = new Vector3(0.0f, 0.0f, 45.0f);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("w")) {
            transArray[0].Rotate(rotateAngle);
            transArray[1].Rotate(-rotateAngle);
        }

        if (Input.GetButtonDown(("Fire1"))) {
            float trans0Y = transArray[0].position.y;
            transArray[0].position = new Vector3(transArray[0].position.x,
                                                 transArray[1].position.y,
                                                 transArray[0].position.z);
            transArray[1].position = new Vector3(transArray[1].position.x,
                                                 trans0Y,
                                                 transArray[1].position.z);
        }

        if (Input.GetButtonUp("Fire1")) {
			PrintAndHide printHide = transArray [0].GetComponent<PrintAndHide> ();

			if (printHide) {
				Renderer rendRef = printHide.rend;
				rendRef.material.color = new Color (Random.Range (0.2f, 1.0f), 0.0f, 0.0f);
				Debug.Log ("Red: " + rendRef.material.color);

				rendRef = transArray [1].GetComponent<PrintAndHide> ().rend;
				rendRef.material.color = new Color (0.0f, 0.0f, Random.Range (0.2f, 1.0f));
				Debug.Log ("Blue: " + rendRef.material.color);
			}
        }

        if (Input.GetKeyDown("e")) {
            PrintAndHide redComp = transArray[0].GetComponent<PrintAndHide>();
            if (redComp != null)
            {
                Destroy(redComp);
                Destroy(transArray[1].GetComponent<PrintAndHide>());
            }
            else
            {
				for (int i = 0; i < transArray.Length; i++) {
					transArray [i].gameObject.AddComponent<PrintAndHide> ();
					if (!transArray[i].gameObject.activeSelf)
						transArray[i].gameObject.SetActive(true);
				}
            }
        }
	}
}
