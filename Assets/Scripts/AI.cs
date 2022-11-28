using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Player
{
    public override event Action<Player> OnPlayerInput;
    private void Start() 
    {
        BoardManager.Instance.OnTurnChanged += CheckTurn;
    }
    public override void CheckInput()
    {
        StartCoroutine(MakeADecision());
    }
    private IEnumerator MakeADecision(){
        Debug.Log("AI THINKING");
        List<ChessPiece> activePiecesThatCanMove = Color == ChessColor.White ? BoardManager.Instance.activeWhitePieces : BoardManager.Instance.activeBlackPieces;

        foreach(ChessPiece piece in activePiecesThatCanMove)piece.PossibleMoves();
        activePiecesThatCanMove = activePiecesThatCanMove.FindAll(x => x.possibleTrueMoves.Count > 0);

        if(currentSelectedPiece == null){
            ChessPiece randomPiece = activePiecesThatCanMove[UnityEngine.Random.Range(0, activePiecesThatCanMove.Count)];
            currentSelectedPiece = randomPiece;
            Debug.Log(currentSelectedPiece);
        }
        yield return new WaitForSeconds(0.5f);

        Vector2 randomMove = currentSelectedPiece.possibleTrueMoves[UnityEngine.Random.Range(0, currentSelectedPiece.possibleTrueMoves.Count)];
        coordinate = randomMove;

        yield return new WaitForSeconds(0.5f);
        OnPlayerInput?.Invoke(this);
        Debug.Log("AI FINISHED THINKING");
    }
    public override void CheckTurn(ChessColor color)
    {
        if(color == Color)CheckInput();
    }
}