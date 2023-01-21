using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MovesToTrainAI : MonoBehaviour
{
    [SerializeField]
    public GameObject chessAgentGO;

    static ChessAgent chessAgent;
    public static GameObject[] board;
    public static string turn;
    static List<string> listFen;
    static List<string> tempList;

    private void Start()
    {
        board = ChessGame.board;
        turn = ChessGame.turn;
        chessAgent = chessAgentGO.GetComponent<ChessAgent>();
        listFen = new List<string>();
        tempList = new List<string>();
        listFen.Add("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
    }
    public static void MakeMove(int currentPiecePos, int futurePiecePos, int promoting, bool opponentToMoveAfter)
    {
        Debug.Log("Beginning: " + ChessGame.GenerateFEN(board) + " moving this piece: " + board[currentPiecePos].name);
        //Setting correct values, adding to counter and acknowledge that piece has moved.
        board = ChessGame.board;
        turn = ChessGame.turn;
        GameObject pieceGO = board[currentPiecePos];
        Pieces piece = pieceGO.GetComponent<Pieces>();
        piece.movedTimes++;
        chessAgent = GameObject.Find("ChessAgent").GetComponent<ChessAgent>();
        //int currentPosB = piece.currentPosB;
        string type = piece.type;
        string color = piece.color;
        string name = piece.name;
        piece.hasMoved = true;

        ///SPECIAL MOVES
        //Castling
        if (type == "king" && Math.Abs(futurePiecePos - currentPiecePos) == 2)
        {
            //Castling Short
            if (futurePiecePos > currentPiecePos && board[futurePiecePos + 1].GetComponent<Pieces>().type == "rook")
            {
                GameObject rook = board[futurePiecePos + 1];
                rook.transform.position -= new Vector3(2, 0, 0);
                board[futurePiecePos - 1] = rook;
                board[futurePiecePos + 1] = null;
            }
            //Castling Long
            else if (futurePiecePos < currentPiecePos && board[futurePiecePos - 2].GetComponent<Pieces>().type == "rook")
            {
                GameObject rook = board[futurePiecePos - 2];
                rook.transform.position += new Vector3(3, 0, 0);
                board[futurePiecePos + 1] = rook;
                board[futurePiecePos - 2] = null;
            }
        }

        ////Pawn Promotion
        if (promoting > 0)
        {
            switch (promoting)
            {
                case 1:
                    if (color == "white")
                    {
                        piece.name = "White Queen";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[9];
                        piece.type = "queen";
                        piece.fenCode = "Q";
                    }
                    else if (color == "black")
                    {
                        piece.name = "Black Queen";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[8];
                        piece.type = "queen";
                        piece.fenCode = "q";
                    }
                    break;

                case 2:
                    if (color == "white")
                    {
                        piece.name = "White Rook";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[11];
                        piece.type = "rook";
                        piece.fenCode = "R";
                    }
                    else if (color == "black")
                    {
                        piece.name = "Black Rook";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[10];
                        piece.type = "rook";
                        piece.fenCode = "r";
                    }
                    break;

                case 3:
                    if (color == "white")
                    {
                        piece.name = "White Knight";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[5];
                        piece.type = "knight";
                        piece.fenCode = "N";
                    }
                    else if (color == "black")
                    {
                        piece.name = "Black Knight";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[4];
                        piece.type = "knight";
                        piece.fenCode = "n";
                    }
                    break;

                case 4:
                    if (color == "white")
                    {
                        piece.name = "White Bishop";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[0];
                        piece.type = "bishop";
                        piece.fenCode = "B";
                    }
                    else if (color == "black")
                    {
                        piece.name = "Black Bishop";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[1];
                        piece.type = "bishop";
                        piece.fenCode = "b";
                    }
                    break;
            }
        }

        //En Passant
        for (int i = 0; i < 64; ++i)
        {
            if (board[i] != null)
            {
                Pieces piecesLoop = board[i].GetComponent<Pieces>();
                if (piecesLoop.justMovedTwoStepsPawn == true)
                {
                    piecesLoop.justMovedTwoStepsPawn = false;
                    break;
                }
            }

        }
        int[] pawnAttacks = { -9, -7, 7, 9 };
        if (type == "pawn" && board[futurePiecePos] == null && pawnAttacks.Contains(futurePiecePos - currentPiecePos))
        {
            switch (futurePiecePos - currentPiecePos)
            {
                case -9:
                case 7:
                    GameObject pawn = board[currentPiecePos - 1];
                    Destroy(pawn);
                    board[currentPiecePos - 1] = null;
                    break;
                case -7:
                case 9:
                    GameObject pawn2 = board[currentPiecePos + 1];
                    Destroy(pawn2);
                    board[currentPiecePos + 1] = null;
                    break;
            }
        }


        if (board[futurePiecePos] != null)
        {
            GameObject opp = board[futurePiecePos];
            float multiplier = opponentToMoveAfter ? 1f : -1f;
            chessAgent.AddReward(opp.GetComponent<Pieces>().GetWorth() * multiplier);
            Destroy(opp);
        }
        board[currentPiecePos] = null;
        board[futurePiecePos] = pieceGO;
        pieceGO.transform.position = new Vector3((futurePiecePos % 8) + 1, (int)(futurePiecePos / 8) + 1, -1);

        if (type == "pawn" && Math.Abs(futurePiecePos - currentPiecePos) == 16)
        {
            piece.justMovedTwoStepsPawn = true;
        }
        piece.currentPosB = futurePiecePos;

        //Change turn
        string turnPreChange = turn;
        ChessGame.NewTurn();
        turn = ChessGame.turn;

        ///Check if game ended
        //Checkmate or Stalemate
        if (MovesFinder(turn).Count == 0)
        {
            if (MovesFinder(turnPreChange).Contains(FindEnnemyKing(turn)))
            {
                //Checkmate
                if(opponentToMoveAfter)
                {
                    chessAgent.AddReward(200f);
                    EndGame();
                    return;
                }
                else
                {
                    chessAgent.AddReward(-200f);
                    EndGame();
                    return;
                }
            }
            else
            {
                //Stalemate
                chessAgent.SetReward(0f);
                EndGame();
                return;

            }
        }

        //Draw by repition or because only the kings are left
        else if (CheckForRepitition(ChessGame.GenerateFEN(board)) || OnlyKingsLeft())
        {
            chessAgent.SetReward(0f);
            EndGame();
            return;
        }


        Debug.Log("End: " + ChessGame.GenerateFEN(board) + " and is at new place: " + board[futurePiecePos].name);

        //Let opponent play (or not)
        if (opponentToMoveAfter)
        {
            OpponentToMove();
        }
        else
        {
            chessAgent.RequestDecision();
        }
    }

    public static void OpponentToMove()
    {
        board = ChessGame.board;
        turn = ChessGame.turn;
        var random = new System.Random();
        //Choose a random piece to move
        List<int> availablePieces = new List<int>();
        for(int i = 0; i < 64; ++i)
        {
            GameObject gO = board[i];
            if(gO != null)
            {
                Pieces gOp = gO.GetComponent<Pieces>();
                if(gOp.color == turn && gOp.FindLegalMoves().Count != 0)
                {
                    availablePieces.Add(i);
                }
            }
        }
        int randomIndex = random.Next(availablePieces.Count);
        GameObject chosenOne = board[availablePieces[randomIndex]];

        //Choose a random move from this piece
        int promotion = 0;
        List<int> availableMoves = chosenOne.GetComponent<Pieces>().FindLegalMoves();
        if(availableMoves.Count != availableMoves.Distinct().ToList().Count)
        {
            promotion = random.Next(1, 5);
            //Debug.Log("gonna promote: " + promotion);
        }
        int randomMoveIndex = random.Next(availableMoves.Count);

        //Make move
        MakeMove(availablePieces[randomIndex], availableMoves[randomMoveIndex], promotion, false);
    }

    static bool CheckForRepitition(string newFEN)
    {
        if(tempList.Count != 0 && tempList.Contains(newFEN))
        {
            return true;
        }
        listFen.Add(newFEN);
        if (listFen.Count != listFen.Distinct().ToList().Count)
        {
            tempList.Add(newFEN);
        }
        listFen = listFen.Distinct().ToList();
        return false;
    }

    static bool OnlyKingsLeft()
    {
        for(int i = 0; i < 64; i++)
        {
            if(board[i] != null && board[i].GetComponent<Pieces>().type != "king")
            {
                return false;
            }
        }
        return true;
    }

    static int FindEnnemyKing(string colorInput)
    {
        int position = 0;
        foreach(GameObject gO in board)
        {
            if(gO != null)
            {
                Pieces p = gO.GetComponent<Pieces>();
                if(p.color == colorInput && p.type == "king")
                {
                    position = p.currentPosB;
                }
            }
        }
        return position;
    }

    static List<int> MovesFinder(string colorInput)
    {
        List<int> output = new List<int>();
        foreach (GameObject gO in board)
        {
            if (gO != null)
            {
                Pieces p = gO.GetComponent<Pieces>();
                if (p.color == colorInput)
                {
                    output.AddRange(p.FindLegalMoves());
                }
            }
        }
        return output;
    }

    static void EndGame()
    {
        Debug.Log("EndGame.");
        listFen.Clear();
        tempList.Clear();
        ChessGame.ResetBoard();
        ChessGame.turn = "white";
        chessAgent.EndEpisode();
    }
}
