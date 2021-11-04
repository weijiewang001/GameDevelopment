using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLoader : MonoBehaviour {
    public bool leftLoaded, rightLoaded, topLoaded, bottomLoaded = false;
    public Vector3 pos;

    void Update() {
        if (pos.x > 2.5) {
            if (!rightLoaded) {
                SceneManager.LoadSceneAsync("RightScene", LoadSceneMode.Additive);
                rightLoaded = true;
            }
            if (leftLoaded) {
                SceneManager.UnloadSceneAsync("LeftScene");
                leftLoaded = false;
            }
        }

        if (pos.x < -2.5) {
            if (!leftLoaded) {
                SceneManager.LoadSceneAsync("LeftScene", LoadSceneMode.Additive);
                leftLoaded = true;
            }
            if (rightLoaded) {
                SceneManager.UnloadSceneAsync("RightScene");
                rightLoaded = false;
            }
        }

        if (pos.z > 2.5) {
            if (!topLoaded) {
                SceneManager.LoadSceneAsync("TopScene", LoadSceneMode.Additive);
                topLoaded = true;
            }
            if (bottomLoaded) {
                SceneManager.UnloadSceneAsync("BottomScene");
                bottomLoaded = false;
            }
        }

        if (pos.z < -2.5) {
            if (!bottomLoaded) {
                SceneManager.LoadSceneAsync("BottomScene", LoadSceneMode.Additive);
                bottomLoaded = true;
            }
            if (topLoaded) {
                SceneManager.UnloadSceneAsync("TopScene");
                topLoaded = false;
            }
        }
    } 
}
