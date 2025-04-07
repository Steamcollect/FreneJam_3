using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class RythmeManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float margeDelay;
    [SerializeField] _Delay[] delays;
    int currentDelayIndex;
    bool isEven;

    [System.Serializable]
    public class _Delay
    {
        public float delay;
        public AudioClip clip;
    }

    [Header("References")]
    [SerializeField] AudioSource audioSource;

    [Space(10)]
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

        StartCoroutine(Utils.Delay(delays[currentDelayIndex].delay - margeDelay, () => { rseSetCanTakeDamage.Call(false); }));
        yield return new WaitForSeconds(delays[currentDelayIndex].delay);
        StartCoroutine(Utils.Delay(margeDelay, () => { rseSetCanTakeDamage.Call(true); }));

        audioSource.clip = delays[currentDelayIndex].clip;
        audioSource.Play();

        currentDelayIndex = (currentDelayIndex + 1) % delays.Length;
        isEven = !isEven;

        rsoRythmeIsEven.Value = isEven;

        rseDrawConsole.Call();

        delayCoroutine = StartCoroutine(Delay());
    }
}