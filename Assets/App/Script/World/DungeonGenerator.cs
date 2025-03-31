using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int maxRoomAmount;
    int currentRoomAmount;

    [SerializeField] Vector2Int minMaxRoomSize = new Vector2Int(3, 6);

    List<RoomData> roomsToCheck = new();
    List<RoomData> nextRoomsToCheck = new();

    //[System.Serializable]

    Dictionary<Vector2Int, RoomData> rooms = new();

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    [Header("Output")]
    [SerializeField] RSE_OnDungeonCreated rseOnDungeonCreated;

    private void Start()
    {
        RoomData startingRoom = new RoomData(Vector2Int.zero, new Vector2Int(Random.Range(minMaxRoomSize.x, minMaxRoomSize.y), Random.Range(minMaxRoomSize.x, minMaxRoomSize.y)));
        roomsToCheck.Add(startingRoom);

        CheckRooms();
    }

    void CheckRooms()
    {
        foreach (var room in roomsToCheck)
        {
            if (Random.value < .5f) TryCreateRoom(room, Vector2Int.up);
            if (Random.value < .5f) TryCreateRoom(room, Vector2Int.down);
            if (Random.value < .5f) TryCreateRoom(room, Vector2Int.left);
            if (Random.value < .5f) TryCreateRoom(room, Vector2Int.right);
        }

        if(currentRoomAmount < maxRoomAmount && nextRoomsToCheck.Count == 0)
            roomsToCheck = new List<RoomData>(rooms.Values);
        else
            roomsToCheck = new List<RoomData>(nextRoomsToCheck);

        nextRoomsToCheck.Clear();

        if (roomsToCheck.Count > 0) CheckRooms();
        else rseOnDungeonCreated.Call(rooms);
    }

    void TryCreateRoom(RoomData creator, Vector2Int direction)
    {
        if (currentRoomAmount >= maxRoomAmount) return;

        if (!rooms.ContainsKey(creator.position + direction))
        {
            nextRoomsToCheck.Add(CreateRoom(creator, creator.position + direction));
            currentRoomAmount++;
        }
        else if(Random.value < .5f)
        {
            creator.roomConnected.Add(rooms[creator.position + direction]);
            rooms[creator.position + direction].roomConnected.Add(creator);
        }
    }
    RoomData CreateRoom(RoomData creator, Vector2Int position)
    {
        RoomData newRoom = new RoomData(position, new Vector2Int(Random.Range(minMaxRoomSize.x, minMaxRoomSize.y), Random.Range(minMaxRoomSize.x, minMaxRoomSize.y)));
        creator.roomConnected.Add(newRoom);
        newRoom.roomConnected.Add(creator);

        rooms.Add(position, newRoom);

        return newRoom;
    }
}