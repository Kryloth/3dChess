using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
    public Vector3 OriginalPos;
    public int OriginalX;
    public int OriginalY;
    public int CurrentX { set; get; }
    public int CurrentY { set; get; }
    public List<Vector2> possibleTrueMoves = null;

    public ChessColor chessColor;
    private void Start() {
        OriginalPos = this.transform.position;
    }
    public void SetOriginalPosition(int x, int y){
        OriginalX = x;
        OriginalY = y;
    }
    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }

    public virtual bool[,] PossibleMoves()
    {
        return new bool[8, 8];
    }

    public bool Move(int x, int y, ref bool[,] r)
    {
        possibleTrueMoves.Clear();
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            ChessPiece c = BoardManager.Instance.ChessPieces[x, y];
            if (c == null){
                r[x, y] = true;
                possibleTrueMoves.Add(new Vector2(x, y));
            }
            else
            {
                if (chessColor != c.chessColor){
                    r[x, y] = true;
                    possibleTrueMoves.Add(new Vector2(x, y));
                }
                return true;
            }
        }
        return false;
    }
}
