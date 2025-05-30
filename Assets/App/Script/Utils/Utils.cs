using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public static class Utils
{
    
    #region IENUMERABLE
    public static T GetRandom<T>(this IEnumerable<T> elems)
    {
        if (elems.Count() == 0)
        {
            Debug.LogError("Try to get random elem from empty IEnumerable");
        }
        return elems.ElementAt(new System.Random().Next(0, elems.Count()));
    }
    #endregion

    #region COROUTINE
    public static IEnumerator Delay(float delay, Action ev)
    {
        yield return new WaitForSeconds(delay);
        ev?.Invoke();
    }
    #endregion

    public static void ClearLogConsole()
    {
#if UNITY_EDITOR
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.SceneView));

        System.Type type = assembly.GetType("UnityEditor.LogEntries");
        System.Reflection.MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
#endif
    }
}