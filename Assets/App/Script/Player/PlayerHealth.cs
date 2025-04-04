using System.Text;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int maxHealth;
    int currentHealh;

    bool canTakeDamage;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_DrawHealth rseDrawHealth;
    [SerializeField] RSE_SetCanTakeDamage rseSetCanTakeDamage;
    [SerializeField] RSE_TakeDamage rseTakeDamage;

    [Header("Output")]
    [SerializeField] RSF_IsPlayerOnDamageable rsfIsPlayerOnDamageable;

    private void OnEnable()
    {
        rseDrawHealth.Action += DrawHealth;
        rseSetCanTakeDamage.Action += SetCanTakeDamage;
        rseTakeDamage.Action += TakeDamage;
    }
    private void OnDisable()
    {
        rseDrawHealth.Action -= DrawHealth;
        rseSetCanTakeDamage.Action -= SetCanTakeDamage;
        rseTakeDamage.Action -= TakeDamage;
    }

    private void Start()
    {
        currentHealh = maxHealth;
    }

    void TakeDamage()
    {
        currentHealh--;
    }

    void DrawHealth()
    {
        CustomConsoleWindow.ReceiveLog($"Health: {currentHealh}/{maxHealth}");
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