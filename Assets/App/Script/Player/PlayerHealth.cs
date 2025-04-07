using System.Text;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int maxHealth;
    int currentHealh;

    bool canTakeDamage;

    [Header("References")]

    //[Space(10)]
    // RSO
    [SerializeField] RSO_GameState rsoGameState;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_DrawHealth rseDrawHealth;
    [SerializeField] RSE_SetCanTakeDamage rseSetCanTakeDamage;
    [SerializeField] RSE_TakeDamage rseTakeDamage;
    [SerializeField] RSE_StartGame rseStartGame;

    [Header("Output")]
    [SerializeField] RSF_IsPlayerOnDamageable rsfIsPlayerOnDamageable;

    private void OnEnable()
    {
        rseDrawHealth.Action += DrawHealth;
        rseSetCanTakeDamage.Action += SetCanTakeDamage;
        rseTakeDamage.Action += TakeDamage;
        rseStartGame.Action += OnStart;
    }
    private void OnDisable()
    {
        rseDrawHealth.Action -= DrawHealth;
        rseSetCanTakeDamage.Action -= SetCanTakeDamage;
        rseTakeDamage.Action -= TakeDamage;
        rseStartGame.Action -= OnStart;
    }

    private void OnStart()
    {
        currentHealh = maxHealth;
    }

    void TakeDamage()
    {
        if (!canTakeDamage) return;
        currentHealh--;
        if(currentHealh <= 0 && rsoGameState.Value != GameState.Win)
        {
            rsoGameState.Value = GameState.Lose;
        }
    }

    void DrawHealth()
    {
        CustomConsoleWindow.ReceiveLog($"<color=white>Health: {currentHealh}/{maxHealth}</color>");
    }
    void SetCanTakeDamage( bool canTakeDamage)
    {
        this.canTakeDamage = canTakeDamage;

        if(canTakeDamage && rsfIsPlayerOnDamageable.Call())
        {
            TakeDamage();
        }
    }
}