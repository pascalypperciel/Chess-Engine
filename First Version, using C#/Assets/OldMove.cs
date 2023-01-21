using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class OldMove
{
    public GameObject pieceYouJustMoved, pieceWeJustTook;
    public int whereItWas, whereItIsNow;
    public bool beforeEnPassant;

    public OldMove(GameObject gO, int was, int isNow, GameObject victim, bool enPassant)
    {
        pieceYouJustMoved = gO;
        whereItWas = was;
        whereItIsNow = isNow;
        pieceWeJustTook = victim;
        beforeEnPassant = enPassant;
    }
}
