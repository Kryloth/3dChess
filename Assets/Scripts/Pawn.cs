using System.Collections;
using UnityEngine;

public class Pawn : ChessPiece
{

    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8];
        possibleTrueMoves.Clear();

        ChessPiece c, c2;

        int[] e = BoardManager.Instance.EnPassantMove;

        if (chessColor == ChessColor.White)
        {
            ////// White team move //////

            // Diagonal left
            if (CurrentX != 0 && CurrentY != 7)
            {
                if(e[0] == CurrentX -1 && e[1] == CurrentY + 1){
                    r[CurrentX - 1, CurrentY + 1] = true;
                    possibleTrueMoves.Add(new Vector2(CurrentX - 1, CurrentY + 1));
                }

                c = BoardManager.Instance.ChessPieces[CurrentX - 1, CurrentY + 1];
                if (c != null && c.chessColor == ChessColor.Black){
                    r[CurrentX - 1, CurrentY + 1] = true;
                    possibleTrueMoves.Add(new Vector2(CurrentX - 1, CurrentY + 1));
                }
            }

            // Diagonal right
            if (CurrentX != 7 && CurrentY != 7)
            {
                if (e[0] == CurrentX + 1 && e[1] == CurrentY + 1){
                    r[CurrentX + 1, CurrentY + 1] = true;
                    possibleTrueMoves.Add(new Vector2(CurrentX + 1, CurrentY + 1));
                }

                c = BoardManager.Instance.ChessPieces[CurrentX + 1, CurrentY + 1];
                if (c != null && c.chessColor == ChessColor.Black){
                    r[CurrentX + 1, CurrentY + 1] = true;
                    possibleTrueMoves.Add(new Vector2(CurrentX + 1, CurrentY + 1));
                }
            }

            // Middle
            if (CurrentY != 7)
            {
                c = BoardManager.Instance.ChessPieces[CurrentX, CurrentY + 1];
                if (c == null){
                    r[CurrentX, CurrentY + 1] = true;
                    possibleTrueMoves.Add(new Vector2(CurrentX, CurrentY + 1));
                }
            }

            // Middle on first move
            if (CurrentY == 1)
            {
                c = BoardManager.Instance.ChessPieces[CurrentX, CurrentY + 1];
                c2 = BoardManager.Instance.ChessPieces[CurrentX, CurrentY + 2];
                if (c == null && c2 == null){
                    r[CurrentX, CurrentY + 2] = true;
                    possibleTrueMoves.Add(new Vector2(CurrentX, CurrentY + 2));
                }
            }
        }
        else
        {
            ////// Black team move //////

            // Diagonal left
            if (CurrentX != 0 && CurrentY != 0)
            {
                if (e[0] == CurrentX - 1 && e[1] == CurrentY - 1){
                    r[CurrentX - 1, CurrentY - 1] = true;
                    possibleTrueMoves.Add(new Vector2(CurrentX - 1, CurrentY - 1));
                }

                c = BoardManager.Instance.ChessPieces[CurrentX - 1, CurrentY - 1];
                if (c != null && c.chessColor == ChessColor.White){
                    r[CurrentX - 1, CurrentY - 1] = true;
                    possibleTrueMoves.Add(new Vector2(CurrentX - 1, CurrentY - 1));
                }
            }

            // Diagonal right
            if (CurrentX != 7 && CurrentY != 0)
            {
                if (e[0] == CurrentX + 1 && e[1] == CurrentY - 1){
                    r[CurrentX + 1, CurrentY - 1] = true;
                    possibleTrueMoves.Add(new Vector2(CurrentX + 1, CurrentY - 1));
                }

                c = BoardManager.Instance.ChessPieces[CurrentX + 1, CurrentY - 1];
                if (c != null && c.chessColor == ChessColor.White){
                    r[CurrentX + 1, CurrentY - 1] = true;
                    possibleTrueMoves.Add(new Vector2(CurrentX + 1, CurrentY - 1));
                }
            }

            // Middle
            if (CurrentY != 0)
            {
                c = BoardManager.Instance.ChessPieces[CurrentX, CurrentY - 1];
                if (c == null){
                    r[CurrentX, CurrentY - 1] = true;
                    possibleTrueMoves.Add(new Vector2(CurrentX, CurrentY - 1));
                }
            }

            // Middle on first move
            if (CurrentY == 6)
            {
                c = BoardManager.Instance.ChessPieces[CurrentX, CurrentY - 1];
                c2 = BoardManager.Instance.ChessPieces[CurrentX, CurrentY - 2];
                if (c == null && c2 == null){
                    r[CurrentX, CurrentY - 2] = true;
                    possibleTrueMoves.Add(new Vector2(CurrentX, CurrentY - 2));
                }
            }
        }
        return r;
    }
}
