using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Perft : MonoBehaviour
{
    static GameObject[] board;
    private void Start()
    {
        board = BoardGrid.board;
    }
    public static int MovesTest(int ply)
    {
        if (ply == 0)
        {
            return 1;
        }
        List<Move> moves = MovesFinderForEveryPiece(ply, ChessGame.turn);
        int counter = 0;
        int divideCounter = 0;
        foreach (Move m in moves)
        {
            //if (ply == 4)
            //{
            //    divideCounter = counter;
            //}
            //if (ply == 3 && ChessGame.GenerateFEN(board) == "rnbq1k1r/pp1Pbppp/2p5/8/2B5/6P1/PPP1Nn1P/RNBQK2R")
            //{
            //    divideCounter = counter;
            //}
            //if (ply == 2 && ChessGame.GenerateFEN(board) == "rnb1qk1r/pp1Pbppp/2p5/8/2B5/6P1/PPP1Nn1P/RNBQK2R")
            //{
            //    divideCounter = counter;
            //}
            Moves.MakeMove(m.posWhereYouWannaGo, m.pieceYouWannaMove, m.whereYouAreRightNow, m.promoting);
            counter += MovesTest(ply - 1);
            Moves.UndoMove(m);
            //if (ply == 4)
            //{
            //    Debug.Log(ChessGame.ChangeFormatPos((int)m.whereYouAreRightNow.x - 1 + ((int)m.whereYouAreRightNow.y - 1) * 8) + ChessGame.ChangeFormatPos(m.posWhereYouWannaGo) + " : " + (counter - divideCounter));
            //}
            //if (ply == 3 && ChessGame.GenerateFEN(board) == "rnbq1k1r/pp1Pbppp/2p5/8/2B5/6P1/PPP1Nn1P/RNBQK2R")
            //{
            //    if (board[24] != null)
            //    {
            //        Debug.Log("## " + ChessGame.ChangeFormatPos((int)m.whereYouAreRightNow.x - 1 + ((int)m.whereYouAreRightNow.y - 1) * 8) + ChessGame.ChangeFormatPos(m.posWhereYouWannaGo) + " : " + (counter - divideCounter) + " and the pawn at 24 is: " + board[24].GetComponent<Pieces>().justMovedTwoStepsPawn);
            //    }
            //    else
            //    {
            //        Debug.Log("## " + ChessGame.ChangeFormatPos((int)m.whereYouAreRightNow.x - 1 + ((int)m.whereYouAreRightNow.y - 1) * 8) + ChessGame.ChangeFormatPos(m.posWhereYouWannaGo) + " : " + (counter - divideCounter));
            //    }
            //    //Debug.Log("## " + ChangeFormatPos((int)m.whereYouAreRightNow.x - 1 + ((int)m.whereYouAreRightNow.y - 1) * 8) + ChangeFormatPos(m.posWhereYouWannaGo) + " : " + (counter - divideCounter) + " and the pawn at 24 is: " + board[24].GetComponent<Pieces>().justMovedTwoStepsPawn);
            //}
            //if (ply == 2 && ChessGame.GenerateFEN(board) == "rnb1qk1r/pp1Pbppp/2p5/8/2B5/6P1/PPP1Nn1P/RNBQK2R")
            //{
            //    if (board[24] != null)
            //    {
            //        Debug.Log("#### " + ChessGame.ChangeFormatPos((int)m.whereYouAreRightNow.x - 1 + ((int)m.whereYouAreRightNow.y - 1) * 8) + ChessGame.ChangeFormatPos(m.posWhereYouWannaGo) + " : " + (counter - divideCounter) + " and the pawn at 24 is: " + board[24].GetComponent<Pieces>().justMovedTwoStepsPawn);
            //    }
            //    else
            //    {
            //        Debug.Log("#### " + ChessGame.ChangeFormatPos((int)m.whereYouAreRightNow.x - 1 + ((int)m.whereYouAreRightNow.y - 1) * 8) + ChessGame.ChangeFormatPos(m.posWhereYouWannaGo) + " : " + (counter - divideCounter));
            //    }
            //    //Debug.Log("#### " + ChangeFormatPos((int)m.whereYouAreRightNow.x - 1 + ((int)m.whereYouAreRightNow.y - 1) * 8) + ChangeFormatPos(m.posWhereYouWannaGo) + " : " + (counter - divideCounter));
            //}
        }

        return counter;
    }

    public static List<Move> MovesFinderForEveryPiece(int ply, string colorInput)
    {
        List<Move> moves = new List<Move>();
        board = BoardGrid.board;
        foreach (GameObject g in board)
        {
            if (g != null)
            {
                Pieces p = g.GetComponent<Pieces>();
                if (p.color == colorInput)
                {
                    //Debug.Log("Checking " + g.name + " at: " + p.currentPosB + " and hasMoved is: " + p.hasMoved);
                    List<int> legalMoves = p.FindLegalMoves();
                    if (legalMoves.Count != 0)
                    {
                        int promoIndex = FindPromoIndex(legalMoves);
                        int promoCounter = 1;
                        for (int i = 0; i < legalMoves.Count; ++i)
                        {
                            if (board[legalMoves[i]] != null)
                            {
                                //Debug.Log("Capture Counter at ply: " + ply);
                            }
                            int promoting;
                            if (promoIndex != -1 && promoCounter <= 4)
                            {
                                switch (promoCounter)
                                {
                                    case 1:
                                        //Debug.Log("Promotion Counter at ply: " + ply);
                                        if (p.color == "white")
                                        {
                                            g.GetComponent<Pieces>().name = "White Queen";
                                            g.GetComponent<Pieces>().GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[9];
                                            g.GetComponent<Pieces>().type = "queen";
                                            g.GetComponent<Pieces>().fenCode = "Q";
                                        }
                                        else if (p.color == "black")
                                        {
                                            g.GetComponent<Pieces>().name = "Black Queen";
                                            g.GetComponent<Pieces>().GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[8];
                                            g.GetComponent<Pieces>().type = "queen";
                                            g.GetComponent<Pieces>().fenCode = "q";
                                        }
                                        break;

                                    case 2:
                                        //Debug.Log("Promotion Counter at ply: " + ply);
                                        if (p.color == "white")
                                        {
                                            //Debug.Log("PROMOTION, WHITE ROOK");
                                            g.GetComponent<Pieces>().name = "White Rook";
                                            g.GetComponent<Pieces>().GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[11];
                                            g.GetComponent<Pieces>().type = "rook";
                                            g.GetComponent<Pieces>().fenCode = "R";
                                        }
                                        else if (p.color == "black")
                                        {
                                            //Debug.Log("PROMOTION, BLACK ROOK");
                                            g.GetComponent<Pieces>().name = "Black Rook";
                                            g.GetComponent<Pieces>().GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[10];
                                            g.GetComponent<Pieces>().type = "rook";
                                            g.GetComponent<Pieces>().fenCode = "r";
                                        }
                                        break;

                                    case 3:
                                        //Debug.Log("Promotion Counter at ply: " + ply);
                                        if (p.color == "white")
                                        {
                                            //Debug.Log("PROMOTION, WHITE KNIGHT");
                                            g.GetComponent<Pieces>().name = "White Knight";
                                            g.GetComponent<Pieces>().GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[5];
                                            g.GetComponent<Pieces>().type = "knight";
                                            g.GetComponent<Pieces>().fenCode = "N";
                                        }
                                        else if (p.color == "black")
                                        {
                                            //Debug.Log("PROMOTION, BLACK KNIGHT");
                                            g.GetComponent<Pieces>().name = "Black Knight";
                                            g.GetComponent<Pieces>().GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[4];
                                            g.GetComponent<Pieces>().type = "knight";
                                            g.GetComponent<Pieces>().fenCode = "n";
                                        }
                                        break;

                                    case 4:
                                        //Debug.Log("Promotion Counter at ply: " + ply);
                                        if (p.color == "white")
                                        {
                                            //Debug.Log("PROMOTION, WHITE BISHOP");
                                            g.GetComponent<Pieces>().name = "Promoted White Bishop";
                                            g.GetComponent<Pieces>().GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[0];
                                            g.GetComponent<Pieces>().type = "bishop";
                                            g.GetComponent<Pieces>().fenCode = "B";
                                        }
                                        else if (p.color == "black")
                                        {
                                            //Debug.Log("PROMOTION, BLACK BISHOP");
                                            g.GetComponent<Pieces>().name = "Promoted Black Bishop";
                                            g.GetComponent<Pieces>().GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[1];
                                            g.GetComponent<Pieces>().type = "bishop";
                                            g.GetComponent<Pieces>().fenCode = "b";
                                        }
                                        break;

                                }
                                promoting = promoCounter;
                                promoCounter++;
                            }
                            else if (p.type == "king" && p.currentPosB - legalMoves[i] == 2)
                            {
                                //Debug.Log("Castling Counter at ply: " + ply);
                                p.hasMoved = false;
                                promoting = -2;
                            }
                            else if (p.type == "king" && p.currentPosB - legalMoves[i] == -2)
                            {
                                //Debug.Log("Castling Counter at ply: " + ply);
                                p.hasMoved = false;
                                promoting = -3;
                            }
                            else
                            {
                                promoting = 0;
                            }
                            if (ChessGame.GenerateFEN(board) == "rnb1qk1r/pp1Pbppp/2p5/8/2B5/6P1/PPP1Nn1P/RNBQK2R")
                            {
                                Debug.Log("I am: " + p.name + ". from : " + p.currentPosB + " and my moves are: " + legalMoves[i]);
                            }
                            moves.Add(new Move(legalMoves[i], g, p.transform.position, promoting, ply));
                            if (promoCounter == 5)
                            {
                                if (p.color == "white")
                                {
                                    //Debug.Log("GOING BACK TO WHITE PAWN");
                                    p.name = "White Pawn";
                                    p.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[7];
                                    p.type = "pawn";
                                    p.fenCode = "P";
                                }
                                else if (p.color == "black")
                                {
                                    //Debug.Log("GOING BACK TO BLACK PAWN");
                                    p.name = "Black Pawn";
                                    p.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[6];
                                    p.type = "pawn";
                                    p.fenCode = "p";
                                }
                                promoCounter = 1;
                            }
                        }
                    }
                }
            }
        }
        //Debug.Log("I am returning: " + moves.Count + ". As turn is: " + turn + ". And fen is like this: " + GenerateFEN(board));
        //Debug.Log(GenerateFEN(board));
        return moves;
    }

    static int FindPromoIndex(List<int> legalMoves)
    {
        int result = -1;
        bool areTheSame = (legalMoves.Count == legalMoves.Distinct().Count());

        if (!areTheSame)
        {
            if (legalMoves.Count > 1)
            {
                for (int i = 0; i < legalMoves.Count; ++i)
                {
                    if (legalMoves[i] == legalMoves[i + 1])
                    {
                        result = i;
                        break;
                    }
                }
            }
        }
        return result;
    }
}
