using System.Collections;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //[Header("Settings")]
    int keyCountRequire;

    bool debugDelay = true;

    [Header("References")]

    //[Space(10)]
    // RSO
    [SerializeField] RSO_KeyCount rsoKeyCount;
    [SerializeField] RSO_GameState rsoGameState;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_SetKeyAmountRequire rseSetKeyCount;
    [SerializeField] RSE_DrawKeyCount rseDrawKeyCount;
    [SerializeField] RSE_StartGame rseStartGame;

    //[Header("Output")]

    private void OnEnable()
    {
        rseDrawKeyCount.Action += DrawKeyCount;
        rseSetKeyCount.Action += SetMaxKey;
        rseStartGame.Action += OnStart;
        rsoKeyCount.OnChanged += OnKeyPicked;
    }
    private void OnDisable()
    {
        rseDrawKeyCount.Action -= DrawKeyCount;
        rseSetKeyCount.Action -= SetMaxKey;
        rseStartGame.Action -= OnStart;
        rsoKeyCount.OnChanged -= OnKeyPicked;
    }

    private void OnStart()
    {
        rsoKeyCount.Value = 0;
    }

    void SetMaxKey(int max)
    {
        keyCountRequire = max;
    }

    void OnKeyPicked(int keyCount)
    {
        if(keyCount >= keyCountRequire && !debugDelay)
        {
            rsoGameState.Value = GameState.Win;
        }
    }

    void DrawKeyCount()
    {
        CustomConsoleWindow.ReceiveLog($"<color=yellow>Health: {rsoKeyCount.Value}/{keyCountRequire}</color>");
    }

    IEnumerator DebugDelay()
    {
        debugDelay = true;
        yield return new WaitForSeconds(0.5f);
        debugDelay = false;
    }
}