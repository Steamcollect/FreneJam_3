using UnityEngine;
public class DrawerManager : MonoBehaviour
{
    //[Header("Settings")]

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_DrawConsole rseDrawConsole;

    [Header("Output")]
    [SerializeField] RSE_DrawPlayerRoom rseDrawPlayerRoom;
    [SerializeField] RSE_DrawDungeon rseDrawDungeon;
    [SerializeField] RSE_DrawHealth rseDrawHealth;

    private void OnEnable()
    {
        rseDrawConsole.Action += DrawScene;
    }
    private void OnDisable()
    {
        rseDrawConsole.Action -= DrawScene;
    }

    void DrawScene()
    {
        CustomConsoleWindow.ClearLogs();
        rseDrawPlayerRoom.Call();
        rseDrawDungeon.Call();
        rseDrawHealth.Call();
    }
}