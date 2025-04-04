using System.Text;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int maxHealth;
    int currentHealh;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_DrawHealth rseDrawHealth;

    //[Header("Output")]

    private void OnEnable()
    {
        rseDrawHealth.Action += DrawHealth;
    }
    private void OnDisable()
    {
        rseDrawHealth.Action -= DrawHealth;
    }

    private void Start()
    {
        currentHealh = maxHealth;
    }

    void DrawHealth()
    {
        CustomConsoleWindow.ReceiveLog($"Health: {currentHealh}/{maxHealth}");
    }
}