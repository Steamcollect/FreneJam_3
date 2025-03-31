using System.Collections.Generic;
using System.Linq;
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

    [Header("PropsRef")]
    [SerializeField] RSE_OnDoorEnter rseOnDoorEnter;

    private void Start()
    {
        RoomData startingRoom = 
            new RoomData(
                Vector2Int.zero, 
                new Vector2Int(Random.Range(minMaxRoomSize.x, minMaxRoomSize.y), 
                Random.Range(minMaxRoomSize.x, minMaxRoomSize.y)));
        rooms.Add(Vector2Int.zero, startingRoom);
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
        else
        {
            AddTriggerables();
            rseOnDungeonCreated.Call(rooms);
        }
    }

    void TryCreateRoom(RoomData creator, Vector2Int direction)
    {
        if (currentRoomAmount >= maxRoomAmount) return;

        if (!rooms.ContainsKey(creator.position + direction))
        {
            nextRoomsToCheck.Add(CreateRoom(creator, creator.position + direction));
            currentRoomAmount++;
        }
        else if(!creator.roomConnected.Any(x => x.position == creator.position + direction) && Random.value < .4f)
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

    void AddTriggerables()
    {
        foreach (var room in rooms.Values)
        {
            print(room.roomConnected.Count);
            foreach (var roomConnected in room.roomConnected)
            {
                Door door = new Door();
                door.charType = 'D';
                door.rseOnDoorEnter = rseOnDoorEnter;
                Vector2Int doorPos = room.position;

                if (roomConnected.position == room.position + Vector2Int.up) // up
                    doorPos = new Vector2Int(Random.Range(1, room.size.x - 1), room.size.y);
                if (roomConnected.position == room.position + Vector2Int.down) // down
                    doorPos = new Vector2Int(Random.Range(1, room.size.x - 1), 0);

                if (roomConnected.position == room.position + Vector2Int.left) // left
                    doorPos = new Vector2Int(0, Random.Range(1, room.size.y - 1));
                if (roomConnected.position == room.position + Vector2Int.right) // right
                    doorPos = new Vector2Int(room.size.x, Random.Range(1, room.size.y - 1));

                door.position = doorPos;
                room.triggerables.Add(doorPos, door);
            }
        }
    }
}