using System;
using UnityEngine;

public class Human : Player
{
    public override event Action<Player> OnPlayerInput;
    private void Update() {
        CheckInput();
    }
    public override void CheckInput()
    {
        int x = 0;
        int y = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50.0f, LayerMask.GetMask("ChessPlane")))
        {
            x = (int)hit.point.x;
            y = (int)hit.point.z;
        }
        else
        {
            x = -1;
            y = -1;
        }
        coordinate = new Vector2(x, y);
        if (Input.GetMouseButtonDown(0))
        {
            OnPlayerInput?.Invoke(this);
        }
    }
    public override void CheckTurn(ChessColor color){}
}