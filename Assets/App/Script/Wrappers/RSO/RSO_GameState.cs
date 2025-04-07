using UnityEngine;

[CreateAssetMenu(fileName = "RSO_GameState", menuName = "RSO/Game/RSO_GameState")]
public class RSO_GameState : BT.ScriptablesObject.RuntimeScriptableObject<GameState>{}

public enum GameState
{
    Intro,
    InGame,
    Win,
    Lose
}