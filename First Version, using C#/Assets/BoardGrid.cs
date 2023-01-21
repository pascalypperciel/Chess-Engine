using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGrid : MonoBehaviour
{
    //Sets variables.
    public static GameObject[] board, dotGrid;
    public Sprite dotSprite;
    void Start()
    {
        board = new GameObject[64];
        dotGrid = new GameObject[64];
        CreateDotGrid();
    }

    //Creates the dot grid. Those dots activate to show a piece's legal moves.
    void CreateDotGrid()
    {
        for(int i = 0; i < 8; ++i)
        {
            for(int j = 0; j < 8; ++j)
            {
                GameObject dot = new GameObject("Dot", typeof(SpriteRenderer));
                dot.GetComponent<SpriteRenderer>().sprite = dotSprite;
                dot.GetComponent<SpriteRenderer>().color = new Vector4(.46f,.46f,.46f,.5f);
                dot.transform.position = new Vector3(j + 1, i + 1, -2);
                dot.transform.localScale = new Vector3(0.3f, 0.3f, 1);
                dotGrid[j + i * 8] = dot;
                dot.SetActive(false);
            }
        }    
    }

    //Resets the board
    public static void ResetBoard()
    {
        board = new GameObject[64];
    }
}
