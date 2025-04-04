using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //[Header("Settings")]
    int keyCountRequire;

    [Header("References")]

    //[Space(10)]
    // RSO
    [SerializeField] RSO_KeyCount rsoKeyCount;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_SetKeyAmountRequire rseSetKeyCount;
    [SerializeField] RSE_DrawKeyCount rseDrawKeyCount;

    //[Header("Output")]

    private void OnEnable()
    {
        rseDrawKeyCount.Action += DrawKeyCount;
        rseSetKeyCount.Action += SetMaxKey;
    }
    private void OnDisable()
    {
        rseDrawKeyCount.Action -= DrawKeyCount;
        rseSetKeyCount.Action -= SetMaxKey;
    }

    private void Start()
    {
        rsoKeyCount.Value = 0;
    }

    void SetMaxKey(int max)
    {
        keyCountRequire = max;
    }
    void DrawKeyCount()
    {
        CustomConsoleWindow.ReceiveLog($"<color=yellow>Health: {rsoKeyCount.Value}/{keyCountRequire}</color>");
    }
}