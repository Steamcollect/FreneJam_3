using UnityEngine;

public abstract class Triggerable
{
    public Vector2Int position;
    public char charType;

    public abstract void OnTriggerEnter();
}

public enum TriggerableType
{
    Door,
}