using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Moves : MonoBehaviour
{
    public static GameObject[] board;
    public static string turn;
    public static List<OldMove> movesList;
    static int counterProm;
    public static bool started;

    private void Awake()
    {
        counterProm = 0;
    }
    public static void MakeMove(int newPosB, GameObject gO, Vector3 pos, int promoting)
    {
        //Setting correct values, adding to counter and acknowledge that piece has moved.
        movesList = ChessGame.movesList;
        started = true;
        board = ChessGame.board;
        turn = ChessGame.turn;
        Pieces piece = gO.GetComponent<Pieces>();
        piece.movedTimes++;
        //Debug.Log("Current fen at beginning: " + ChessGame.GenerateFEN(board));
        int currentPosB = piece.currentPosB;
        string type = piece.type;
        string color = piece.color;
        string name = piece.name;
        piece.hasMoved = true;

        //SPECIAL MOVES
        //Castling
        //////Works well with just playing lol
        //////if (type == "king" && Math.Abs(newPosB - currentPosB) == 2)
        //////{
        //////    Debug.Log("Castling, piece we are checking: " + name);
        //////    //Castling Short
        //////    if (newPosB > currentPosB && board[newPosB + 1].GetComponent<Pieces>().type == "rook")
        //////    {
        //////        GameObject rook = board[newPosB + 1];
        //////        rook.transform.position -= new Vector3(2, 0, 0);
        //////        board[newPosB - 1] = rook;
        //////        board[newPosB + 1] = null;
        //////    }
        //////    //Castling Long
        //////    if (newPosB < currentPosB && board[newPosB - 2].GetComponent<Pieces>().type == "rook")
        //////    {
        //////        GameObject rook = board[newPosB - 2];
        //////        rook.transform.position += new Vector3(3, 0, 0);
        //////        board[newPosB + 1] = rook;
        //////        board[newPosB - 2] = null;
        //////    }
        //////}
        //if (ChessGame.GenerateFEN(board) == "rnNq1kr1/pp2bppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R" && promoting == -2 || promoting == -3)
        //{
        //    Debug.Log(" IN DOING MOVE i am: " + name + " newPosB: " + newPosB);
        //}
        if (promoting == -2)
        {
            //Debug.Log("Castling, piece we are checking: " + name);
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
        if(promoting == -3)
        {
            //Debug.Log("Castling, piece we are checking: " + name);
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
        ////Pawn Promotion
        if (promoting >= 1)
        {
            if (movesList.Count == 0)
            {
                //if (color == "white")
                //{
                //    Debug.Log("PROMOTION, WHITE QUEEN " + name);
                //    piece.name = "White Queen";
                //    piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[9];
                //    piece.type = "queen";
                //}
                //else if (color == "black")
                //{
                //    Debug.Log("PROMOTION, BLACK QUEEN");
                //    piece.name = "Black Queen";
                //    piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[8];
                //    piece.type = "queen";
                //}
                counterProm = 1;
                //I WAS THINKING ABOUT MAKING THE "BOOL PROMOTING" VALUE OF MOVE CLASS A INT SO WE CAN USE A SWITCH CASE HERE.
            }
            switch (promoting)
            {
                case 1:
                    if (color == "white")
                    {
                        //Debug.Log("1PROMOTION, WHITE QUEEN");
                        piece.name = "White Queen";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[9];
                        piece.type = "queen";
                        piece.fenCode = "Q";
                    }
                    else if (color == "black")
                    {
                        //Debug.Log("1PROMOTION, BLACK QUEEN");
                        piece.name = "Black Queen";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[8];
                        piece.type = "queen";
                        piece.fenCode = "q";
                    }
                    break;

                case 2:
                    if (color == "white")
                    {
                        //Debug.Log("1PROMOTION, WHITE ROOK");
                        piece.name = "White Rook";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[11];
                        piece.type = "rook";
                        piece.fenCode = "R";
                    }
                    else if (color == "black")
                    {
                        //Debug.Log("1PROMOTION, BLACK ROOK");
                        piece.name = "Black Rook";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[10];
                        piece.type = "rook";
                        piece.fenCode = "r";
                    }
                    break;

                case 3:
                    if (color == "white")
                    {
                        //Debug.Log("1PROMOTION, WHITE KNIGHT");
                        piece.name = "White Knight";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[5];
                        piece.type = "knight";
                        piece.fenCode = "N";
                    }
                    else if (color == "black")
                    {
                        //Debug.Log("1PROMOTION, BLACK KNIGHT");
                        piece.name = "Black Knight";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[4];
                        piece.type = "knight";
                        piece.fenCode = "n";
                    }
                    break;

                case 4:
                    if (color == "white")
                    {
                        //Debug.Log("1PROMOTION, WHITE BISHOP, PROMOTED");
                        piece.name = "Promoted White Bishop";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[0];
                        piece.type = "bishop";
                        piece.fenCode = "B";
                    }
                    else if (color == "black")
                    {
                        //Debug.Log("1PROMOTION, BLACK BISHOP, PROMOTED");
                        piece.name = "Promoted Black Bishop";
                        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[1];
                        piece.type = "bishop";
                        piece.fenCode = "b";
                    }
                    break;

                //case 5:
                //    if (color == "white")
                //    {
                //        Debug.Log("1PROMOTION, WHITE BISHOP");
                //        piece.name = "White Pawn";
                //        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[7];
                //        piece.type = "pawn";
                //        piece.fenCode = "P";
                //    }
                //    else if (color == "black")
                //    {
                //        Debug.Log("1PROMOTION, BLACK BISHOP");
                //        piece.name = "Black Pawn";
                //        piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[6];
                //        piece.type = "pawn";
                //        piece.fenCode = "p";
                //    }
                //    break;
            }
        }//This works for when we actually wanna play:
        //if (type == "pawn" && color == "white" && (int)(newPosB / 8) == 7)
        //    {
        //Debug.Log("PROMOTION");
        //piece.name = "White Queen";
        //piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[9];
        //piece.type = "queen";
        //    }
        //if (type == "pawn" && color == "black" && (int)(newPosB / 8) == 0)
        //{
        //Debug.Log("PROMOTION");
        //piece.name = "Black Queen";
        //piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[8];
        //piece.type = "queen";
        //}
        currentPosB = piece.currentPosB;
        type = piece.type;
        color = piece.color;
        name = piece.name;
        board[currentPosB] = null;
        GameObject pieceWeWillTake = null;

        bool beforeWasPawnTwoTiles = false;
        for (int i = 0; i < 64; ++i)
        {
            if (board[i] != null)
            {
                Pieces piecesLoop = board[i].GetComponent<Pieces>();
                if (piecesLoop.justMovedTwoStepsPawn == true)
                {
                    if (movesList.Count != 0)
                    {
                        beforeWasPawnTwoTiles = true;
                    }
                    break;
                }
            }

        }

        //En Passant
        int[] pawnAttacks = { -9, -7, 7, 9 };
        if (type == "pawn" && board[newPosB] == null && pawnAttacks.Contains(newPosB - currentPosB))
        {
            //Debug.Log("EN PASSANT as turn is: " + color);
            switch (newPosB - currentPosB)
            {
                //optimize this
                case -9:
                    //black, lower left
                    GameObject pawn = board[currentPosB - 1];
                    pieceWeWillTake = pawn;
                    Destroy(pawn);
                    board[currentPosB - 1] = null;
                    break;
                case -7:
                    //black, lower right
                    GameObject pawn2 = board[currentPosB + 1];
                    pieceWeWillTake = pawn2;
                    Destroy(pawn2);
                    board[currentPosB + 1] = null;
                    break;
                case 7:
                    //white, upper left
                    GameObject pawn3 = board[currentPosB - 1];
                    pieceWeWillTake = pawn3;
                    Destroy(pawn3);
                    board[currentPosB - 1] = null;
                    break;
                case 9:
                    //white, upper right
                    GameObject pawn4 = board[currentPosB + 1];
                    pieceWeWillTake = pawn4;
                    Destroy(pawn4);
                    board[currentPosB + 1] = null;
                    break;
            }
        }
        if (board[newPosB] != null)
        {
            pieceWeWillTake = board[newPosB];
            Destroy(pieceWeWillTake);
        }
        board[newPosB] = gO;
        gO.transform.position = pos;
        //maybe optimize this
        //bool beforeWasPawnTwoTiles = false;
        //for(int i = 0; i < 64; ++i)
        //{
        //    if(board[i] != null)
        //    {
        //        Pieces piecesLoop = board[i].GetComponent<Pieces>();
        //        if(piecesLoop.justMovedTwoStepsPawn == true)
        //        {
        //            if(movesList.Count != 0)
        //            {
        //                beforeWasPawnTwoTiles = true;
        //            }
        //            piecesLoop.justMovedTwoStepsPawn = false;
        //            break;
        //        }
        //    }
            
        //}
        foreach(GameObject piecesOnBoard in board)
        {
            if(piecesOnBoard != null)
            {
                piecesOnBoard.GetComponent<Pieces>().justMovedTwoStepsPawn = false;
            }
        }

        if (type == "pawn" && Math.Abs(newPosB - currentPosB) == 16)
        {
            piece.justMovedTwoStepsPawn = true;
        }
        piece.currentPosB = newPosB;
        ChessGame.NewTurn();
        turn = ChessGame.turn;
        movesList.Add(new OldMove(gO, currentPosB, newPosB, pieceWeWillTake, beforeWasPawnTwoTiles));
    }

    public static void UndoMove(Move m)
    {
        Pieces piece = m.pieceYouWannaMove.GetComponent<Pieces>();
        //if (ChessGame.GenerateFEN(board) == "rnNq1kr1/pp2bppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R" && m.promoting == -2 || m.promoting == -3)
        //{
        //    Debug.Log(" IN UNDO MOVE i am: " + m.pieceYouWannaMove.name + " newPosB: " + m.posWhereYouWannaGo + " and promoting is: " + m.promoting);
        //}
        piece.movedTimes--;
        if(piece.movedTimes == 0)
        {
            piece.hasMoved = false;
        }
        if (movesList.Count - 1 == 0)
        {
            //Debug.Log("Undo in " + turn);
            //board[movesList[0].whereItIsNow] = null;
            piece.currentPosB = movesList[0].whereItWas;
            piece.hasMoved = false;
            OldMove moveUndo = movesList[0];
            if (piece.type == "pawn" && ((int)m.whereYouAreRightNow.x - 1 + ((int)m.whereYouAreRightNow.y - 1) * 8) == movesList[movesList.Count - 1].whereItWas)
            {
                m.undoCounter = 0;
                moveUndo.pieceYouJustMoved.GetComponent<Pieces>().hasMoved = false;
            }

            //Debug.Log("Reset board totally");
            ChessGame.ResetBoard();
            movesList.RemoveAt(movesList.Count - 1);
        }
        else
        {
            //Assigning values:
            OldMove moveUndo = movesList[movesList.Count - 1];

            //////If it was a promoted bishop
            ////if (moveUndo.pieceYouJustMoved.GetComponent<Pieces>().name.Contains("Promoted") && m.promoting >= 4)
            ////{
            ////    //Debug.Log("Going back to being a pawn.");

            ////    if (moveUndo.pieceYouJustMoved.GetComponent<Pieces>().type == "bishop")
            ////    {
            ////        if (moveUndo.pieceYouJustMoved.GetComponent<Pieces>().color == "white")
            ////        {
            ////            //Debug.Log("undoPROMOTION, WHITE PAWN");
            ////            moveUndo.pieceYouJustMoved.GetComponent<Pieces>().name = "White Pawn";
            ////            moveUndo.pieceYouJustMoved.GetComponent<Pieces>().GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[7];
            ////            moveUndo.pieceYouJustMoved.GetComponent<Pieces>().type = "pawn";
            ////            moveUndo.pieceYouJustMoved.GetComponent<Pieces>().fenCode = "P";
            ////        }
            ////        else if (moveUndo.pieceYouJustMoved.GetComponent<Pieces>().color == "black")
            ////        {
            ////            //Debug.Log("undoPROMOTION, BLACK PAWN");
            ////            moveUndo.pieceYouJustMoved.GetComponent<Pieces>().name = "Black Pawn";
            ////            moveUndo.pieceYouJustMoved.GetComponent<Pieces>().GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[6];
            ////            moveUndo.pieceYouJustMoved.GetComponent<Pieces>().type = "pawn";
            ////            moveUndo.pieceYouJustMoved.GetComponent<Pieces>().fenCode = "p";
            ////        }
            ////    }
            ////}
            if(m.promoting > 0)
            {
                if (moveUndo.pieceYouJustMoved.GetComponent<Pieces>().color == "white")
                {
                    //Debug.Log("undoPROMOTION, WHITE PAWN");
                    moveUndo.pieceYouJustMoved.GetComponent<Pieces>().name = "White Pawn";
                    moveUndo.pieceYouJustMoved.GetComponent<Pieces>().GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[7];
                    moveUndo.pieceYouJustMoved.GetComponent<Pieces>().type = "pawn";
                    moveUndo.pieceYouJustMoved.GetComponent<Pieces>().fenCode = "P";
                }
                else if (moveUndo.pieceYouJustMoved.GetComponent<Pieces>().color == "black")
                {
                    //Debug.Log("undoPROMOTION, BLACK PAWN");
                    moveUndo.pieceYouJustMoved.GetComponent<Pieces>().name = "Black Pawn";
                    moveUndo.pieceYouJustMoved.GetComponent<Pieces>().GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[6];
                    moveUndo.pieceYouJustMoved.GetComponent<Pieces>().type = "pawn";
                    moveUndo.pieceYouJustMoved.GetComponent<Pieces>().fenCode = "p";
                }
            }

            //Castling stuff
            if (m.promoting == -2)
            {
                //Debug.Log("Fixed the castle, long");
                //Castling Long
                board[m.posWhereYouWannaGo + 1].GetComponent<Pieces>().currentPosB = m.posWhereYouWannaGo - 2;
                GameObject rook = board[m.posWhereYouWannaGo + 1];
                rook.transform.position -= new Vector3(3, 0, 0);
                board[m.posWhereYouWannaGo + 1] = null;
                board[m.posWhereYouWannaGo - 2] = rook;
                rook.GetComponent<Pieces>().hasMoved = false;
                piece.hasMoved = false;
            }
            else if (m.promoting == -3)
            {
                //Debug.Log("Fixed the castle, short");
                //Castling Short
                board[m.posWhereYouWannaGo - 1].GetComponent<Pieces>().currentPosB = m.posWhereYouWannaGo + 1;
                GameObject rook = board[m.posWhereYouWannaGo - 1];
                rook.transform.position += new Vector3(2, 0, 0);
                board[m.posWhereYouWannaGo - 1] = null;
                board[m.posWhereYouWannaGo + 1] = rook;
                rook.GetComponent<Pieces>().hasMoved = false;
                piece.hasMoved = false;
            }

            //Putting piece back where it belongs:
            board[moveUndo.whereItWas] = moveUndo.pieceYouJustMoved;
            moveUndo.pieceYouJustMoved.GetComponent<Pieces>().currentPosB = moveUndo.whereItWas;

            //Replacing what the piece destroyed (if any):
            board[moveUndo.whereItIsNow] = null;
            if (moveUndo.pieceWeJustTook != null)
            {
                Pieces takenPiece = moveUndo.pieceWeJustTook.GetComponent<Pieces>();
                board[takenPiece.currentPosB] = moveUndo.pieceWeJustTook;
            }

            //reset the piece that was available for en passant
            if(moveUndo.beforeEnPassant)
            {
                movesList[movesList.Count - 2].pieceYouJustMoved.GetComponent<Pieces>().justMovedTwoStepsPawn = true;
            }
            //idr what that does
            if (m.pieceYouWannaMove.GetComponent<Pieces>().type == "pawn" && ((int)m.whereYouAreRightNow.x - 1 + ((int)m.whereYouAreRightNow.y - 1) * 8) == movesList[movesList.Count - 1].whereItWas)
            {
                m.undoCounter = 0;
                moveUndo.pieceYouJustMoved.GetComponent<Pieces>().hasMoved = false;
            }
            ChessGame.turn = moveUndo.pieceYouJustMoved.GetComponent<Pieces>().color;
            movesList.RemoveAt(movesList.Count - 1);
        }
        
        turn = ChessGame.turn;
    }
}
