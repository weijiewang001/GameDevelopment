using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    int sceneInd = 0;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.N))
        {
            sceneInd++;
            if (sceneInd < 3)
                SceneManager.LoadScene(sceneInd);
            else
                Application.Quit();
        }
    }
}
