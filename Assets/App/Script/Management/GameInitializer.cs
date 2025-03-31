using UnityEditor;
using UnityEngine;

public class GameInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void SpawnAtStartup()
    {
        GameObject myPrefab = Resources.Load<GameObject>("Scene");
        GameObject.Instantiate(myPrefab);

        EditorApplication.ExecuteMenuItem("Window/General/Game");
    }
}