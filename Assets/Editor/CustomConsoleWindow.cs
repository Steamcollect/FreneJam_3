using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class CustomConsoleWindow : EditorWindow
{
    private static List<string> logMessages = new List<string>();
    private Vector2 scrollPosition;
    private static int fontSize = 21;

    [MenuItem("Tools/Custom Console")]
    public static void ShowWindow()
    {
        GetWindow<CustomConsoleWindow>("Custom Console");
    }

    public static void ReceiveLog(string message)
    {
        logMessages.Add(message);
        if (logMessages.Count > 500)
            logMessages.RemoveAt(0);

        var window = GetWindow<CustomConsoleWindow>();
        if (window != null)
        {
            window.Repaint();
        }

        EditorApplication.ExecuteMenuItem("Window/General/Game");
    }

    public static void ClearLogs()
    {
        logMessages.Clear();

        var window = GetWindow<CustomConsoleWindow>();
        if (window != null)
        {
            window.Repaint();
        }

        EditorApplication.ExecuteMenuItem("Window/General/Game");
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear", GUILayout.Width(80)))
        {
            ClearLogs();
        }

        GUILayout.Space(5);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUIStyle style = new GUIStyle(EditorStyles.label)
        {
            richText = true,
            fontSize = fontSize,
            font = Font.CreateDynamicFontFromOSFont("Courier New", fontSize)
        };

        foreach (var message in logMessages)
        {
            GUILayout.Label(message, style);
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}