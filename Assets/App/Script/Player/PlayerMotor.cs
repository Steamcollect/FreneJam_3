using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    //[Header("Settings")]
    [Header("References")]

    //[Space(10)]
    // RSO
    [SerializeField] RSO_PlayerPos rsoPlayerPos;
    [SerializeField] RSO_GameState rsoGameState;
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_SetPlayerPos rseSetPlayerPos;

    [Header("Output")]
    [SerializeField] RSF_PlayerTryMovement rsfPlayerTryMovement;
    [SerializeField] RSE_OnPlayerMove rseOnPlayerMove;
    [SerializeField] RSF_IsPlayerOnDamageable rsfIsPlayerOnDamageable;
    [SerializeField] RSE_TakeDamage rseTakeDamage;

    private void Update()
    {
        if(rsoGameState.Value == GameState.InGame)
        {
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow)) Move(Vector2Int.up);
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) Move(Vector2Int.down);
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector2Int.left);
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector2Int.right);
        }
    }

    void Move(Vector2Int input)
    {
        Vector2Int desirePos = rsoPlayerPos.Value + input;
        if(rsfPlayerTryMovement.Call(desirePos))
        {
            rsoPlayerPos.Value = desirePos;
            rseOnPlayerMove.Call(desirePos);

            if (rsfIsPlayerOnDamageable.Call()) rseTakeDamage.Call();
        }
    }
}