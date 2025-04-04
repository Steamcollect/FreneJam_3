using UnityEngine;

public class Door : Triggerable
{
    public Vector2Int roomConnected;
    public Vector2Int doorConnected;

    public DoorPositionType positionType;

    public Vector2Int GetRoomConnected(){return roomConnected;}
    public Vector2Int GetDoorConnectedPos(){return doorConnected;}
}

public enum DoorPositionType
{
    Up,
    Down,
    Left,
    Right
}