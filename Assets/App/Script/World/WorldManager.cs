using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    //[Header("Settings")]
    Vector2Int playerRoomPos;

    //[Header("References")]
    Dictionary<Vector2Int, RoomData> rooms = new Dictionary<Vector2Int, RoomData>();

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSF_PlayerTryMovement rsfPlayerTryMovement;
    [SerializeField] RSE_OnPlayerMove rseOnPlayerMove;

    [SerializeField] RSE_OnDungeonCreated rseOnDungeonCreated;

    [Header("Output")]
    [SerializeField] RSE_SetPlayerPos rseSetPlayerPos;

    private void OnEnable()
    {
        rseOnDungeonCreated.Action += OnDungeonCreated;
        rsfPlayerTryMovement.Action += TryMove;
        rseOnPlayerMove.Action += DrawRoom;
    }
    private void OnDisable()
    {
        rseOnDungeonCreated.Action -= OnDungeonCreated;
        rsfPlayerTryMovement.Action -= TryMove;
        rseOnPlayerMove.Action -= DrawRoom;
    }

    void OnDungeonCreated(Dictionary<Vector2Int, RoomData> rooms)
    {
        this.rooms = rooms;
        rseSetPlayerPos.Call(rooms[Vector2Int.zero].size / 2);
        DrawRoom(rooms[Vector2Int.zero].size / 2);
    }

    void DrawDungeon()
    {
        if (rooms == null || rooms.Count == 0)
        {
            CustomConsoleWindow.ReceiveLog("<color=red>Pas de salle à afficher !</color>");
            return;
        }

        var positions = rooms.Keys.ToArray();
        int minX = positions.Min(p => p.x);
        int maxX = positions.Max(p => p.x);
        int minY = positions.Min(p => p.y);
        int maxY = positions.Max(p => p.y);

        int gridWidth = (maxX - minX + 1) * 2 - 1;
        int gridHeight = (maxY - minY + 1) * 2 - 1;

        string[,] map = new string[gridWidth, gridHeight];

        // Initialisation
        for (int y = 0; y < gridHeight; y++)
            for (int x = 0; x < gridWidth; x++)
                map[x, y] = "   ";

        // Positionner les salles
        Dictionary<Vector2Int, Vector2Int> gridPosByRoom = new();

        foreach (var kvp in rooms)
        {
            Vector2Int localPos = new Vector2Int((kvp.Key.x - minX) * 2, (kvp.Key.y - minY) * 2);
            gridPosByRoom[kvp.Key] = localPos;

            map[localPos.x, localPos.y] = (kvp.Key == Vector2Int.zero) ? "<color=cyan>[S]</color>" : "[X]";
        }

        // Dessiner les connexions
        foreach (var room in rooms.Values)
        {
            Vector2Int from = gridPosByRoom[room.position];

            foreach (var connectedRoom in room.roomConnected)
            {
                Vector2Int to = gridPosByRoom[connectedRoom.position];

                Vector2Int linkPos = (from + to) / 2;

                if (from.x == to.x)
                    map[linkPos.x, linkPos.y] = " | ";
                else if (from.y == to.y)
                    map[linkPos.x, linkPos.y] = "---";
            }
        }

        // Construction du string
        StringBuilder sb = new StringBuilder();

        for (int y = gridHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                sb.Append(map[x, y]);
            }
            sb.AppendLine();
        }

        CustomConsoleWindow.ReceiveLog(sb.ToString());
    }
    void DrawRoom(Vector2Int playerPos)
    {
        CustomConsoleWindow.ClearLogs();

        StringBuilder sb = new StringBuilder();

        sb.AppendLine();
        for (int y = rooms[playerRoomPos].size.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < rooms[playerRoomPos].size.x; x++)
            {
                if (playerPos.x == x && playerPos.y == y)
                    sb.Append($"<color=white>[P]</color>");
                else
                    sb.Append("[0]");
            }
            sb.AppendLine();
        }

        CustomConsoleWindow.ReceiveLog(sb.ToString());
        DrawDungeon();
    }

    bool TryMove(Vector2Int position)
    {
        if (position.x < rooms[playerRoomPos].size.x && position.x >= 0 && position.y < rooms[playerRoomPos].size.y && position.y >= 0)
            return true;
        else
            return false;
    }
}