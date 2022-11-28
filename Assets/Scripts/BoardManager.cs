using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; set; }
    public Player PlayerOne;
    public Player PlayerTwo;
    public ChessPiece WhiteQueen;
    public ChessPiece BlackQueen;
    [SerializeField]private TMPro.TextMeshProUGUI winningText;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;

    public List<ChessPiece> piecesList;
    private List<ChessPiece> activePieces{get{return piecesList.FindAll(x => x.gameObject.activeSelf);}}
    public List<ChessPiece> activeWhitePieces{get{return activePieces.FindAll(x => x.chessColor == ChessColor.White);}}
    public List<ChessPiece> activeBlackPieces{get{return activePieces.FindAll(x => x.chessColor == ChessColor.Black);}}

    private Quaternion whiteOrientation = Quaternion.Euler(0, 270, 0);
    private Quaternion blackOrientation = Quaternion.Euler(0, 90, 0);

    public ChessPiece[,] ChessPieces { get; set; }

    public ChessColor CurrentTurn;

    private Material previousMat;
    public Material selectedMat;

    public int[] EnPassantMove { set; get; }
    public event Action<ChessColor> OnTurnChanged;

    void Awake() 
    {
        Instance = this;
        SetPlayers();
    }
    void Start()
    {
        EnPassantMove = new int[2] { -1, -1 };
        SetAllPieces();
        NewGame(true);
    }
    public void NewGame(bool playerOneWhite){
        if(playerOneWhite){
            PlayerOne.Color = ChessColor.White;
            PlayerTwo.Color = ChessColor.Black;
        }else{
            PlayerOne.Color = ChessColor.Black;
            PlayerTwo.Color = ChessColor.White;
        }
        ResetGame();
    }
    void SetPlayers()
    {
        PlayerOne.OnPlayerInput += GetInput;
        PlayerTwo.OnPlayerInput += GetInput;
    }

    void GetInput(Player player)
    {
        if(player.Color != CurrentTurn)return;
        if(player.currentSelectedPiece == null){
            player.currentSelectedPiece = SelectChessman(player);
            return;
        }
        
        MovePiece(player);
    }
    private ChessPiece SelectChessman(Player player)
    {
        ChessPiece selectedPiece;
        Debug.Log("Choosing");

        int x, y;
        x = (int)player.coordinate.x;
        y = (int)player.coordinate.y;

        if (ChessPieces[x, y] == null) return null;
        if (ChessPieces[x, y].chessColor != player.Color) return null;
        bool hasAtLeastOneMove = false;

        var allowedMoves = ChessPieces[x, y].PossibleMoves();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (allowedMoves[i, j])
                {
                    hasAtLeastOneMove = true;
                    i = 8;
                    break;
                }
            }
        }
        if (!hasAtLeastOneMove)
            return null;
        
        selectedPiece = ChessPieces[x, y];
        BoardHighlights.Instance.HighLightAllowedMoves(allowedMoves);
        return selectedPiece;
    }

    private void MovePiece(Player player)
    {   
        ChessPiece selectedPiece = player.currentSelectedPiece;
        var allowedMoves = selectedPiece.PossibleMoves();
        int x, y;
        x = (int)player.coordinate.x;
        y = (int)player.coordinate.y;

        if (allowedMoves[x, y])
        {
            ChessPiece c = ChessPieces[x, y];

            if (c != null && c.chessColor != CurrentTurn)
            {
                // Capture a piece
                c.gameObject.SetActive(false);
                if (c.GetType() == typeof(King))
                {
                    // End the game
                    EndGame();
                    return;
                }
            }
            if (x == EnPassantMove[0] && y == EnPassantMove[1])
            {
                if (CurrentTurn == ChessColor.White)
                    c = ChessPieces[x, y - 1];
                else
                    c = ChessPieces[x, y + 1];

                c.gameObject.SetActive(false);
            }
            EnPassantMove[0] = -1;
            EnPassantMove[1] = -1;
            if (selectedPiece.GetType() == typeof(Pawn))
            {
                if(y == 7) // White Promotion
                {
                    selectedPiece.gameObject.SetActive(false);
                    ChessPieces[selectedPiece.CurrentX, selectedPiece.CurrentY] = null;
                    
                    c.gameObject.SetActive(false);
                    ChessPiece piece = Instantiate(WhiteQueen, GetTileCenter(x,y), whiteOrientation);

                    piece.transform.SetParent(transform);
                    piece.chessColor = ChessColor.White;

                    ChessPieces[x, y] = piece;
                    ChessPieces[x, y].SetPosition(x, y);

                    piecesList.Add(piece);
                    selectedPiece = ChessPieces[x, y];
                }
                else if (y == 0) // Black Promotion
                {
                    selectedPiece.gameObject.SetActive(false);
                    ChessPieces[selectedPiece.CurrentX, selectedPiece.CurrentY] = null;

                    c.gameObject.SetActive(false);
                    ChessPiece piece = Instantiate(BlackQueen, GetTileCenter(x,y), whiteOrientation);

                    piece.transform.SetParent(transform);
                    piece.chessColor = ChessColor.Black;
                    
                    ChessPieces[x, y] = piece;
                    ChessPieces[x, y].SetPosition(x, y);

                    piecesList.Add(piece);
                    selectedPiece = ChessPieces[x, y];
                }
                EnPassantMove[0] = x;
                if (selectedPiece.CurrentY == 1 && y == 3)
                    EnPassantMove[1] = y - 1;
                else if (selectedPiece.CurrentY == 6 && y == 4)
                    EnPassantMove[1] = y + 1;
            }

            ChessPieces[selectedPiece.CurrentX, selectedPiece.CurrentY] = null;
            selectedPiece.transform.position = GetTileCenter(x, y);
            selectedPiece.SetPosition(x, y);
            ChessPieces[x, y] = selectedPiece;
            if(CurrentTurn == ChessColor.White)CurrentTurn = ChessColor.Black;
            else CurrentTurn = ChessColor.White;
            OnTurnChanged?.Invoke(CurrentTurn);
        }

        BoardHighlights.Instance.HideHighlights();
        player.currentSelectedPiece = null;
    }
    private void ResetGame()
    {
        if(piecesList.Count > 32){
            for (int i = 32; i < piecesList.Count; i++)
            {
                Destroy(piecesList[i].gameObject);
                piecesList.RemoveAt(i);
            }
        }
        ChessPieces = new ChessPiece[8, 8];
        Array.Clear(ChessPieces, 0, ChessPieces.Length);

        foreach(ChessPiece piece in piecesList){
            piece.gameObject.SetActive(true);
            piece.transform.position = piece.OriginalPos;
            ChessPieces[piece.OriginalX, piece.OriginalY] = piece;
            piece.SetPosition(piece.OriginalX, piece.OriginalY);
        }
        CurrentTurn = ChessColor.White;
        OnTurnChanged?.Invoke(CurrentTurn);
    }
    private void SetPiecesBoardPosition(int index, int x, int y){
        piecesList[index].SetOriginalPosition(x, y);
    }
    private void SetAllPieces()
    {
        /////// White ///////
        // King
        SetPiecesBoardPosition(0, 4, 0);

        // Queen
        SetPiecesBoardPosition(1, 3, 0);

        // Rooks
        SetPiecesBoardPosition(2, 0, 0);
        SetPiecesBoardPosition(3, 7, 0);

        // Bishops
        SetPiecesBoardPosition(4, 2, 0);
        SetPiecesBoardPosition(5, 5, 0);

        // Knights
        SetPiecesBoardPosition(6, 1, 0);
        SetPiecesBoardPosition(7, 6, 0);

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SetPiecesBoardPosition(8+i, i, 1);
        }


        /////// Black ///////
        // King
        SetPiecesBoardPosition(16, 4, 7);

        // Queen
        SetPiecesBoardPosition(17, 3, 7);

        // Rooks
        SetPiecesBoardPosition(18, 0, 7);
        SetPiecesBoardPosition(19, 7, 7);

        // Bishops
        SetPiecesBoardPosition(20, 2, 7);
        SetPiecesBoardPosition(21, 5, 7);

        // Knights
        SetPiecesBoardPosition(22, 1, 7);
        SetPiecesBoardPosition(23, 6, 7);

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SetPiecesBoardPosition(24+i, i, 6);
        }
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;

        return origin;
    }

    private void EndGame()
    {
        if (CurrentTurn == ChessColor.White){
            StartCoroutine(ShowWinningText("WHITE WINS"));
            winningText.color = Color.white;
        }
        else{
            StartCoroutine(ShowWinningText("BLACK WINS"));
            winningText.color = Color.black;
        }

        CurrentTurn = ChessColor.White;
        BoardHighlights.Instance.HideHighlights();
        ResetGame();
    }
    IEnumerator ShowWinningText(string text){
        winningText.gameObject.SetActive(true);
        winningText.text = text;
        yield return new WaitForSeconds(2f);
        winningText.gameObject.SetActive(false);
    }
}


