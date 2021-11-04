using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour {
    public SaveGameManager saver;

	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            SpeedManager.CurrentSpeedState = (SpeedManager.CurrentSpeedState == SpeedManager.GameSpeed.Slow) ? SpeedManager.GameSpeed.Fast : SpeedManager.GameSpeed.Slow;
            saver.SaveSpeed();
        }

        if (GameManager.currentGameState == GameManager.GameState.Start)
            if (Input.GetKeyDown(KeyCode.Return)) {                
                GameManager.currentGameState = GameManager.GameState.WalkingLevel;
                DontDestroyOnLoad(gameObject);
                Destroy(gameObject.GetComponent<Tweener>());
                SceneManager.LoadScene(1);
            }
	}

}
