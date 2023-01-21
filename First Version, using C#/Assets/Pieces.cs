using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//This script is added to every single piece on the board. It allows the piece to be
//played with the mouse and provides information about the piece such as its color,
//type, and legal moves.
public class Pieces : MonoBehaviour
{
    //Setting Variables
    Vector3 mouseOffset, originalPosition;
    public int currentPosB, movedTimes;
    Collider2D col;
    public static GameObject[] board, dotGrid;
    public string color, type, oppColor, fenCode;
    public static string turn;
    public bool justMovedTwoStepsPawn, hasMoved, isPinned;
    List<int> attempt;

    private void Start()
    {
        movedTimes = 0;
        isPinned = false;
        turn = ChessGame.turn;
        col = gameObject.GetComponent<Collider2D>();
        board = ChessGame.board;
        dotGrid = BoardGrid.dotGrid;
        hasMoved = false;
        justMovedTwoStepsPawn = false;
        if (color == "white")
        {
            oppColor = "black";
        }
        else
        {
            oppColor = "white";
        }
    }

    //This ID is used by the ML-Agent. The ML-Agent gets all positions of pieces on the
    //board and can differentiate them by their ID.
    public int GetID()
    {
        int id = 0;
        switch(type)
        {
            case "pawn":
                id = 1;
                break;
            case "rook":
                id = 2;
                break;
            case "knight":
                id = 3;
                break;
            case "bishop":
                id = 4;
                break;
            case "queen":
                id = 5;
                break;
            case "king":
                id = 6;
                break;
        }
        if(color == "black")
        {
            id += 6;
        }

        return id;
    }

    //This Worth variable is also used by the ML-Agent and is used to give (or remove) 
    //reward points.
    public int GetWorth()
    {
        int worth = 0;
        switch (type)
        {
            case "pawn":
                worth = 1;
                break;
            case "rook":
                worth = 5;
                break;
            case "knight":
                worth = 3;
                break;
            case "bishop":
                worth = 3;
                break;
            case "queen":
                worth = 9;
                break;
            case "king":
                worth = 200;
                break;
        }

        return worth;
    }

    //This Vector3 variable has to do with moving the piece on the board by the player
    //using their mouse.
    private Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    //All these function "OnMouse..." are used to let the player interact with the pieces using
    //the mouse as input:

    //This particular function is not necessary by any means, but seeing all of the piece's legal
    //moves when the mouse hovers over it is a nice feature and also a nice debugging tool.
    private void OnMouseEnter()
    {
        //Debug.Log("test3" + board[28].name + " at " + board[28].transform.position);
        turn = ChessGame.turn;
        if (color == turn)
        {
            List<int> list = FindLegalMoves();
            for (int i = 0; i < list.Count; i++)
            {
                dotGrid[list[i]].SetActive(true);
            }
        }
    }

    //Remove all indicators of legal moves when mouse stops hovering the piece.
    private void OnMouseExit()
    {
        for (int i = 0; i < 64; i++)
        {
            dotGrid[i].SetActive(false);
        }
    }

    //Start dragging the piece.
    private void OnMouseDown()
    {
        mouseOffset = gameObject.transform.position - GetMouseWorldPosition();
        col.enabled = false;
        attempt = FindLegalMoves();
        originalPosition = transform.position;
    }

    //This is when the player (human) makes a move with the mouse.
    private void OnMouseUp()
    {
        //Setting variables:
        col.enabled = true;
        Vector3 pos = transform.position;
        pos.x = Mathf.RoundToInt(GetMouseWorldPosition().x + mouseOffset.x);
        pos.y = Mathf.RoundToInt(GetMouseWorldPosition().y + mouseOffset.y);
        pos.z = Mathf.RoundToInt(GetMouseWorldPosition().z + mouseOffset.z);
        int newPosB = (int)pos.x - 1 + ((int)pos.y - 1) * 8;

        //If the player attempts an illegal moves or tries to move the opponent's pieces, the move
        //isn't allowed and the piece goes back to where it was on the board. Otherwise, let the player
        //make the move and send a special value in case a special move (such as en passant, castling 
        //or pawn promotion) is done.
        if (!attempt.Contains(newPosB) || color != turn)
        {
            transform.position = originalPosition;
        }
        //if castling
        else if (type == "king" && currentPosB - newPosB == 2)
        {
            PlayingMoves.MakeMove(newPosB, gameObject, pos, -2, false);
            ChessAI.RequestMove();
        }
        else if (type == "king" && currentPosB - newPosB == -2)
        {
            PlayingMoves.MakeMove(newPosB, gameObject, pos, -3, false);
            ChessAI.RequestMove();
        }
        else if(type == "pawn" && ((int)(newPosB / 8) == 7 || (int)(newPosB / 8) == 0))
        {
            Debug.Log("before, before the thing");
            PlayingMoves.MakeMove(newPosB, gameObject, pos, 1, false);
            ChessAI.RequestMove();
        }
        else
        {
            PlayingMoves.MakeMove(newPosB, gameObject, pos, 0, false);
            ChessAI.RequestMove();
        }
        board = ChessGame.board;
        //Debug.Log("TESTING HERE: " + board[40].name);
    }

    //Drags the piece around.
    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + mouseOffset;
    }

    //This function returns a list of all the legal moves the piece can do. It starts
    //by finding all the moves the piece can do with FindAllMoves(string) and then removes
    //all the illegal moves from the list with RemoveIllegalsDueToChecking(List<int>).
    public List<int> FindLegalMoves()
    {
        board = BoardGrid.board;
        return RemoveIllegalsDueToChecking(FindAllMoves(type));
    }

    //Find all the moves of the piece, legal or not.
    public List<int> FindAllMoves(string type)
    {
        board = BoardGrid.board;
        List<int> legals = new List<int>();
        int pos = currentPosB;
        switch (type)
        {
            case "pawn":
                if (color == "white")
                {
                    if ((int)pos / 8 != 1)
                    {
                        hasMoved = true;
                    }
                    if (pos + 8 <= 63)
                    {
                        //moving forward
                        if (board[pos + 8] == null)
                        {
                            legals.Add(pos + 8);
                            if (!hasMoved && board[pos + 16] == null)
                            {
                                legals.Add(pos + 16);
                            }
                            //Promotion
                            else if (pos + 8 <= 63 && pos + 8 >= 56)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    legals.Add(pos + 8);
                                }
                            }
                        }
                        //attacking
                        if (pos % 8 >= 1 && board[pos + 7] != null)
                        {
                            if (board[pos + 7].GetComponent<Pieces>().color == "black")
                            {
                                legals.Add(pos + 7);
                                //Promotion
                                if (pos + 8 <= 63 && pos + 8 >= 56)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        legals.Add(pos + 7);
                                    }
                                }
                            }
                        }
                        if (pos % 8 <= 6 && board[pos + 9] != null)
                        {
                            if (board[pos + 9].GetComponent<Pieces>().color == "black")
                            {
                                legals.Add(pos + 9);
                                //Promotion
                                if (pos + 8 <= 63 && pos + 8 >= 56)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        legals.Add(pos + 9);
                                    }
                                }
                            }
                        }
                        //En Passant
                        if (pos % 8 >= 1 && board[pos + 7] == null && board[pos - 1] != null)
                        {
                            if (board[pos - 1].GetComponent<Pieces>().color == "black" && board[pos - 1].GetComponent<Pieces>().type == "pawn" && board[pos - 1].GetComponent<Pieces>().justMovedTwoStepsPawn == true)
                            {
                                legals.Add(pos + 7);
                            }
                        }
                        if (pos % 8 <= 6 && board[pos + 9] == null && board[pos + 1] != null)
                        {
                            if (board[pos + 1].GetComponent<Pieces>().color == "black" && board[pos + 1].GetComponent<Pieces>().type == "pawn" && board[pos + 1].GetComponent<Pieces>().justMovedTwoStepsPawn == true)
                            {
                                legals.Add(pos + 9);
                            }
                        }
                    }
                }
                else if (color == "black")
                {
                    if ((int)pos / 8 != 6)
                    {
                        hasMoved = true;
                    }
                    if (pos - 8 >= 0)
                    {
                        //moving forward
                        if (board[pos - 8] == null)
                        {
                            legals.Add(pos - 8);
                            if (!hasMoved && board[pos - 16] == null)
                            {
                                legals.Add(pos - 16);
                            }
                            //Promotion
                            else if (pos - 8 <= 7 && pos - 8 >= 0)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    legals.Add(pos - 8);
                                }
                            }
                        }
                        //attacking
                        if (pos % 8 <= 6 && board[pos - 7] != null)
                        {
                            if (board[pos - 7].GetComponent<Pieces>().color == "white")
                            {
                                legals.Add(pos - 7);
                                //Promotion
                                if (pos - 8 <= 7 && pos - 8 >= 0)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        legals.Add(pos - 7);
                                    }
                                }
                            }
                        }
                        if (pos % 8 >= 1 && board[pos - 9] != null)
                        {
                            if (board[pos - 9].GetComponent<Pieces>().color == "white")
                            {
                                legals.Add(pos - 9);
                                //Promotion
                                if (pos - 8 <= 7 && pos - 8 >= 0)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        legals.Add(pos - 9);
                                    }
                                }
                            }
                        }
                        //En Passant
                        if (pos % 8 <= 6 && board[pos - 7] == null && board[pos + 1] != null)
                        {
                            if (board[pos + 1].GetComponent<Pieces>().color == "white" && board[pos + 1].GetComponent<Pieces>().type == "pawn" && board[pos + 1].GetComponent<Pieces>().justMovedTwoStepsPawn == true)
                            {
                                legals.Add(pos - 7);
                            }
                        }
                        if (pos % 8 >= 1 && board[pos - 9] == null && board[pos - 1] != null)
                        {
                            if (board[pos - 1].GetComponent<Pieces>().color == "white" && board[pos - 1].GetComponent<Pieces>().type == "pawn" && board[pos - 1].GetComponent<Pieces>().justMovedTwoStepsPawn == true)
                            {
                                legals.Add(pos - 9);
                            }
                        }
                    }
                }
                break;

            case "rook":
                //top
                for (int i = 1; i <= 7 - (int)pos / 8; ++i)
                {
                    if (board[pos + (i * 8)] == null)
                    {
                        legals.Add(pos + i * 8);
                    }
                    else
                    {
                        if (board[pos + i * 8].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos + i * 8);
                        }
                        break;
                    }
                }
                //bottom
                for (int i = 1; i <= (int)pos / 8; ++i)
                {
                    if (board[pos - i * 8] == null)
                    {
                        legals.Add(pos - i * 8);
                    }
                    else
                    {
                        if (board[pos - i * 8].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos - i * 8);
                        }
                        break;
                    }
                }
                //right
                for (int i = 1; i <= 7 - pos % 8; ++i)
                {
                    if (board[pos + i] == null)
                    {
                        legals.Add(pos + i);
                    }
                    else
                    {
                        if (board[pos + i].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos + i);
                        }
                        break;
                    }
                }
                //left
                for (int i = 1; i <= pos % 8; ++i)
                {
                    if (board[pos - i] == null)
                    {
                        legals.Add(pos - i);
                    }
                    else
                    {
                        if (board[pos - i].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos - i);
                        }
                        break;
                    }
                }
                break;

            case "knight":
                int[] knightPos;
                if ((int)pos % 8 == 0)
                {
                    knightPos = new int[] { -15, -6, 10, 17 };
                }
                else if ((int)pos % 8 == 1)
                {
                    knightPos = new int[] { -17, -15, -6, 10, 15, 17 };
                }
                else if ((int)pos % 8 == 6)
                {
                    knightPos = new int[] { -17, -15, -10, 6, 15, 17 };
                }
                else if ((int)pos % 8 == 7)
                {
                    knightPos = new int[] { -17, -10, 6, 15 };
                }
                else
                {
                    knightPos = new int[] { -17, -15, -10, -6, 6, 10, 15, 17 };
                }
                foreach (int i in knightPos)
                {
                    if (pos + i <= 63 && pos + i >= 0)
                    {
                        if (board[pos + i] != null)
                        {
                            if (board[pos + i].GetComponent<Pieces>().color != color)
                            {
                                legals.Add(pos + i);
                            }
                        }
                        else
                        {
                            legals.Add(pos + i);
                        }
                    }
                }
                break;

            case "bishop":
                //top left
                if (pos % 8 != 0)
                {
                    for (int i = 1; i < pos % 8 + 1; ++i)
                    {
                        if (pos + i * 7 <= 62)
                        {
                            if (board[pos + i * 7] == null)
                            {
                                legals.Add(pos + i * 7);
                            }
                            else
                            {
                                if (board[pos + i * 7].GetComponent<Pieces>().color != color)
                                {
                                    legals.Add(pos + i * 7);
                                }
                                break;
                            }
                        }
                    }
                }
                //top right
                if (pos % 8 != 7)
                {
                    for (int i = 1; i < 8 - pos % 8; ++i)
                    {
                        if (pos + i * 9 <= 63)
                        {
                            if (board[pos + i * 9] == null)
                            {
                                legals.Add(pos + i * 9);
                            }
                            else
                            {
                                if (board[pos + i * 9].GetComponent<Pieces>().color != color)
                                {
                                    legals.Add(pos + i * 9);
                                }
                                break;
                            }
                        }
                    }
                }

                //bottom left
                if (pos % 8 != 0)
                {
                    for (int i = 1; i < pos % 8 + 1; ++i)
                    {
                        if (pos - i * 9 >= 0)
                        {
                            if (board[pos - i * 9] == null)
                            {
                                legals.Add(pos - i * 9);
                            }
                            else
                            {
                                if (board[pos - i * 9].GetComponent<Pieces>().color != color)
                                {
                                    legals.Add(pos - i * 9);
                                }
                                break;
                            }
                        }
                    }
                }
                //bottom right
                if (pos % 8 != 7)
                {
                    for (int i = 1; i < 8 - pos % 8; ++i)
                    {
                        if (pos - i * 7 >= 1)
                        {
                            if (board[pos - i * 7] == null)
                            {
                                legals.Add(pos - i * 7);
                            }
                            else
                            {
                                if (board[pos - i * 7].GetComponent<Pieces>().color != color)
                                {
                                    legals.Add(pos - i * 7);
                                }
                                break;
                            }
                        }
                    }
                }
                break;

            case "queen":
                //top left
                if (pos % 8 != 0)
                {
                    for (int i = 1; i < pos % 8 + 1; ++i)
                    {
                        if (pos + i * 7 <= 62)
                        {
                            if (board[pos + i * 7] == null)
                            {
                                legals.Add(pos + i * 7);
                            }
                            else
                            {
                                if (board[pos + i * 7].GetComponent<Pieces>().color != color)
                                {
                                    legals.Add(pos + i * 7);
                                }
                                break;
                            }
                        }
                    }
                }
                //top right
                if (pos % 8 != 7)
                {
                    for (int i = 1; i < 8 - pos % 8; ++i)
                    {
                        if (pos + i * 9 <= 63)
                        {
                            if (board[pos + i * 9] == null)
                            {
                                legals.Add(pos + i * 9);
                            }
                            else
                            {
                                if (board[pos + i * 9].GetComponent<Pieces>().color != color)
                                {
                                    legals.Add(pos + i * 9);
                                }
                                break;
                            }
                        }
                    }
                }

                //bottom left
                if (pos % 8 != 0)
                {
                    for (int i = 1; i < pos % 8 + 1; ++i)
                    {
                        if (pos - i * 9 >= 0)
                        {
                            if (board[pos - i * 9] == null)
                            {
                                legals.Add(pos - i * 9);
                            }
                            else
                            {
                                if (board[pos - i * 9].GetComponent<Pieces>().color != color)
                                {
                                    legals.Add(pos - i * 9);
                                }
                                break;
                            }
                        }
                    }
                }
                //bottom right
                if (pos % 8 != 7)
                {
                    for (int i = 1; i < 8 - pos % 8; ++i)
                    {
                        if (pos - i * 7 >= 1)
                        {
                            if (board[pos - i * 7] == null)
                            {
                                legals.Add(pos - i * 7);
                            }
                            else
                            {
                                if (board[pos - i * 7].GetComponent<Pieces>().color != color)
                                {
                                    legals.Add(pos - i * 7);
                                }
                                break;
                            }
                        }
                    }
                }
                //top
                for (int i = 1; i <= 7 - (int)pos / 8; ++i)
                {
                    if (board[pos + i * 8] == null)
                    {
                        legals.Add(pos + i * 8);
                    }
                    else
                    {
                        if (board[pos + i * 8].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos + i * 8);
                        }
                        break;
                    }
                }
                //bottom
                for (int i = 1; i <= (int)pos / 8; ++i)
                {
                    if (board[pos - i * 8] == null)
                    {
                        legals.Add(pos - i * 8);
                    }
                    else
                    {
                        if (board[pos - i * 8].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos - i * 8);
                        }
                        break;
                    }
                }
                //right
                for (int i = 1; i <= 7 - pos % 8; ++i)
                {
                    if (board[pos + i] == null)
                    {
                        legals.Add(pos + i);
                    }
                    else
                    {
                        if (board[pos + i].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos + i);
                        }
                        break;
                    }
                }
                //left
                for (int i = 1; i <= pos % 8; ++i)
                {
                    if (board[pos - i] == null)
                    {
                        legals.Add(pos - i);
                    }
                    else
                    {
                        if (board[pos - i].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos - i);
                        }
                        break;
                    }
                }
                break;

            case "king":
                //top left
                if (pos + 7 <= 62 && pos % 8 >= 1)
                {
                    if (board[pos + 7] == null)
                    {
                        legals.Add(pos + 7);
                    }
                    else
                    {
                        if (board[pos + 7].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos + 7);
                        }
                    }
                }
                //top right
                if (pos + 9 <= 63 && pos % 8 <= 6)
                {
                    if (board[pos + 9] == null)
                    {
                        legals.Add(pos + 9);
                    }
                    else
                    {
                        if (board[pos + 9].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos + 9);
                        }
                    }
                }

                //bottom left
                if (pos - 9 >= 0 && pos % 8 >= 1)
                {
                    if (board[pos - 9] == null)
                    {
                        legals.Add(pos - 9);
                    }
                    else
                    {
                        if (board[pos - 9].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos - 9);
                        }
                    }
                }
                //bottom right
                if (pos - 7 >= 1 && pos % 8 <= 6)
                {
                    if (board[pos - 7] == null)
                    {
                        legals.Add(pos - 7);
                    }
                    else
                    {
                        if (board[pos - 7].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos - 7);
                        }
                    }
                }
                //top
                if (pos + 8 <= 63)
                {
                    if (board[pos + 8] == null)
                    {
                        legals.Add(pos + 8);
                    }
                    else
                    {
                        if (board[pos + 8].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos + 8);
                        }
                    }
                }
                //bottom
                if (pos - 8 >= 0)
                {
                    if (board[pos - 8] == null)
                    {
                        legals.Add(pos - 8);
                    }
                    else
                    {
                        if (board[pos - 8].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos - 8);
                        }
                    }
                }
                //right
                if (pos + 1 <= 63 && !(((pos + 1) % 8) == 0))
                {
                    if (board[pos + 1] == null)
                    {
                        legals.Add(pos + 1);
                    }
                    else
                    {
                        if (board[pos + 1].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos + 1);
                        }
                    }
                }
                //left
                if (pos - 1 >= 0 && !(((pos - 1) % 8) == 7))
                {
                    if (board[pos - 1] == null)
                    {
                        legals.Add(pos - 1);
                    }
                    else
                    {
                        if (board[pos - 1].GetComponent<Pieces>().color != color)
                        {
                            legals.Add(pos - 1);
                        }
                    }
                }

                //castling
                //white
                if (pos == 4 && color == "white")
                {
                    if (board[7] != null)
                    {
                        if (board[7].name == "White Rook")
                        {
                            if (board[5] == null && board[6] == null)
                            {
                                if (board[7].GetComponent<Pieces>().hasMoved == false && hasMoved == false)
                                {
                                    legals.Add(pos + 2);
                                }
                            }
                        }
                    }
                    if (board[0] != null)
                    {
                        if (board[0].name == "White Rook")
                        {
                            if (board[1] == null && board[2] == null && board[3] == null)
                            {
                                if (board[0].GetComponent<Pieces>().hasMoved == false && hasMoved == false)
                                {
                                    legals.Add(pos - 2);
                                }
                            }
                        }
                    }
                }

                //black
                if (pos == 60 && color == "black")
                {
                    if (board[63] != null)
                    {
                        if (board[63].name == "Black Rook")
                        {
                            if (board[63].GetComponent<Pieces>().hasMoved == false && hasMoved == false)
                            {
                                if (board[61] == null && board[62] == null)
                                {
                                    legals.Add(pos + 2);
                                }
                            }
                        }
                    }
                    if (board[56] != null)
                    {
                        if (board[56].name == "Black Rook")
                        {
                            if (board[56].GetComponent<Pieces>().hasMoved == false && hasMoved == false)
                            {
                                if (board[57] == null && board[58] == null && board[59] == null)
                                {
                                    legals.Add(pos - 2);
                                }
                            }
                        }
                    }
                }
                break;
        }
        return legals;
    }

    //Returns all the moves a piece can do where it can attack.
    public List<int> FindAllProtectingMoves(string typeInput, string colorInput, int posInput)
    {
        board = BoardGrid.board;
        List<int> protectedBy = new List<int>();
        int pos = posInput;
        switch (typeInput)
        {
            case "pawn":
                if (color == "white")
                {
                    if (pos + 8 <= 63)
                    {
                        if (pos % 8 >= 1 && board[pos + 7] != null)
                        {
                            if (board[pos + 7].GetComponent<Pieces>().color == colorInput)
                            {
                                protectedBy.Add(pos + 7);
                            }
                        }
                        if (pos % 8 <= 6 && board[pos + 9] != null)
                        {
                            if (board[pos + 9].GetComponent<Pieces>().color == colorInput)
                            {
                                protectedBy.Add(pos + 9);
                            }
                        }
                    }
                }
                else if (color == "black")
                {
                    if (pos - 8 >= 0)
                    {
                        if (pos % 8 <= 6 && board[pos - 7] != null)
                        {
                            if (board[pos - 7].GetComponent<Pieces>().color == colorInput)
                            {
                                protectedBy.Add(pos - 7);
                            }
                        }
                        if (pos % 8 >= 1 && board[pos - 9] != null)
                        {
                            if (board[pos - 9].GetComponent<Pieces>().color == colorInput)
                            {
                                protectedBy.Add(pos - 9);
                            }
                        }
                    }
                }
                break;

            case "rook":
                //top
                for (int i = 1; i <= 7 - (int)pos / 8; ++i)
                {
                    if (board[pos + (i * 8)] != null)
                    {
                        if (board[pos + i * 8].GetComponent<Pieces>().color == colorInput)
                        {
                            protectedBy.Add(pos + i * 8);
                        }
                        break;
                    }
                }
                //bottom
                for (int i = 1; i <= (int)pos / 8; ++i)
                {
                    if (board[pos - i * 8] != null)
                    {
                        if (board[pos - i * 8].GetComponent<Pieces>().color == colorInput)
                        {
                            protectedBy.Add(pos - i * 8);
                        }
                        break;
                    }
                }
                //right
                for (int i = 1; i <= 7 - pos % 8; ++i)
                {
                    if (board[pos + i] != null)
                    {
                        if (board[pos + i].GetComponent<Pieces>().color == colorInput)
                        {
                            protectedBy.Add(pos + i);
                        }
                        break;
                    }
                }
                //left
                for (int i = 1; i <= pos % 8; ++i)
                {
                    if (board[pos - i] != null)
                    {
                        if (board[pos - i].GetComponent<Pieces>().color == colorInput)
                        {
                            protectedBy.Add(pos - i);
                        }
                        break;
                    }
                }
                break;

            case "knight":
                int[] knightPos;
                if ((int)pos % 8 == 0)
                {
                    knightPos = new int[] { -15, -6, 10, 17 };
                }
                else if ((int)pos % 8 == 1)
                {
                    knightPos = new int[] { -17, -15, -6, 10, 15, 17 };
                }
                else if ((int)pos % 8 == 6)
                {
                    knightPos = new int[] { -17, -15, -10, 6, 15, 17 };
                }
                else if ((int)pos % 8 == 7)
                {
                    knightPos = new int[] { -17, -10, 6, 15 };
                }
                else
                {
                    knightPos = new int[] { -17, -15, -10, -6, 6, 10, 15, 17 };
                }
                foreach (int i in knightPos)
                {
                    if (pos + i <= 63 && pos + i >= 0)
                    {
                        if (board[pos + i] != null)
                        {
                            if (board[pos + i].GetComponent<Pieces>().color == colorInput)
                            {
                                protectedBy.Add(pos + i);
                            }
                        }
                    }
                }
                break;

            case "bishop":
                //top left
                if (pos % 8 != 0)
                {
                    for (int i = 1; i < pos % 8 + 1; ++i)
                    {
                        if (pos + i * 7 <= 62)
                        {
                            if (board[pos + i * 7] != null)
                            {
                                if (board[pos + i * 7].GetComponent<Pieces>().color == colorInput)
                                {
                                    protectedBy.Add(pos + i * 7);
                                }
                                break;
                            }
                        }
                    }
                }
                //top right
                if (pos % 8 != 7)
                {
                    for (int i = 1; i < 8 - pos % 8; ++i)
                    {
                        if (pos + i * 9 <= 63)
                        {
                            if (board[pos + i * 9] != null)
                            {
                                if (board[pos + i * 9].GetComponent<Pieces>().color == colorInput)
                                {
                                    protectedBy.Add(pos + i * 9);
                                }
                                break;
                            }
                        }
                    }
                }

                //bottom left
                if (pos % 8 != 0)
                {
                    for (int i = 1; i < pos % 8 + 1; ++i)
                    {
                        if (pos - i * 9 >= 0)
                        {
                            if (board[pos - i * 9] != null)
                            {
                                if (board[pos - i * 9].GetComponent<Pieces>().color == colorInput)
                                {
                                    protectedBy.Add(pos - i * 9);
                                }
                                break;
                            }
                        }
                    }
                }
                //bottom right
                if (pos % 8 != 7)
                {
                    for (int i = 1; i < 8 - pos % 8; ++i)
                    {
                        if (pos - i * 7 >= 1)
                        {
                            if (board[pos - i * 7] != null)
                            {
                                if (board[pos - i * 7].GetComponent<Pieces>().color == colorInput)
                                {
                                    protectedBy.Add(pos - i * 7);
                                }
                                break;
                            }
                        }
                    }
                }
                break;

            case "queen":
                //top left
                if (pos % 8 != 0)
                {
                    for (int i = 1; i < pos % 8 + 1; ++i)
                    {
                        if (pos + i * 7 <= 62)
                        {
                            if (board[pos + i * 7] != null)
                            {
                                if (board[pos + i * 7].GetComponent<Pieces>().color == colorInput)
                                {
                                    protectedBy.Add(pos + i * 7);
                                }
                                break;
                            }
                        }
                    }
                }
                //top right
                if (pos % 8 != 7)
                {
                    for (int i = 1; i < 8 - pos % 8; ++i)
                    {
                        if (pos + i * 9 <= 63)
                        {
                            if (board[pos + i * 9] != null)
                            {
                                if (board[pos + i * 9].GetComponent<Pieces>().color == colorInput)
                                {
                                    protectedBy.Add(pos + i * 9);
                                }
                                break;
                            }
                        }
                    }
                }

                //bottom left
                if (pos % 8 != 0)
                {
                    for (int i = 1; i < pos % 8 + 1; ++i)
                    {
                        if (pos - i * 9 >= 0)
                        {
                            if (board[pos - i * 9] != null)
                            {
                                if (board[pos - i * 9].GetComponent<Pieces>().color == colorInput)
                                {
                                    protectedBy.Add(pos - i * 9);
                                }
                                break;
                            }
                        }
                    }
                }
                //bottom right
                if (pos % 8 != 7)
                {
                    for (int i = 1; i < 8 - pos % 8; ++i)
                    {
                        if (pos - i * 7 >= 1)
                        {
                            if (board[pos - i * 7] != null)
                            {
                                if (board[pos - i * 7].GetComponent<Pieces>().color == colorInput)
                                {
                                    protectedBy.Add(pos - i * 7);
                                }
                                break;
                            }
                        }
                    }
                }
                //top
                for (int i = 1; i <= 7 - (int)pos / 8; ++i)
                {
                    if (board[pos + (i * 8)] != null)
                    {
                        if (board[pos + i * 8].GetComponent<Pieces>().color == colorInput)
                        {
                            protectedBy.Add(pos + i * 8);
                        }
                        break;
                    }
                }
                //bottom
                for (int i = 1; i <= (int)pos / 8; ++i)
                {
                    if (board[pos - i * 8] != null)
                    {
                        if (board[pos - i * 8].GetComponent<Pieces>().color == colorInput)
                        {
                            protectedBy.Add(pos - i * 8);
                        }
                        break;
                    }
                }
                //right
                for (int i = 1; i <= 7 - pos % 8; ++i)
                {
                    if (board[pos + i] != null)
                    {
                        if (board[pos + i].GetComponent<Pieces>().color == colorInput)
                        {
                            protectedBy.Add(pos + i);
                        }
                        break;
                    }
                }
                //left
                for (int i = 1; i <= pos % 8; ++i)
                {
                    if (board[pos - i] != null)
                    {
                        if (board[pos - i].GetComponent<Pieces>().color == colorInput)
                        {
                            protectedBy.Add(pos - i);
                        }
                        break;
                    }
                }
                break;
        }
        return protectedBy;
    }

    //Returns a list of moves after all the illegal moves were taken out of it.
    List<int> RemoveIllegalsDueToChecking(List<int> list)
    {
        //Creating variables (kings and all legal moves).
        GameObject ourKing = null;
        GameObject oppKing = null;
        List<GameObject> allOppsWithLegalMoves = new List<GameObject>();
        List<int> ourAllLegalMoves = list;
        List<int> oppAllLegalMoves = new List<int>();

        //Assigning and finding correct values for variables.
        for (int i = 0; i < 64; ++i)
        {
            GameObject g = board[i];
            if (g != null)
            {
                Pieces p = g.GetComponent<Pieces>();
                if (p.type == "king")
                {
                    if (p.color == color)
                    {
                        ourKing = g;
                    }
                    else
                    {
                        oppKing = g;
                    }
                }
                if (p.FindAllMoves(p.type) != null)
                {
                    if (p.color != color)
                    {
                        allOppsWithLegalMoves.Add(g);

                        if (p.type != "pawn")
                        {
                            oppAllLegalMoves.AddRange(p.FindAllMoves(p.type));
                        }
                        else
                        {
                            List<int> pawnMoves = p.FindAllMoves(p.type);
                            pawnMoves = pawnMoves.Distinct().ToList();
                            pawnMoves.Remove(p.currentPosB - 16);
                            pawnMoves.Remove(p.currentPosB - 8);
                            pawnMoves.Remove(p.currentPosB + 8);
                            pawnMoves.Remove(p.currentPosB + 16);
                            oppAllLegalMoves.AddRange(pawnMoves);
                        }
                    }
                }
            }
        }

        int ourkingPosB = ourKing.GetComponent<Pieces>().currentPosB;
        int oppKingPosB = oppKing.GetComponent<Pieces>().currentPosB;
        List<int> allOppsWithLegalMovesPos = new List<int>();
        foreach (GameObject g in allOppsWithLegalMoves)
        {
            allOppsWithLegalMovesPos.Add(g.GetComponent<Pieces>().currentPosB);
        }

        //The king is a piece that requires alot of veryfing because due to its nature, it can
        //have many illegal moves depending on different situations.
        if (type == "king")
        {
            //This removes all the moves that are on a square that is being watched by an opposing piece.
            for (int i = ourAllLegalMoves.Count - 1; i >= 0; i--)
            {
                if (oppAllLegalMoves.Contains(ourAllLegalMoves[i]))
                {
                    ourAllLegalMoves.RemoveAt(i);
                }
            }

            //This makes it so the king may not attack a piece if that piece is guarded.
            for (int i = ourAllLegalMoves.Count - 1; i >= 0; i--)
            {
                if (allOppsWithLegalMovesPos.Contains(ourAllLegalMoves[i]) || oppAllLegalMoves.Contains(currentPosB))
                {
                    if (WouldCheckMyselfKing(allOppsWithLegalMoves, ourAllLegalMoves[i]))
                    {
                        ourAllLegalMoves.RemoveAt(i);
                    }
                }
            }

            //This is for a very niche rule of chess: A king may not capture a piece that is guarded
            //EVEN IF the piece guarding it is pinned.
            for (int i = ourAllLegalMoves.Count - 1; i >= 0; i--)
            {
                if (board[ourAllLegalMoves[i]] != null && Math.Abs(currentPosB - ourAllLegalMoves[i]) != 2)
                {
                    foreach (GameObject g in allOppsWithLegalMoves)
                    {
                        Pieces gP = g.GetComponent<Pieces>();
                        List<int> listLoop = new List<int>(FindAllProtectingMoves(gP.type, gP.color, gP.currentPosB));
                        if (listLoop.Count > 0)
                        {
                            if (listLoop.Contains(ourAllLegalMoves[i]))
                            {
                                ourAllLegalMoves.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }

            //This for loop will not let the king castle if it is checked.
            for (int i = ourAllLegalMoves.Count - 1; i >= 0; i--)
            {
                if (oppAllLegalMoves.Contains(currentPosB) && Math.Abs(currentPosB - ourAllLegalMoves[i]) == 2)
                {
                    ourAllLegalMoves.RemoveAt(i);
                }
            }

            //This will not let the king walk into a check by a pawn.
            for (int i = ourAllLegalMoves.Count - 1; i >= 0; i--)
            {
                if (color == "white")
                {
                    if (ourAllLegalMoves[i] <= 55)
                    {
                        if (ourAllLegalMoves[i] % 8 >= 1)
                        {
                            if (board[ourAllLegalMoves[i] + 7] != null)
                            {
                                if (board[ourAllLegalMoves[i] + 7].name == "Black Pawn")
                                {
                                    ourAllLegalMoves.RemoveAt(i);
                                    continue;
                                }
                            }
                        }
                        if (ourAllLegalMoves[i] % 8 <= 6)
                        {
                            if (board[ourAllLegalMoves[i] + 9] != null)
                            {
                                if (board[ourAllLegalMoves[i] + 9].name == "Black Pawn")
                                {
                                    ourAllLegalMoves.RemoveAt(i);
                                    continue;
                                }
                            }
                        }
                    }
                }
                else if (color == "black")
                {
                    if (ourAllLegalMoves[i] >= 8)
                    {
                        if (ourAllLegalMoves[i] % 8 >= 1)
                        {
                            if (board[ourAllLegalMoves[i] - 9] != null)
                            {
                                if (board[ourAllLegalMoves[i] - 9].name == "White Pawn")
                                {
                                    ourAllLegalMoves.RemoveAt(i);
                                    continue;
                                }
                            }
                        }
                        if (ourAllLegalMoves[i] % 8 <= 6)
                        {
                            if (board[ourAllLegalMoves[i] - 7] != null)
                            {
                                if (board[ourAllLegalMoves[i] - 7].name == "White Pawn")
                                {
                                    ourAllLegalMoves.RemoveAt(i);
                                    continue;
                                }
                            }
                        }
                    }
                }
            }

            //This will not let the king castle if the position that the king watns to skips over is being watched.
            for (int i = ourAllLegalMoves.Count - 1; i >= 0; i--)
            {
                if (color == "white")
                {
                    //Castling
                    if (ourkingPosB == 4)
                    {
                        if (ourAllLegalMoves[i] == 6 && !ourAllLegalMoves.Contains(5) || ourAllLegalMoves[i] == 2 && !ourAllLegalMoves.Contains(3))
                        {
                            ourAllLegalMoves.RemoveAt(i);
                            continue;
                        }
                    }
                }
                else
                {
                    //Castling
                    if (ourkingPosB == 60)
                    {
                        if (ourAllLegalMoves[i] == 62 && !ourAllLegalMoves.Contains(61) || ourAllLegalMoves[i] == 58 && !ourAllLegalMoves.Contains(59))
                        {
                            ourAllLegalMoves.RemoveAt(i);
                            continue;
                        }
                    }
                }
            }
        }

        //This is for when the piece isn't a king: Checking if we are pinned.
        if (type != "king")
        {
            List<int> amount = AreWePinned(oppAllLegalMoves, allOppsWithLegalMoves, ourkingPosB);
            List<int> output = new List<int>();
            if (amount.Count != 0)
            {
                if (amount.Count == 1)
                {
                    if (ourAllLegalMoves.Contains(amount[0]))
                    {
                        output.Add(amount[0]);
                    }
                    foreach (int myMove in ourAllLegalMoves)
                    {
                        if (AreBetween(ourkingPosB, amount[0], myMove))
                        {
                            output.Add(myMove);
                        }
                    }
                    List<int> returnedOutput = output.Distinct().ToList();
                    ourAllLegalMoves = returnedOutput;
                }
                else
                {
                    return new List<int>();
                }
            }
        }

        //Trimming the list of GameObject so we only have the ones checking our king.
        for (int i = allOppsWithLegalMoves.Count - 1; i >= 0; i--)
        {
            Pieces p = allOppsWithLegalMoves[i].GetComponent<Pieces>();
            if (!p.FindAllMoves(p.type).Contains(ourkingPosB))
            {
                allOppsWithLegalMoves.RemoveAt(i);
            }
        }
        //Make list of moves of opponent's pieces that are checking our king.
        List<int> oppsMovePos = new List<int>();
        for (int i = 0; i < allOppsWithLegalMoves.Count; ++i)
        {
            Pieces p = allOppsWithLegalMoves[i].GetComponent<Pieces>();
            oppsMovePos.AddRange(p.FindAllMoves(p.type));
        }

        //Make list of position of opponent's that are checking our king.
        List<int> oppsPos = new List<int>();
        for (int i = 0; i < allOppsWithLegalMoves.Count; ++i)
        {
            Pieces p = allOppsWithLegalMoves[i].GetComponent<Pieces>();
            oppsPos.Add(p.currentPosB);
        }

        //Check if we are letting our opponent check our king.
        if (allOppsWithLegalMoves.Count != 0 && type != "king")
        {
            for (int i = ourAllLegalMoves.Count - 1; i >= 0; i--)
            {
                if (oppsPos.Count > 1)
                {
                    if (!WouldPreventCheck(ourkingPosB, oppsPos, ourAllLegalMoves[i], oppsMovePos))
                    {
                        ourAllLegalMoves.RemoveAt(i);
                    }
                }
                //if yes, remove all illegal moves (those moves would be the ones that don't stop the check).
                else if (!(oppsPos.Contains(ourAllLegalMoves[i]) || WouldPreventCheck(ourkingPosB, oppsPos, ourAllLegalMoves[i], oppsMovePos)))
                {
                    ourAllLegalMoves.RemoveAt(i);
                }
            }
        }

        return ourAllLegalMoves;
    }

    //Verify if the king is walking into a check when trying to capture a piece.
    bool WouldCheckMyselfKing(List<GameObject> allOpps, int attempt)
    {
        bool output = false;
        GameObject temp = board[attempt];
        BoardGrid.board[attempt] = gameObject;
        BoardGrid.board[currentPosB] = null;
        int tempPos = currentPosB;
        currentPosB = attempt;
        foreach (GameObject opp in allOpps)
        {
            Pieces foreachPieces = opp.GetComponent<Pieces>();
            List<int> list = foreachPieces.FindAllMoves(foreachPieces.type);
            list = list.Distinct().ToList();
            if(foreachPieces.type == "pawn")
            {
                list.Remove(foreachPieces.currentPosB - 8);
                list.Remove(foreachPieces.currentPosB + 8);
            }
            if (list.Contains(attempt))
            {
                output = true;
                break;
            }
        }
        currentPosB = tempPos;
        BoardGrid.board[attempt] = temp;
        BoardGrid.board[currentPosB] = gameObject;

        return output;
    }

    //This function checks if a piece is trying to go between an opposing piece and it's king.
    bool AreBetween(int king, int opp, int attempt)
    {
        if (opp % 8 == king % 8 && opp % 8 == attempt % 8 || (int)(opp / 8) == (int)(king / 8) && (int)(opp / 8) == (int)(attempt / 8) ||
                Math.Abs(opp - king) % 7 == 0 && Math.Abs(opp - attempt) % 7 == 0 && Math.Abs(attempt - king) % 7 == 0 && (int)(opp / 8) != (int)(king / 8) && (int)(opp / 8) != (int)(attempt / 8) && ((opp % 8) - (king % 8) > 0 && (opp % 8) - (attempt % 8) > 0 && Math.Abs((opp % 8) - (king % 8)) > Math.Abs((opp % 8) - (attempt % 8)) || (opp % 8) - (king % 8) < 0 && (opp % 8) - (attempt % 8) < 0 && Math.Abs((opp % 8) - (king % 8)) > Math.Abs((opp % 8) - (attempt % 8))) ||
                Math.Abs(opp - king) % 9 == 0 && Math.Abs(opp - attempt) % 9 == 0 && Math.Abs(attempt - king) % 9 == 0 && (int)(opp / 8) != (int)(king / 8) && (int)(opp / 8) != (int)(attempt / 8) && ((opp % 8) - (king % 8) > 0 && (opp % 8) - (attempt % 8) > 0 && Math.Abs((opp % 8) - (king % 8)) > Math.Abs((opp % 8) - (attempt % 8)) || (opp % 8) - (king % 8) < 0 && (opp % 8) - (attempt % 8) < 0 && Math.Abs((opp % 8) - (king % 8)) > Math.Abs((opp % 8) - (attempt % 8))))
        {
            if (Math.Abs(opp - king) > Math.Abs(opp - attempt))
            {
                if (opp - attempt > 0 && opp - king > 0 || opp - attempt < 0 && opp - king < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    //Check if the move would prevent it's king from being checked.
    bool WouldPreventCheck(int king, List<int> oppPos, int attempt, List<int> oppMovePos)
    {

        List<bool> results = new List<bool>();

        foreach (int opp in oppPos)
        {
            if (type != "king")
            {
                if (opp % 8 == king % 8 && opp % 8 == attempt % 8 || (int)(opp / 8) == (int)(king / 8) && (int)(opp / 8) == (int)(attempt / 8) ||
                Math.Abs(opp - king) % 7 == 0 && Math.Abs(opp - attempt) % 7 == 0 && Math.Abs(king - attempt) % 7 == 0 && (int)(opp / 8) != (int)(king / 8) && (int)(opp / 8) != (int)(attempt / 8) && ((opp % 8) - (king % 8) > 0 && (opp % 8) - (attempt % 8) > 0 && Math.Abs((opp % 8) - (king % 8)) > Math.Abs((opp % 8) - (attempt % 8)) || (opp % 8) - (king % 8) < 0 && (opp % 8) - (attempt % 8) < 0 && Math.Abs((opp % 8) - (king % 8)) > Math.Abs((opp % 8) - (attempt % 8))) ||
                Math.Abs(opp - king) % 9 == 0 && Math.Abs(opp - attempt) % 9 == 0 && Math.Abs(king - attempt) % 9 == 0 && (int)(opp / 8) != (int)(king / 8) && (int)(opp / 8) != (int)(attempt / 8) && ((opp % 8) - (king % 8) > 0 && (opp % 8) - (attempt % 8) > 0 && Math.Abs((opp % 8) - (king % 8)) > Math.Abs((opp % 8) - (attempt % 8)) || (opp % 8) - (king % 8) < 0 && (opp % 8) - (attempt % 8) < 0 && Math.Abs((opp % 8) - (king % 8)) > Math.Abs((opp % 8) - (attempt % 8))))
                {
                    if (Math.Abs(opp - king) > Math.Abs(opp - attempt))
                    {
                        if (opp - attempt > 0 && opp - king > 0 || opp - attempt < 0 && opp - king < 0)
                        {
                            results.Add(true);
                        }
                        else
                        {
                            results.Add(false);
                        }

                    }
                    else
                    {
                        results.Add(false);
                    }
                }
                else
                {
                    results.Add(false);
                }


            }
            else
            {
                if (oppMovePos.Contains(attempt))
                {
                    results.Add(false);
                }
                else if (opp % 8 == king % 8 && opp % 8 == attempt % 8 || (int)(opp / 8) == (int)(king / 8) && (int)(opp / 8) == (int)(attempt / 8) ||
                Math.Abs(opp - king) == 7 && Math.Abs(opp - attempt) % 7 == 0 || Math.Abs(opp - king) % 9 == 0 && Math.Abs(opp - attempt) % 9 == 0 || oppMovePos.Contains(attempt))
                {
                    results.Add(false);
                }
            }
        }

        bool final = true;

        foreach (bool b in results)
        {
            if (!b)
            {
                final = false;
            }
        }
        return final;
    }

    //This function tells if the piece is pinned or not.
    List<int> AreWePinned(List<int> oppMoves, List<GameObject> oppsInput, int king)
    {
        List<GameObject> opps = new List<GameObject>(oppsInput);
        List<bool> truths = new List<bool>();
        List<int> oppsAttackingPos = new List<int>();
        //int ourPos = (int)transform.position.x - 1 + ((int)transform.position.y - 1) * 8;
        int ourPos = currentPosB;

        //If this piece is threatened.
        if (oppMoves.Contains(ourPos))
        {
            //Trim opponents list so we only have the ones threatening our piece.
            for (int i = opps.Count - 1; i >= 0; i--)
            {
                Pieces p = opps[i].GetComponent<Pieces>();
                if (!p.FindAllMoves(p.type).Contains(ourPos) || p.type == "knight" || p.type == "pawn" || p.type == "king")
                {
                    opps.RemoveAt(i);
                }
            }
            //Check if the oppenent's piece is pinning our piece.
            foreach (GameObject opponent in opps)
            {
                bool isBeingPinned = true;
                int opp = opponent.GetComponent<Pieces>().currentPosB;
                int multiplier = 0;
                //On same column:
                if (opp % 8 == king % 8 && opp % 8 == ourPos % 8)
                {
                    if (!((int)(opp / 8) > (int)(ourPos / 8) && (int)(ourPos / 8) > (int)(king / 8) || (int)(opp / 8) < (int)(ourPos / 8) && (int)(ourPos / 8) < (int)(king / 8)))
                    {
                        multiplier = 0;
                    }
                    multiplier = 8;
                }
                //On same row:
                else if ((int)(opp / 8) == (int)(king / 8) && (int)(opp / 8) == (int)(ourPos / 8))
                {
                    if (!(opp % 8 > ourPos % 8 && ourPos % 8 > king % 8 || opp % 8 < ourPos % 8 && ourPos % 8 < king % 8))
                    {
                        multiplier = 0;
                    }
                    multiplier = 1;
                }
                //On same diagonal \:
                else if (Math.Abs(opp - king) % 7 == 0 && Math.Abs(opp - ourPos) % 7 == 0)
                {
                    multiplier = 7;
                    if (!(opp % 8 > ourPos % 8 && ourPos % 8 > king % 8 || opp % 8 < ourPos % 8 && ourPos % 8 < king % 8))
                    {
                        multiplier = 0;
                    }
                }
                //On same diagonal /:
                else if (Math.Abs(opp - king) % 9 == 0 && Math.Abs(opp - ourPos) % 9 == 0)
                {
                    multiplier = 9;
                    if (!(opp % 8 > ourPos % 8 && ourPos % 8 > king % 8 || opp % 8 < ourPos % 8 && ourPos % 8 < king % 8))
                    {
                        multiplier = 0;
                    }
                }
                if (king < ourPos)
                {
                    multiplier *= -1;
                }

                if (multiplier != 0)
                {
                    for (int i = 1; i < Math.Abs(king - ourPos) / Math.Abs(multiplier); ++i)
                    {
                        if (board[ourPos + (i * multiplier)] != null)
                        {
                            isBeingPinned = false;
                            break;
                        }
                    }
                }
                else
                {
                    isBeingPinned = false;
                }

                if (isBeingPinned)
                {
                    oppsAttackingPos.Add(opp);
                }
                truths.Add(isBeingPinned);
            }
        }


        return oppsAttackingPos;
    }
}
