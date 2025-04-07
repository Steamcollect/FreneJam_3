
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.Audio;

public class RythmeTimelineEditor : EditorWindow
{
    private RythmeManager targetManager;
    private SerializedObject serializedObject;
    private SerializedProperty delaysProperty;

    private Vector2 scrollPos;
    private float pixelsPerSecond = 150f;
    private float timelineHeight = 200f;
    private int draggingIndex = -1;
    private Vector2 dragOffset;
    private bool isDraggingHandle = false;

    private bool isDraggingPlayhead = false;
    private float[] keyframeTargetPositions;

    private bool isPlaying = false;
    private float playbackTime = 0f;
    private double lastTime;
    private int lastPlayedIndex = -1;

    [MenuItem("Tools/Rythme Timeline Editor")]
    public static void ShowWindow()
    {
        GetWindow<RythmeTimelineEditor>("Rythme Timeline");
    }

    private void OnEnable()
    {
        lastTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        if (!isPlaying) return;

        double currentTime = EditorApplication.timeSinceStartup;
        float deltaTime = (float)(currentTime - lastTime);
        lastTime = currentTime;
        playbackTime += deltaTime;

        float totalTime = 0f;
        for (int i = 0; i < delaysProperty.arraySize; i++)
        {
            totalTime += delaysProperty.GetArrayElementAtIndex(i).FindPropertyRelative("delay").floatValue;
        }

        if (playbackTime > totalTime)
        {
            playbackTime = 0f;
            lastPlayedIndex = -1;
        }

        float currentMarker = 0f;
        for (int i = 0; i < delaysProperty.arraySize; i++)
        {
            float delay = delaysProperty.GetArrayElementAtIndex(i).FindPropertyRelative("delay").floatValue;
            float nextMarker = currentMarker + delay;

            if (playbackTime >= currentMarker && playbackTime < nextMarker)
            {
                if (lastPlayedIndex != i)
                {
                    SerializedProperty clipProp = delaysProperty.GetArrayElementAtIndex(i).FindPropertyRelative("clip");
                    AudioClip clip = clipProp.objectReferenceValue as AudioClip;
                    if (clip != null) PlayClip(clip);
                    lastPlayedIndex = i;
                }
                break;
            }
            currentMarker = nextMarker;
        }

        Repaint();
    }

    private void OnGUI()
    {
        Event evt = Event.current;
        EditorGUILayout.Space();
        targetManager = EditorGUILayout.ObjectField("Rythme Manager", targetManager, typeof(RythmeManager), true) as RythmeManager;

        if (targetManager == null)
        {
            EditorGUILayout.HelpBox("Assigne un RythmeManager pour éditer les delays.", MessageType.Info);
            return;
        }

        serializedObject = new SerializedObject(targetManager);
        delaysProperty = serializedObject.FindProperty("delays");

        if (keyframeTargetPositions == null || keyframeTargetPositions.Length != delaysProperty.arraySize)
        {
            keyframeTargetPositions = new float[delaysProperty.arraySize];
        }

        if (evt.type == EventType.ScrollWheel)
        {
            float zoomDelta = -evt.delta.y * 0.05f;
            pixelsPerSecond = Mathf.Clamp(pixelsPerSecond + zoomDelta * pixelsPerSecond, 30f, 800f);
            evt.Use();
        }

        float totalTime = 0f;
        for (int i = 0; i < delaysProperty.arraySize; i++)
        {
            totalTime += delaysProperty.GetArrayElementAtIndex(i).FindPropertyRelative("delay").floatValue;
        }

        float timelineWidth = Mathf.Max(totalTime * pixelsPerSecond + 400, position.width);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Timeline", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(isPlaying ? "Stop" : "Play"))
        {
            isPlaying = !isPlaying;
            lastTime = EditorApplication.timeSinceStartup;
            if (!isPlaying) Repaint();
        }

        GUILayout.Label($"Time: {playbackTime:F2}s", GUILayout.Width(100));
        GUILayout.EndHorizontal();

        if (!isPlaying && !isDraggingPlayhead)
        {
            playbackTime = EditorGUILayout.Slider("Playhead Position", playbackTime, 0f, totalTime);
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(timelineHeight + 60));
        Rect outerRect = GUILayoutUtility.GetRect(timelineWidth + 10, timelineHeight + 10);
        Rect timelineRect = new Rect(outerRect.x + 5, outerRect.y + 5, outerRect.width - 10, outerRect.height - 10);

        EditorGUI.DrawRect(outerRect, new Color(0.2f, 0.2f, 0.2f));
        EditorGUI.DrawRect(timelineRect, new Color(0.3f, 0.3f, 0.3f));

        float playheadX = timelineRect.x + playbackTime * pixelsPerSecond;
        Rect playhead = new Rect(playheadX, timelineRect.y, 2, timelineRect.height);
        Rect playheadHandle = new Rect(playheadX - 5, timelineRect.y - 10, 10, 10);

        EditorGUI.DrawRect(playhead, Color.red);
        EditorGUI.DrawRect(playheadHandle, Color.yellow);

        // Drag du playhead
        switch (evt.type)
        {
            case EventType.MouseDown:
                if (playheadHandle.Contains(evt.mousePosition))
                {
                    isDraggingPlayhead = true;
                    evt.Use();
                }
                break;

            case EventType.MouseDrag:
                if (isDraggingPlayhead)
                {
                    float relativeMouseX = evt.mousePosition.x - timelineRect.x;
                    playbackTime = relativeMouseX / pixelsPerSecond;
                    playbackTime = Mathf.Clamp(playbackTime, 0f, totalTime);
                    evt.Use();
                }
                break;

            case EventType.MouseUp:
                if (isDraggingPlayhead)
                {
                    isDraggingPlayhead = false;
                    evt.Use();
                }
                break;
        }

        float currentX = timelineRect.x;
        float yStart = timelineRect.y + 40;

        for (int i = 0; i < delaysProperty.arraySize; i++)
        {
            SerializedProperty element = delaysProperty.GetArrayElementAtIndex(i);
            SerializedProperty delayProp = element.FindPropertyRelative("delay");
            SerializedProperty clipProp = element.FindPropertyRelative("clip");

            float delay = delayProp.floatValue;
            float width = Mathf.Max(60f, delay * pixelsPerSecond);
            float height = 100f;

            // Animation vers la nouvelle position
            float targetX = currentX;
            float oldX = keyframeTargetPositions[i];
            keyframeTargetPositions[i] = Mathf.Lerp(oldX, targetX, 0.2f);

            Rect keyRect = new Rect(keyframeTargetPositions[i], yStart, width, height);
            Rect handleRect = new Rect(keyRect.x, keyRect.y - 18, width, 15);

            Color keyColor = (draggingIndex == i && isDraggingHandle) ? new Color(1f, 0.8f, 0.3f) : new Color(0.85f, 0.85f, 0.85f);
            Color handleColor = (draggingIndex == i && isDraggingHandle) ? new Color(1f, 0.5f, 0f) : new Color(0.2f, 0.2f, 0.2f);

            EditorGUI.DrawRect(keyRect, keyColor);
            EditorGUI.DrawRect(handleRect, handleColor);
            GUI.Label(handleRect, "", EditorStyles.helpBox); // contour de la poignée

            HandleDragReorder(evt, i, handleRect);

            GUILayout.BeginArea(new Rect(keyRect.x + 4, keyRect.y + 4, keyRect.width - 8, keyRect.height - 8));

            if (width < 140f)
            {
                GUILayout.Label(i.ToString(), EditorStyles.boldLabel);
                delayProp.floatValue = EditorGUILayout.FloatField(delayProp.floatValue);
                clipProp.objectReferenceValue = EditorGUILayout.ObjectField(clipProp.objectReferenceValue, typeof(AudioClip), false);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("<-")) delaysProperty.MoveArrayElement(i, Mathf.Max(0, i - 1));
                if (GUILayout.Button("->")) delaysProperty.MoveArrayElement(i, Mathf.Min(delaysProperty.arraySize - 1, i + 1));
                if (GUILayout.Button("X")) { delaysProperty.DeleteArrayElementAtIndex(i); GUILayout.EndHorizontal(); GUILayout.EndArea(); break; }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Label(i.ToString(), EditorStyles.boldLabel);
                delayProp.floatValue = EditorGUILayout.FloatField(delayProp.floatValue);
                clipProp.objectReferenceValue = EditorGUILayout.ObjectField(clipProp.objectReferenceValue, typeof(AudioClip), false);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("<-")) delaysProperty.MoveArrayElement(i, Mathf.Max(0, i - 1));
                if (GUILayout.Button("->")) delaysProperty.MoveArrayElement(i, Mathf.Min(delaysProperty.arraySize - 1, i + 1));
                if (GUILayout.Button("X")) { delaysProperty.DeleteArrayElementAtIndex(i); GUILayout.EndHorizontal(); GUILayout.EndVertical(); GUILayout.EndArea(); break; }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            GUILayout.EndArea();

            HandleDragDrop(keyRect, clipProp);

            currentX += delay * pixelsPerSecond;
        }

        HandleGlobalTimelineDrop(timelineRect);
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Delay"))
        {
            delaysProperty.InsertArrayElementAtIndex(delaysProperty.arraySize);
            var newElement = delaysProperty.GetArrayElementAtIndex(delaysProperty.arraySize - 1);
            newElement.FindPropertyRelative("delay").floatValue = 1f;
            newElement.FindPropertyRelative("clip").objectReferenceValue = null;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void HandleDragReorder(Event evt, int index, Rect handleRect)
    {
        switch (evt.type)
        {
            case EventType.MouseDown:
                if (handleRect.Contains(evt.mousePosition))
                {
                    if (evt.button == 2) // clic molette = delete
                    {
                        delaysProperty.DeleteArrayElementAtIndex(index);
                        evt.Use();
                        return;
                    }

                    isDraggingHandle = true;
                    draggingIndex = index;
                    evt.Use();
                }
                break;

            case EventType.MouseDrag:
                if (isDraggingHandle && draggingIndex == index)
                {
                    float targetX = evt.mousePosition.x + scrollPos.x;
                    int targetIndex = GetTargetIndexFromPosition(targetX);

                    if (targetIndex != index && targetIndex >= 0 && targetIndex < delaysProperty.arraySize)
                    {
                        delaysProperty.MoveArrayElement(index, targetIndex);
                        draggingIndex = targetIndex;
                    }

                    evt.Use();
                }
                break;

            case EventType.MouseUp:
                if (isDraggingHandle && draggingIndex == index)
                {
                    isDraggingHandle = false;
                    draggingIndex = -1;
                    evt.Use();
                }
                break;
        }
    }

    private int GetTargetIndexFromPosition(float xPos)
    {
        float total = 0f;
        for (int i = 0; i < delaysProperty.arraySize; i++)
        {
            float d = delaysProperty.GetArrayElementAtIndex(i).FindPropertyRelative("delay").floatValue;
            float segmentStart = total * pixelsPerSecond;
            float segmentEnd = segmentStart + d * pixelsPerSecond;

            if (xPos < segmentEnd)
                return i;

            total += d;
        }
        return delaysProperty.arraySize - 1;
    }

    private void HandleDragDrop(Rect rect, SerializedProperty clipProp)
    {
        Event evt = Event.current;
        if (rect.Contains(evt.mousePosition))
        {
            if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
            {
                if (DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences[0] is AudioClip)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        clipProp.objectReferenceValue = DragAndDrop.objectReferences[0];
                    }
                    evt.Use();
                }
            }
        }
    }

    private void HandleGlobalTimelineDrop(Rect timelineRect)
    {
        Event evt = Event.current;
        if ((evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform) &&
            DragAndDrop.objectReferences.Length > 0 &&
            DragAndDrop.objectReferences[0] is AudioClip &&
            timelineRect.Contains(evt.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                AudioClip droppedClip = DragAndDrop.objectReferences[0] as AudioClip;

                delaysProperty.InsertArrayElementAtIndex(delaysProperty.arraySize);
                var newElement = delaysProperty.GetArrayElementAtIndex(delaysProperty.arraySize - 1);
                newElement.FindPropertyRelative("delay").floatValue = 1f;
                newElement.FindPropertyRelative("clip").objectReferenceValue = droppedClip;

                evt.Use();
            }
        }
    }

    public static void PlayClip(AudioClip clip)
    {
    }
}