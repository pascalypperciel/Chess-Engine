using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;

public class ChessAgent : Agent
{
    [SerializeField]
    bool isWhite;

    string agentColor;

    public override void CollectObservations(VectorSensor sensor)
    {
        for(int i = 0; i < 64; i++)
        {
            if(ChessGame.board[i] != null)
            {
                sensor.AddObservation(ChessGame.board[i].GetComponent<Pieces>().GetID());
            }
            else
            {
                sensor.AddObservation(0);
            }
        }
        sensor.AddObservation(isWhite);
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        Debug.Log("called too as turn is: " + ChessGame.turn);
        agentColor = isWhite ? "white" : "black";
        for (int i = 0; i < 64; i++)
        {
            bool allowed1 = false;
            bool allowed2 = false;
            List<int> moves = new List<int>();
            GameObject gO = ChessGame.board[i];

            actionMask.SetActionEnabled(1, 1, false);
            actionMask.SetActionEnabled(1, 2, false);
            actionMask.SetActionEnabled(1, 3, false);
            actionMask.SetActionEnabled(1, 4, false);

            //delte this
            actionMask.SetActionEnabled(0, 0, false);

            if (gO != null && gO.GetComponent<Pieces>().color == agentColor)
            {
                allowed1 = true;
                Pieces piece = gO.GetComponent<Pieces>();
                moves = piece.FindLegalMoves();
                if(moves.Count != moves.Distinct().ToList().Count && piece.type == "pawn")
                {
                    allowed2 = true;
                }
                moves = moves.Distinct().ToList();
            }
            for (int j = 0; j < 64; j++)
            {
                if(allowed1 && moves.Contains(j))
                {
                    //Debug.Log("allow it : " + (i * 64 + j));
                    //actionMask.SetActionEnabled(0, i * 64 + j, true);
                }
                else
                {
                    //Debug.Log("masking it: " + (i * 64 + j));
                    actionMask.SetActionEnabled(0, i * 64 + j, false);
                }

                if(allowed2)
                {
                    //need to make it so cant be 0 if pawn
                    actionMask.SetActionEnabled(1, 1, true);
                    actionMask.SetActionEnabled(1, 2, true);
                    actionMask.SetActionEnabled(1, 3, true);
                    actionMask.SetActionEnabled(1, 4, true);
                }
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //initializing stuff
        int number = actions.DiscreteActions[0];
        int selectedPiecePos = (int)(number / 64);
        int wherePieceIsGoing = number - 64 * selectedPiecePos;

        Debug.Log("Piece selected by black: " + ChessGame.board[selectedPiecePos].name + " at " + selectedPiecePos + ". number output is: " + number + " and is going there: " + wherePieceIsGoing + " and the type is: " + ChessGame.board[selectedPiecePos].GetComponent<Pieces>().type);
        //bool isPawn = ChessGame.board[selectedPiecePos].GetComponent<Pieces>().type == "pawn";
        bool isPawn = false;

        if(ChessGame.board[selectedPiecePos].GetComponent<Pieces>().type == "pawn")
        {
            Debug.Log("it's a pawn.");
            isPawn = true;
        }

        int promotion;
        if (isPawn && ((int)(wherePieceIsGoing / 8) == 7 || (int)(wherePieceIsGoing / 8) == 0))
        {
            Debug.Log("gonna promote");
            //i don't know if thats the best way to do it
            if(actions.DiscreteActions[1] == 0)
            {
                promotion = 1;
            }
            else
            {
                promotion = actions.DiscreteActions[1];
            }
        }
        else
        {
            promotion = 0;
        }

        //make move
        MovesToTrainAI.MakeMove(selectedPiecePos, wherePieceIsGoing, promotion, true);
    }

    public override void OnEpisodeBegin()
    {
        ChessGame.turn = "white";
        if(!isWhite)
        {
            MovesToTrainAI.OpponentToMove();
        }
        else
        {
            RequestDecision();
        }
    }
}
