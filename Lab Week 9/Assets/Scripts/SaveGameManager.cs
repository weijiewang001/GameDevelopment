using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameManager : MonoBehaviour {
    private const string speedKey = "GameSpeed";
    private const string asyncKey = "AsyncLoader";

	void Awake () {
        LoadSpeed();
	}
	
	public void LoadSpeed() {
        if (PlayerPrefs.HasKey(speedKey))
            SpeedManager.CurrentSpeedState = (SpeedManager.GameSpeed)PlayerPrefs.GetInt(speedKey);
    }

    public void SaveSpeed() {
        PlayerPrefs.SetInt(speedKey, (int)SpeedManager.CurrentSpeedState);
        PlayerPrefs.Save();
    }
}
