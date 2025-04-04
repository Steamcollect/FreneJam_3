using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomData
{
    public List<RoomData> roomConnected = new();
    public Vector2Int position;
    public bool isVisited = false;

    public Vector2Int size;

    public Dictionary<Vector2Int, Door> doors = new();
    public Dictionary<Vector2Int, Key> keys = new();
    public Dictionary<Vector2Int, Triggerable> triggerables = new();

    public RoomData(Vector2Int position, Vector2Int size)
    {
        this.position = position;
        this.size = size;
    }
}