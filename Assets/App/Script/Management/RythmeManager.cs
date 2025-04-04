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
    // RSF
    // RSP

    //[Header("Input")]
    [Header("Output")]
    [SerializeField] RSE_DrawConsole rseDrawConsole;
    [SerializeField] RSE_SetCanTakeDamage rseSetCanTakeDamage;

    private void Start()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        StartCoroutine(Utils.Delay(delays[currentDelayIndex] - margeDelay, () => { rseSetCanTakeDamage.Call(false); }));
        yield return new WaitForSeconds(delays[currentDelayIndex]);
        StartCoroutine(Utils.Delay(margeDelay, () => { rseSetCanTakeDamage.Call(true); }));

        currentDelayIndex = (currentDelayIndex + 1) % delays.Length;
        isEven = !isEven;

        rsoRythmeIsEven.Value = isEven;

        rseDrawConsole.Call();

        StartCoroutine(Delay());
    }
}