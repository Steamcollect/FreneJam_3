using UnityEngine;

public class Door : Triggerable
{
    public Vector2Int roomConnected;
    public RSE_OnDoorEnter rseOnDoorEnter;

    public override void OnTriggerEnter()
    {
        rseOnDoorEnter.Call(roomConnected);
    }
}