using System;
using UnityEngine;

public abstract class Player : MonoBehaviour {
    public abstract event Action<Player> OnPlayerInput;
    public ChessColor Color;
    public ChessPiece currentSelectedPiece;
    public Vector2 coordinate;
    public virtual void CheckInput(){}
    public virtual void CheckTurn(ChessColor color){}
}
public enum ChessColor{
    White,
    Black
}