using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayingMoves : MonoBehaviour
{
    public static GameObject[] board;
    public static string turn;
    static int promotionChoice;

    public static void MakeMove(int newPosB, GameObject gO, Vector3 pos, int promoting, bool fromBot)
    {
        //Setting correct values, adding to counter and acknowledge that piece has moved.
        board = ChessGame.board;
        turn = ChessGame.turn;
        Pieces piece = gO.GetComponent<Pieces>();
        piece.movedTimes++;
        int currentPosB = piece.currentPosB;
        string type = piece.type;
        string color = piece.color;
        string name = piece.name;
        piece.hasMoved = true;

        //SPECIAL MOVES
        //Castling
        if (promoting == -2)
        {
            //Castling Long
            if (newPosB < currentPosB && board[newPosB - 2].GetComponent<Pieces>().type == "rook")
            {
                board[newPosB - 2].GetComponent<Pieces>().currentPosB = newPosB + 1;
                GameObject rook = board[newPosB - 2];
                rook.transform.position += new Vector3(3, 0, 0);
                board[newPosB + 1] = rook;
                board[newPosB - 2] = null;
            }
        }
        if (promoting == -3)
        {
            //Castling Short
            if (newPosB > currentPosB && board[newPosB + 1].GetComponent<Pieces>().type == "rook")
            {
                board[newPosB + 1].GetComponent<Pieces>().currentPosB = newPosB - 1;
                GameObject rook = board[newPosB + 1];
                rook.transform.position -= new Vector3(2, 0, 0);
                board[newPosB - 1] = rook;
                board[newPosB + 1] = null;
            }
        }
        //Pawn Promotion... fix this
        if (promoting == 1)
        {
            if (color == "white")
            {
                piece.name = "White Queen";
                piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[9];
                piece.type = "queen";
            }
            else if (color == "black")
            {
                piece.name = "Black Queen";
                piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[8];
                piece.type = "queen";
            }
        }
        //En Passant
        int[] pawnAttacks = { -9, -7, 7, 9 };
        if (type == "pawn" && board[newPosB] == null && pawnAttacks.Contains(newPosB - currentPosB))
        {
            switch (newPosB - currentPosB)
            {
                case -9:
                case 7:
                    GameObject pawn = board[currentPosB - 1];
                    Destroy(pawn);
                    board[currentPosB - 1] = null;
                    break;
                case -7:
                case 9:
                    GameObject pawn2 = board[currentPosB + 1];
                    Destroy(pawn2);
                    board[currentPosB + 1] = null;
                    break;
            }
        }


        currentPosB = piece.currentPosB;
        type = piece.type;
        color = piece.color;
        name = piece.name;
        board[currentPosB] = null;
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
        if (board[newPosB] != null)
        {
            Destroy(board[newPosB]);
        }
        Debug.Log("Before moving " + name + ", it's here : " + currentPosB + " while physically here: " + gO.transform.position.x + ", " + gO.transform.position.y + ", " + gO.transform.position.z);
        board[newPosB] = gO;
        if(!fromBot)
        {
            gO.transform.position = pos;
        }
        else
        {
            gO.transform.position = new Vector3(newPosB % 8 + 1, (int)(newPosB/8) + 1, -1);
        }
        piece.currentPosB = newPosB;
        Debug.Log("After moving " + name + ", it's here : " + currentPosB + " while physically here: " + gO.transform.position.x + ", " + gO.transform.position.y + ", " + gO.transform.position.z);
        if (type == "pawn" && Math.Abs(newPosB - currentPosB) == 16)
        {
            piece.justMovedTwoStepsPawn = true;
        }
        ChessGame.NewTurn();
        turn = ChessGame.turn;
        Debug.Log(turn);
    }
}
