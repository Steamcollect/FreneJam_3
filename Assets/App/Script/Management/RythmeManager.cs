using System.Collections;
using UnityEngine;

public class RythmeManager : MonoBehaviour
{
    [Header("Settings")]
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

    private void Start()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delays[currentDelayIndex]);
        currentDelayIndex = (currentDelayIndex + 1) % delays.Length;
        isEven = !isEven;

        rsoRythmeIsEven.Value = isEven;

        rseDrawConsole.Call();

        StartCoroutine(Delay());
    }
}