using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int maxRoomAmount;
    int currentRoomAmount;

    [Space(5)]
    [SerializeField] Vector2Int minMaxRoomSize = new Vector2Int(3, 6);

    List<RoomData> roomsToCheck = new();
    List<RoomData> nextRoomsToCheck = new();

    [Space(5)]
    [SerializeField] Vector2Int minMaxKeyCount;

    //[System.Serializable]

    Dictionary<Vector2Int, RoomData> rooms = new();

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_StartGame rseStartGame;

    [Header("Output")]
    [SerializeField] RSE_OnDungeonCreated rseOnDungeonCreated;
    [SerializeField] RSE_SetKeyAmountRequire rseSetKeyRequire;

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
        rooms = new();
        currentRoomAmount = 0;
        currentLoop = 0;
        roomsToCheck.Clear();
        nextRoomsToCheck.Clear();

        RoomData startingRoom = 
            new RoomData(
                Vector2Int.zero, 
                new Vector2Int(Random.Range(minMaxRoomSize.x, minMaxRoomSize.y), 
                Random.Range(minMaxRoomSize.x, minMaxRoomSize.y)));
        startingRoom.isVisited = true;

        rooms.Add(Vector2Int.zero, startingRoom);
        roomsToCheck.Add(startingRoom);

        CheckRooms();
    }

    int currentLoop;
    void CheckRooms()
    {
        currentLoop++;
        if(currentLoop >= 100)
        {
            Debug.LogError("To many try to create the dungeon");
            return;
        }

        foreach (var room in roomsToCheck)
        {
            if (Random.value < .5f) TryCreateRoom(room, Vector2Int.up);
            if (Random.value < .5f) TryCreateRoom(room, Vector2Int.down);
            if (Random.value < .5f) TryCreateRoom(room, Vector2Int.left);
            if (Random.value < .5f) TryCreateRoom(room, Vector2Int.right);
        }

        if(currentRoomAmount < maxRoomAmount && nextRoomsToCheck.Count == 0)
        {
            roomsToCheck = new List<RoomData>(rooms.Values);
            print("Not enough room... Trying to expend it");
        }
        else
        {
            roomsToCheck = new List<RoomData>(nextRoomsToCheck);
        }
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
            RoomData room = CreateRoom(creator, creator.position + direction);
            nextRoomsToCheck.Add(room);
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
        // Set triggerables
        foreach (var room in rooms.Values)
        {
            foreach (var roomConnected in room.roomConnected)
            {
                Door door = new Door();
                door.charType = 'D';
                Vector2Int doorPos = Vector2Int.zero;

                if (roomConnected.position == room.position + Vector2Int.up)
                {
                    door.positionType = DoorPositionType.Up;
                    doorPos = new Vector2Int(Random.Range(1, room.size.x - 1), room.size.y); // +1 à Y pour sortir
                }

                if (roomConnected.position == room.position + Vector2Int.down)
                {
                    door.positionType = DoorPositionType.Down;
                    doorPos = new Vector2Int(Random.Range(1, room.size.x - 1), -1); // -1 à Y pour sortir
                }

                if (roomConnected.position == room.position + Vector2Int.left)
                {
                    door.positionType = DoorPositionType.Left;
                    doorPos = new Vector2Int(-1, Random.Range(1, room.size.y - 1)); // -1 à X pour sortir
                }

                if (roomConnected.position == room.position + Vector2Int.right)
                {
                    door.positionType = DoorPositionType.Right;
                    doorPos = new Vector2Int(room.size.x, Random.Range(1, room.size.y - 1)); // +1 à X pour sortir
                }

                door.position = doorPos;
                door.roomConnected = roomConnected.position;

                room.doors.Add(doorPos, door);
                room.triggerables.Add(doorPos, door);
            }
        }

        // Set door connections
        foreach (RoomData room in rooms.Values)
        {
            foreach (Door door in room.doors.Values)
            {
                DoorPositionType doorConnectedPosType = DoorPositionType.Up;
                switch (door.positionType)
                {
                    case DoorPositionType.Up:
                        doorConnectedPosType = DoorPositionType.Down;
                        break;
                    case DoorPositionType.Down:
                        doorConnectedPosType = DoorPositionType.Up;
                        break;
                    case DoorPositionType.Left:
                        doorConnectedPosType = DoorPositionType.Right;
                        break;
                    case DoorPositionType.Right:
                        doorConnectedPosType = DoorPositionType.Left;
                        break;
                }

                Door doorConnected = GetDoorWithPosition(rooms[door.roomConnected], doorConnectedPosType);
                door.doorConnected = doorConnected.position;

                Door GetDoorWithPosition(RoomData room, DoorPositionType positionType)
                {
                    foreach (var door in room.doors.Values)
                    {
                        if (door.positionType == positionType) return door;
                    }
                    return null;
                }
            }
        }

        // Ajouter des clés dans des salles aléatoires
        int keyCount = Random.Range(minMaxKeyCount.x, minMaxKeyCount.y + 1);
        List<RoomData> availableRooms = rooms.Values.ToList();
        rseSetKeyRequire.Call(keyCount);

        for (int i = 0; i < keyCount && availableRooms.Count > 0; i++)
        {
            // Choisir une salle aléatoire
            int index = Random.Range(0, availableRooms.Count);
            RoomData room = availableRooms[index];
            availableRooms.RemoveAt(index); // éviter les doublons (optionnel)

            // Choisir une position aléatoire dans la salle (intérieure uniquement)
            Vector2Int pos = new Vector2Int(Random.Range(0, room.size.x), Random.Range(0, room.size.y));

            // Si la position est déjà occupée, on cherche une autre
            int safety = 0;
            while ((room.triggerables.ContainsKey(pos) || room.doors.ContainsKey(pos)) && safety < 20)
            {
                pos = new Vector2Int(Random.Range(0, room.size.x), Random.Range(0, room.size.y));
                safety++;
            }

            // Créer une clé
            Key key = new();
            key.charType = 'K';
            key.position = pos;

            room.triggerables[pos] = key;
            room.keys.Add(pos, key);
        }
    }
}