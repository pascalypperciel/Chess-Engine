using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR

using UnityEditor;

#endif

//This function is the "main" function as it is the first one being called and dictates what scripts are
//called depending on desired game mode.
public class ChessGame : MonoBehaviour
{
    //Variables and constants
    enum Mode
    {
        Play,
        Perft,
        AI_Training
    };

    [SerializeField]
    static Mode mode = new Mode();

    [SerializeField]
    string fen;

    [SerializeField]
    int ply;

    [SerializeField]
    public GameObject chessAgentGO;
    static ChessAgent chessAgent;

    const string DefaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    static string[] fenParts;

    public static Sprite[] sprites;
    public static GameObject[] board;
    public static string turn;
    public static List<OldMove> movesList;

    private void Awake()
    {
        if(fen == "")
        {
            fen = DefaultFEN;
        }
        sprites = new Sprite[12];
        sprites[0] = Resources.Load<Sprite>("bb");
        sprites[1] = Resources.Load<Sprite>("bw");
        sprites[2] = Resources.Load<Sprite>("kb");
        sprites[3] = Resources.Load<Sprite>("kw");
        sprites[4] = Resources.Load<Sprite>("nb");
        sprites[5] = Resources.Load<Sprite>("nw");
        sprites[6] = Resources.Load<Sprite>("pb");
        sprites[7] = Resources.Load<Sprite>("pw");
        sprites[8] = Resources.Load<Sprite>("qb");
        sprites[9] = Resources.Load<Sprite>("qw");
        sprites[10] = Resources.Load<Sprite>("rb");
        sprites[11] = Resources.Load<Sprite>("rw");
        fenParts = FENDecoder(fen);
    }
    void Start()
    {
        chessAgent = chessAgentGO.GetComponent<ChessAgent>();
        movesList = new List<OldMove>();
        PositionPieces(fenParts);
        if(mode == Mode.Perft)
        {
            Debug.Log("Perft result is: " + Perft.MovesTest(ply));
        }
        else if(mode == Mode.AI_Training)
        {
            chessAgent.OnEpisodeBegin();
        }
    }

    //This function is to separate the different portions of the input fen so my code can use it.
    static string[] FENDecoder (string fenInput)
    {
        char[] separators = new char[] { ' ', '/' };
        string[] list = fenInput.Split(separators);
        Array.Reverse(list, 0, 8);
        if (list[8] == "w")
        {
            list[8] = "white";
        }
        else if(list[8] == "b")
        {
            list[8] = "black";
        }
        return list;
    }

    //This function sets the pieces on the board according that the inputted fen. This function is called at the very beginning
    //or when the board is reset.
    static void PositionPieces(string[] fenParts)
    {
        turn = fenParts[8];
        BoardGrid.ResetBoard();
        board = BoardGrid.board;
        int counter = 0;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < fenParts[i].Length; ++j)
            {
                if (char.IsNumber(fenParts[i][j]))
                {
                    counter += (int)Char.GetNumericValue(fenParts[i][j]);
                }
                else
                {
                    switch (fenParts[i][j])
                    {
                        case 'p':
                            GameObject blackPawn = new GameObject("Black Pawn", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Pieces));
                            blackPawn.GetComponent<SpriteRenderer>().sprite = sprites[6];
                            blackPawn.transform.position = new Vector3(counter + 1, i + 1, -1);
                            blackPawn.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                            blackPawn.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                            board[counter + i * 8] = blackPawn;
                            Pieces blackPawnPieces = blackPawn.GetComponent<Pieces>();
                            blackPawnPieces.color = "black";
                            blackPawnPieces.type = "pawn";
                            blackPawnPieces.fenCode = "p";
                            blackPawnPieces.currentPosB = counter + i * 8;
                            break;
                        case 'r':
                            GameObject blackRook = new GameObject("Black Rook", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Pieces));
                            blackRook.GetComponent<SpriteRenderer>().sprite = sprites[10];
                            blackRook.transform.position = new Vector3(counter + 1, i + 1, -1);
                            blackRook.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                            blackRook.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                            board[counter + i * 8] = blackRook;
                            Pieces blackRookPieces = blackRook.GetComponent<Pieces>();
                            blackRookPieces.color = "black";
                            blackRookPieces.type = "rook";
                            blackRookPieces.fenCode = "r";
                            blackRookPieces.currentPosB = counter + i * 8;
                            break;
                        case 'n':
                            GameObject blackKnight = new GameObject("Black Knight", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Pieces));
                            blackKnight.GetComponent<SpriteRenderer>().sprite = sprites[4];
                            blackKnight.transform.position = new Vector3(counter + 1, i + 1, -1);
                            blackKnight.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                            blackKnight.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                            board[counter + i * 8] = blackKnight;
                            Pieces blackKnightPieces = blackKnight.GetComponent<Pieces>();
                            blackKnightPieces.color = "black";
                            blackKnightPieces.type = "knight";
                            blackKnightPieces.fenCode = "n";
                            blackKnightPieces.currentPosB = counter + i * 8;
                            break;
                        case 'b':
                            GameObject blackBishop = new GameObject("Black Bishop", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Pieces));
                            blackBishop.GetComponent<SpriteRenderer>().sprite = sprites[0];
                            blackBishop.transform.position = new Vector3(counter + 1, i + 1, -1);
                            blackBishop.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                            blackBishop.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                            board[counter + i * 8] = blackBishop;
                            Pieces blackBishopPieces = blackBishop.GetComponent<Pieces>();
                            blackBishopPieces.color = "black";
                            blackBishopPieces.type = "bishop";
                            blackBishopPieces.fenCode = "b";
                            blackBishopPieces.currentPosB = counter + i * 8;
                            break;
                        case 'q':
                            GameObject blackQueen = new GameObject("Black Queen", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Pieces));
                            blackQueen.GetComponent<SpriteRenderer>().sprite = sprites[8];
                            blackQueen.transform.position = new Vector3(counter + 1, i + 1, -1);
                            blackQueen.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                            blackQueen.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                            board[counter + i * 8] = blackQueen;
                            Pieces blackQueenPieces = blackQueen.GetComponent<Pieces>();
                            blackQueenPieces.color = "black";
                            blackQueenPieces.type = "queen";
                            blackQueenPieces.fenCode = "q";
                            blackQueenPieces.currentPosB = counter + i * 8;
                            break;
                        case 'k':
                            GameObject blackKing = new GameObject("Black King", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Pieces));
                            blackKing.GetComponent<SpriteRenderer>().sprite = sprites[2];
                            blackKing.transform.position = new Vector3(counter + 1, i + 1, -1);
                            blackKing.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                            blackKing.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                            board[counter + i * 8] = blackKing;
                            Pieces blackKingPieces = blackKing.GetComponent<Pieces>();
                            blackKingPieces.color = "black";
                            blackKingPieces.type = "king";
                            blackKingPieces.fenCode = "k";
                            blackKingPieces.currentPosB = counter + i * 8;
                            break;
                        case 'P':
                            GameObject whitePawn = new GameObject("White Pawn", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Pieces));
                            whitePawn.GetComponent<SpriteRenderer>().sprite = sprites[7];
                            whitePawn.transform.position = new Vector3(counter + 1, i + 1, -1);
                            whitePawn.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                            whitePawn.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                            board[counter + i * 8] = whitePawn;
                            Pieces whitePawnPieces = whitePawn.GetComponent<Pieces>();
                            whitePawnPieces.color = "white";
                            whitePawnPieces.type = "pawn";
                            whitePawnPieces.fenCode = "P";
                            whitePawnPieces.currentPosB = counter + i * 8;
                            break;
                        case 'R':
                            GameObject whiteRook = new GameObject("White Rook", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Pieces));
                            whiteRook.GetComponent<SpriteRenderer>().sprite = sprites[11];
                            whiteRook.transform.position = new Vector3(counter + 1, i + 1, -1);
                            whiteRook.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                            whiteRook.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                            board[counter + i * 8] = whiteRook;
                            Pieces whiteRookPieces = whiteRook.GetComponent<Pieces>();
                            whiteRookPieces.color = "white";
                            whiteRookPieces.type = "rook";
                            whiteRookPieces.fenCode = "R";
                            whiteRookPieces.currentPosB = counter + i * 8;
                            break;
                        case 'N':
                            GameObject whiteKnight = new GameObject("White Knight", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Pieces));
                            whiteKnight.GetComponent<SpriteRenderer>().sprite = sprites[5];
                            whiteKnight.transform.position = new Vector3(counter + 1, i + 1, -1);
                            whiteKnight.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                            whiteKnight.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                            board[counter + i * 8] = whiteKnight;
                            Pieces whiteKnightPieces = whiteKnight.GetComponent<Pieces>();
                            whiteKnightPieces.color = "white";
                            whiteKnightPieces.type = "knight";
                            whiteKnightPieces.fenCode = "N";
                            whiteKnightPieces.currentPosB = counter + i * 8;
                            break;
                        case 'B':
                            GameObject whiteBishop = new GameObject("White Bishop", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Pieces));
                            whiteBishop.GetComponent<SpriteRenderer>().sprite = sprites[1];
                            whiteBishop.transform.position = new Vector3(counter + 1, i + 1, -1);
                            whiteBishop.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                            whiteBishop.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                            board[counter + i * 8] = whiteBishop;
                            Pieces whiteBishopPieces = whiteBishop.GetComponent<Pieces>();
                            whiteBishopPieces.color = "white";
                            whiteBishopPieces.type = "bishop";
                            whiteBishopPieces.fenCode = "B";
                            whiteBishopPieces.currentPosB = counter + i * 8;
                            break;
                        case 'Q':
                            GameObject whiteQueen = new GameObject("White Queen", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Pieces));
                            whiteQueen.GetComponent<SpriteRenderer>().sprite = sprites[9];
                            whiteQueen.transform.position = new Vector3(counter + 1, i + 1, -1);
                            whiteQueen.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                            whiteQueen.GetComponent<BoxCollider2D>().size = new Vector2 (0.6f, 0.6f);
                            board[counter + i * 8] = whiteQueen;
                            Pieces whiteQueenPieces = whiteQueen.GetComponent<Pieces>();
                            whiteQueenPieces.color = "white";
                            whiteQueenPieces.type = "queen";
                            whiteQueenPieces.fenCode = "Q";
                            whiteQueenPieces.currentPosB = counter + i * 8;
                            break;
                        case 'K':
                            GameObject whiteKing = new GameObject("White King", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Pieces));
                            whiteKing.GetComponent<SpriteRenderer>().sprite = sprites[3];
                            whiteKing.transform.position = new Vector3(counter + 1, i + 1, -1);
                            whiteKing.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                            whiteKing.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                            board[counter + i * 8] = whiteKing;
                            Pieces whiteKingPieces = whiteKing.GetComponent<Pieces>();
                            whiteKingPieces.color = "white";
                            whiteKingPieces.type = "king";
                            whiteKingPieces.fenCode = "K";
                            whiteKingPieces.currentPosB = counter + i * 8;
                            break;
                    }
                    ++counter;
                }
            }
            counter = 0;
        }
    }

    //Changes the turn. This function is called after a move is made.
    public static void NewTurn()
    {
        if (turn == "white")
        {
            turn = "black";
        }
        else
        {
            turn = "white";
        }
    }

    //Resets the board by destroying all the pieces on the board, and then repositioning the pieces in the inital position
    //with the PositionPieces() function.
    public static void ResetBoard()
    {
        foreach (GameObject gameObj in FindObjectsOfType(typeof(GameObject)))
        {
            if (gameObj.GetComponent<Pieces>() != null)
            {
                Destroy(gameObj.gameObject);
            }
        }

        PositionPieces(fenParts);
    }
    public static void ResetBoard(string fenInput)
    {
        foreach (GameObject gameObj in FindObjectsOfType(typeof(GameObject)))
        {
            if (gameObj.GetComponent<Pieces>() != null)
            {
                Destroy(gameObj.gameObject);
            }
        }
        PositionPieces(FENDecoder(fenInput));
    }

    //This function returns a fen (only the position component of it) according to the current board).
    public static string GenerateFEN(GameObject[] board)
    {
        string fen = "";
        for(int i = 0; i < 8; ++i)
        {
            int counter = 0;
            for (int j = 0; j < 8; ++j)
            {
                if(board[i * 8 + j] != null)
                {
                    if(counter != 0)
                    {
                        fen = fen + counter;
                        counter = 0;
                    }
                    fen = fen + board[i * 8 + j].GetComponent<Pieces>().fenCode;
                }
                else
                {
                    counter++;
                }

                if(j == 7 & counter != 0)
                {
                    fen = fen + counter;
                }
            }
            if(i != 7)
            {

                fen = fen + "/";
            }
        }

        fen = ReverseFen(fen);
        //Debug.Log("Output fen: " + fen);
        return fen;
    }

    //This function reverses the fen so I can position the pieces from a1 to h8 instead of h1 to a8.
    public static string ReverseFen(string fenToReverse)
    {
        //Can implement in PositionPieces
        string[] ranks = fenToReverse.Split('/');
        Array.Reverse(ranks);
        string output = "";
        int counter = 0;
        foreach(string rank in ranks)
        {
            ++counter;
            output = output + rank;
            if(counter != 8)
            {
                output = output + "/";
            }
        }

        return output;
    }

    //This board converts a position in the List<int> of positions to algebraic notation (such as a1, b4, etc...)
    public static string ChangeFormatPos(int pos)
    {
        string output = "";
        //column
        switch(pos % 8)
        {
            case 0:
                output = "a";
                break;

            case 1:
                output = "b";
                break;

            case 2:
                output = "c";
                break;

            case 3:
                output = "d";
                break;

            case 4:
                output = "e";
                break;

            case 5:
                output = "f";
                break;

            case 6:
                output = "g";
                break;

            case 7:
                output = "h";
                break;
        }
        //row
        output = output + ((int)pos / 8 + 1).ToString();

        return output;
    }
}
