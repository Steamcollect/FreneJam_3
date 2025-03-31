using System.Text;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    //[Header("Settings")]
    Vector2Int currentCoordonate;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    [SerializeField] RSE_SetPlayerPos rseSetPlayerPos;

    [Header("Output")]
    [SerializeField] RSF_PlayerTryMovement rsfPlayerTryMovement;
    [SerializeField] RSE_OnPlayerMove rseOnPlayerMove;

    private void OnEnable()
    {
        rseSetPlayerPos.Action += (Vector2Int pos)=> currentCoordonate = pos;
    }
    private void OnDisable()
    {
        rseSetPlayerPos.Action -= (Vector2Int pos)=> currentCoordonate = pos;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow)) Move(Vector2Int.up);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) Move(Vector2Int.down);
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector2Int.right);
    }

    void Move(Vector2Int input)
    {
        Vector2Int desirePos = currentCoordonate + input;
        if(rsfPlayerTryMovement.Call(desirePos))
        {
            currentCoordonate = desirePos;
            rseOnPlayerMove.Call(desirePos);
        }
    }
}