using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RSE_OnDungeonCreated", menuName = "RSE/_/RSE_OnDungeonCreated")]
public class RSE_OnDungeonCreated : BT.ScriptablesObject.RuntimeScriptableEvent<Dictionary<Vector2Int, RoomData>>{}