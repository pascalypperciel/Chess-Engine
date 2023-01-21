using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChessAI : MonoBehaviour
{
    public static string turn;
    public static GameObject[] board;
    static int[] counterForMove;
    static Move bestMove;
    static string fen;
    public static void RequestMove()
    {
        bestMove = null;
        //Save board and stuff
        board = ChessGame.board;
        turn = ChessGame.turn;
        string turnCopy = new string(ChessGame.turn);
        fen = ChessGame.GenerateFEN(board) + " " + turn;
        //GameObject[] boardCopy = new GameObject[64];
        //for (int i = 0; i < 64; ++i)
        //{
        //    if (board[i] == null)
        //    {
        //        boardCopy[i] = null;
        //    }
        //    else
        //    {
        //        boardCopy[i] = Instantiate(board[i]);
        //    }
        //}

        //Find move and ruin everything
        Search(3, -99999, 99999);

        //Replace board and stuff
        ChessGame.turn = turnCopy;
        //ChessGame.ResetBoard(ChessGame.GenerateFEN(boardCopy) + " " + turnCopy);
        //ChessGame.board = BoardGrid.board;

        //Make move
        PlayingMoves.MakeMove(bestMove.posWhereYouWannaGo, bestMove.pieceYouWannaMove, bestMove.whereYouAreRightNow, 0, true);
    }

    public static int EvaluationPosition()
    {
        int whiteWorth = FindWorthColor("white");
        int blackWorth = FindWorthColor("black");

        int evaluation = whiteWorth - blackWorth;

        int multiplier = turn == "white" ? 1 : -1;

        return evaluation * multiplier;
    }

    static int FindWorthColor(string color)
    {
        int material = 0;
        foreach(GameObject gO in board)
        {
            if(gO != null)
            {
                Pieces p = gO.GetComponent<Pieces>();
                if(p.color == color)
                {
                    material += p.GetWorth();
                }
            }
        }
        return material;
    }

    static int Search(int ply, int alpha, int beta)
    {
        if(ply == 0)
        {
            return EvaluationPosition();
        }

        List<Move> moves = Perft.MovesFinderForEveryPiece(ply, turn);
        ForcingEvals(moves);
        if (ply == 1)
        {
            foreach (Move m in moves)
            {
                Debug.Log("Here are the moves for " + m.pieceYouWannaMove.name + " : " + m.posWhereYouWannaGo);
            }
        }
        if (moves.Count == 0)
        {
            Debug.Log("what");
            if(IsInCheck(moves, turn))
            {
                return -99999;
            }
            else
            {
                return 0;
            }
        }
        foreach(Move m in moves)
        {
            MovesAI.MakeMove(m.posWhereYouWannaGo, m.pieceYouWannaMove, m.whereYouAreRightNow, m.promoting);
            int eval = -Search(ply - 1, -beta, -alpha);
            MovesAI.UndoMove(m, fen);
            if(eval >= beta)
            {
                return beta;
            }
            if(eval > alpha)
            {
                bestMove = m;
            }
            alpha = Math.Max(alpha, eval);
        }
        if (bestMove == null)
        {
            bestMove = moves[0];
        }
        return alpha;
    }

    static bool IsInCheck(List<Move> moves, string color)
    {
        int position = -1;
        foreach (GameObject gO in board)
        {
            if (gO != null)
            {
                Pieces p = gO.GetComponent<Pieces>();
                if (p.color == color && p.type == "king")
                {
                    position = p.currentPosB;
                    break;
                }
            }
        }
        foreach(Move m in moves)
        {
            if(m.posWhereYouWannaGo == position)
            {
                return true;
            }
        }

        return false;
    }

    public static void ForcingEvals(List<Move> moves)
    {
        counterForMove = new int[moves.Count];
        for (int i = 0; i < moves.Count; i++)
        {
            Move m = moves[i];
            int counter = 0;
            Pieces p = m.pieceYouWannaMove.GetComponent<Pieces>();
            int worth = p.GetWorth();

            if (board[m.posWhereYouWannaGo] != null)
            {
                counter = board[m.posWhereYouWannaGo].GetComponent<Pieces>().GetWorth() - worth;
            }

            if (m.promoting >= 1)
            {
                switch (m.promoting)
                {
                    case 1:
                        counter += 9;
                        break;
                    case 2:
                        counter += 5;
                        break;
                    case 3:
                    case 4:
                        counter += 3;
                        break;
                }
            }

            if (m.pieceYouWannaMove.GetComponent<Pieces>().color == "white")
            {
                if ((int)(m.posWhereYouWannaGo / 8) <= 6)
                {
                    if (m.posWhereYouWannaGo % 8 >= 1)
                    {
                        if (board[m.posWhereYouWannaGo + 7] != null && board[m.posWhereYouWannaGo + 7].GetComponent<Pieces>().type == "pawn" && board[m.posWhereYouWannaGo + 7].GetComponent<Pieces>().color == "white")
                        {
                            counter -= worth;
                        }
                        else if (m.posWhereYouWannaGo % 8 <= 6)
                        {
                            if (board[m.posWhereYouWannaGo + 9] != null && board[m.posWhereYouWannaGo + 9].GetComponent<Pieces>().type == "pawn" && board[m.posWhereYouWannaGo + 9].GetComponent<Pieces>().color == "white")
                            {
                                counter -= worth;
                            }
                        }
                    }
                }
                else if (m.pieceYouWannaMove.GetComponent<Pieces>().color == "black")
                {
                    if ((int)(m.posWhereYouWannaGo / 8) >= 1)
                    {
                        if (m.posWhereYouWannaGo % 8 >= 1)
                        {
                            if (board[m.posWhereYouWannaGo - 9] != null && board[m.posWhereYouWannaGo - 9].GetComponent<Pieces>().type == "pawn" && board[m.posWhereYouWannaGo - 9].GetComponent<Pieces>().color == "white")
                            {
                                counter -= worth;
                            }
                            else if (m.posWhereYouWannaGo % 8 <= 6)
                            {
                                if (board[m.posWhereYouWannaGo - 7] != null && board[m.posWhereYouWannaGo - 7].GetComponent<Pieces>().type == "pawn" && board[m.posWhereYouWannaGo - 7].GetComponent<Pieces>().color == "white")
                                {
                                    counter -= worth;
                                }
                            }
                        }
                    }
                    counterForMove[i] = counter;
                }

                Sort(moves);
            }
        }
    }

    static void Sort(List<Move> moves)
    {
        for (int i = 0; i < moves.Count - 1; i++)
        {
            for (int j = i + 1; j > 0; j--)
            {
                int index = j - 1;
                if (counterForMove[index] < counterForMove[j])
                {
                    (moves[j], moves[index]) = (moves[index], moves[j]);
                    (counterForMove[j], counterForMove[index]) = (counterForMove[index], counterForMove[j]);
                }
            }
        }
    }
}
