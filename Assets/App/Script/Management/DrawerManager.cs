using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DrawerManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, TextArea] string introTxt;
    [SerializeField, TextArea] string winTxt;
    [SerializeField, TextArea] string loseTxt;

    [Header("References")]
    [SerializeField] AudioClip nextPanelClip;

    [Space(10)]
    // RSO
    [SerializeField] RSO_GameState rsoGameState;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_DrawConsole rseDrawConsole;

    [Header("Output")]
    [SerializeField] RSE_DrawPlayerRoom rseDrawPlayerRoom;
    [SerializeField] RSE_DrawDungeon rseDrawDungeon;
    [SerializeField] RSE_DrawHealth rseDrawHealth;
    [SerializeField] RSE_DrawKeyCount rseDrawKeyCount;
    [SerializeField] RSE_StartGame rseStartGame;

    private void OnEnable()
    {
        rseDrawConsole.Action += DrawScene;
    }
    private void OnDisable()
    {
        rseDrawConsole.Action -= DrawScene;
    }

    private void Start()
    {
        rsoGameState.Value = GameState.Intro;
        CustomConsoleWindow.ClearLogs();
        rseStartGame.Call();

        DrawScene();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (rsoGameState.Value)
            {
                case GameState.Intro:
                    rsoGameState.Value = GameState.InGame;
                    DrawScene();
                    break;
                case GameState.Win:
                    Start();
                    break;
                case GameState.Lose:
                    Start();
                    break;
            }
        }
    }

    void DrawScene()
    {
        switch (rsoGameState.Value)
        {
            case GameState.Intro:
                CustomConsoleWindow.ClearLogs();
                CustomConsoleWindow.ReceiveLog(introTxt);
                break;
            case GameState.InGame:
                CustomConsoleWindow.ClearLogs();
                rseDrawPlayerRoom.Call();
                rseDrawDungeon.Call();
                rseDrawHealth.Call();
                rseDrawKeyCount.Call();
                break;
            case GameState.Win:
                CustomConsoleWindow.ClearLogs();
                CustomConsoleWindow.ReceiveLog(winTxt);
                break;
            case GameState.Lose:
                CustomConsoleWindow.ClearLogs();
                CustomConsoleWindow.ReceiveLog(loseTxt);
                break;
        }
    }
}