using System.Collections;
using UnityEngine;

public class RythmeManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float margeDelay;
    [SerializeField] float[] delays;
    int currentDelayIndex;
    bool isEven;

    [Header("References")]

    //[Space(10)]
    // RSO
    [SerializeField] RSO_RythmeIsEven rsoRythmeIsEven;
    [SerializeField] RSO_GameState rsoGameState;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_StartGame rseStartGame;

    [Header("Output")]
    [SerializeField] RSE_DrawConsole rseDrawConsole;
    [SerializeField] RSE_SetCanTakeDamage rseSetCanTakeDamage;

    private void OnEnable()
    {
        rseStartGame.Action += OnStart;
    }
    private void OnDisable()
    {
        rseStartGame.Action -= OnStart;
    }

    private void OnStart()
    {
        if(delayCoroutine != null) StopCoroutine(delayCoroutine);
        delayCoroutine = StartCoroutine(Delay());
    }

    Coroutine delayCoroutine;
    IEnumerator Delay()
    {
        while(rsoGameState.Value != GameState.InGame)
        {
            yield return null;
        }

        StartCoroutine(Utils.Delay(delays[currentDelayIndex] - margeDelay, () => { rseSetCanTakeDamage.Call(false); }));
        yield return new WaitForSeconds(delays[currentDelayIndex]);
        StartCoroutine(Utils.Delay(margeDelay, () => { rseSetCanTakeDamage.Call(true); }));

        currentDelayIndex = (currentDelayIndex + 1) % delays.Length;
        isEven = !isEven;

        rsoRythmeIsEven.Value = isEven;

        rseDrawConsole.Call();

        delayCoroutine = StartCoroutine(Delay());
    }
}