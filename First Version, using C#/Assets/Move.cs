using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Move
{
    public int posWhereYouWannaGo;
    public GameObject pieceYouWannaMove;
    public Vector3 whereYouAreRightNow;
    public int promoting;
    public int undoCounter;
    public int ply;

    public Move(int pos, GameObject gO, Vector3 pos3, int _promoting, int _ply)
    {
        posWhereYouWannaGo = pos;
        pieceYouWannaMove = gO;
        whereYouAreRightNow = pos3;
        promoting = _promoting;
        undoCounter = 0;
        ply = _ply;
    }

    public void AddToUndoCounter()
    {
        undoCounter++;
    }

}
