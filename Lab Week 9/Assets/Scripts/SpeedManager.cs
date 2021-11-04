using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedManager : MonoBehaviour {
    public enum GameSpeed { Slow = 1, Fast = 3 }

    private static float speedModifier = 1.0f;
    private static GameSpeed currentSpeedState = GameSpeed.Slow;

    public static float SpeedModifier { get { return speedModifier; } }

    public static GameSpeed CurrentSpeedState {
        get { return currentSpeedState; }
        set {
            currentSpeedState = value;
            speedModifier = (float)currentSpeedState;
        }
    }
}
