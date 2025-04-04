using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    //[Header("Settings")]
    Vector2Int playerStartingPos;
    Vector2Int playerRoomPos;

    [Header("References")]
    Dictionary<Vector2Int, RoomData> rooms = new Dictionary<Vector2Int, RoomData>();

    //[Space(10)]
    // RSO
    [SerializeField] RSO_PlayerPos rsoPlayerPos;
    [SerializeField] RSO_RythmeIsEven rsoRythmeIsEven;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSF_PlayerTryMovement rsfPlayerTryMovement;
    [SerializeField] RSE_OnPlayerMove rseOnPlayerMove;

    [SerializeField] RSE_OnDungeonCreated rseOnDungeonCreated;

    [Space(5)]
    [SerializeField] RSE_DrawPlayerRoom rseDrawPlayerRoom;
    [SerializeField] RSE_DrawDungeon rseDrawDungeon;

    [Header("Output")]
    [SerializeField] RSE_DrawConsole rseDrawConsole;

    private void OnEnable()
    {
        rseOnDungeonCreated.Action += OnDungeonCreated;
        rsfPlayerTryMovement.Action += TryMove;
        rseOnPlayerMove.Action += OnPlayerMove;

        rseDrawPlayerRoom.Action += DrawRoom;
        rseDrawDungeon.Action += DrawDungeon;
    }
    private void OnDisable()
    {
        rseOnDungeonCreated.Action -= OnDungeonCreated;
        rsfPlayerTryMovement.Action -= TryMove;
        rseOnPlayerMove.Action -= OnPlayerMove;
    }

    void OnDungeonCreated(Dictionary<Vector2Int, RoomData> rooms)
    {
        this.rooms = rooms;
        playerStartingPos = rooms[Vector2Int.zero].size / 2;
        rsoPlayerPos.Value = playerStartingPos;
        rseDrawConsole.Call();
    }

    void DrawRoom()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine();

        RoomData room = rooms[playerRoomPos];

        // Étendre la grille de -1 à size+1 pour inclure les portes
        for (int y = room.size.y; y >= -1; y--)
        {
            for (int x = -1; x <= room.size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (rsoPlayerPos.Value == pos)
                {
                    sb.Append("<color=cyan>[P]</color>");
                }
                else if (room.triggerables.ContainsKey(pos))
                {
                    char type = room.triggerables[pos].charType;
                    string color = (type == 'D') ? "yellow" : "white";
                    sb.Append($"<color={color}>[{type}]</color>");
                }
                else if (x < 0 || x >= room.size.x || y < 0 || y >= room.size.y)
                {
                    // Extérieur de la salle, affiche un mur vide ou espace
                    sb.Append("   ");
                }
                else
                {
                    // Grille normale
                    if (rsoRythmeIsEven.Value)
                    {
                        sb.Append((x + y) % 2 != 0 ? "<color=red>[X]</color>" : "[0]");
                    }
                    else
                    {
                        sb.Append((x + y) % 2 == 0 ? "<color=red>[X]</color>" : "[0]");
                    }
                }
            }
            sb.AppendLine();
        }

        CustomConsoleWindow.ReceiveLog(sb.ToString());
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
            
            string roomStr = "   ";
            if (kvp.Key == playerRoomPos) roomStr = "<color=cyan>[P]</color>";
            else if (rooms[kvp.Key].isVisited) roomStr = "[X]";

            map[localPos.x, localPos.y] = roomStr;
        }

        // Dessiner les connexions
        foreach (var room in rooms.Values)
        {
            Vector2Int from = gridPosByRoom[room.position];

            foreach (var connectedRoom in room.roomConnected)
            {
                Vector2Int to = gridPosByRoom[connectedRoom.position];
                Vector2Int linkPos = (from + to) / 2;

                if (!room.isVisited && !connectedRoom.isVisited) map[linkPos.x, linkPos.y] = "   ";
                else
                {
                    if (from.x == to.x)
                        map[linkPos.x, linkPos.y] = " | ";
                    else if (from.y == to.y)
                        map[linkPos.x, linkPos.y] = "---";
                }
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

    bool TryMove(Vector2Int position)
    {
        // Si dans la salle, OK
        if (position.x >= 0 && position.x < rooms[playerRoomPos].size.x &&
            position.y >= 0 && position.y < rooms[playerRoomPos].size.y)
            return true;

        // Si c'est une porte (même extérieure), OK
        if (rooms[playerRoomPos].doors.ContainsKey(position))
            return true;

        // Sinon, bloqué
        return false;
    }
    void OnPlayerMove(Vector2Int playerPos)
    {
        if (rooms[playerRoomPos].doors.ContainsKey(playerPos))
        {
            Vector2Int newRoom = rooms[playerRoomPos].doors[playerPos].GetRoomConnected();
            Vector2Int newPos = rooms[playerRoomPos].doors[playerPos].GetDoorConnectedPos();

            playerRoomPos = newRoom;
            playerPos = newPos;
            rsoPlayerPos.Value = newPos;

            rooms[playerRoomPos].isVisited = true;
        }

        rseDrawConsole.Call();
    }
}