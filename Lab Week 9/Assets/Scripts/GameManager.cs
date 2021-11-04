using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public enum GameState { Start, WalkingLevel }
    public static GameState currentGameState = GameState.Start;
}
