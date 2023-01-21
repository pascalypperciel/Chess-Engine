using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionMenu : MonoBehaviour
{
    public int choice = 0;
    GameObject piece;

    private void OnEnable()
    {
        piece = null;
        string color = ChessGame.turn;
        if (color == "white")
        {
            //Queen
            piece = new GameObject("White Queen", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(PromotionSelection));
            piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[9];
            piece.transform.position = new Vector3(3, 4.5f, -1);
            piece.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            piece.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);

            //Rook
            piece = new GameObject("White Rook", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(PromotionSelection));
            piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[11];
            piece.transform.position = new Vector3(3, 4.5f, -1);
            piece.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            piece.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);

            //Bishop
            piece = new GameObject("White Bishop", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(PromotionSelection));
            piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[1];
            piece.transform.position = new Vector3(3, 4.5f, -1);
            piece.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            piece.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);

            //Knight
            piece = new GameObject("White Knight", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(PromotionSelection));
            piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[5];
            piece.transform.position = new Vector3(3, 4.5f, -1);
            piece.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            piece.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
        }
        else
        {
            //Queen
            piece = new GameObject("Black Queen", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(PromotionSelection));
            piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[8];
            piece.transform.position = new Vector3(3, 4.5f, -1);
            piece.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            piece.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);

            //Rook
            piece = new GameObject("Black Rook", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(PromotionSelection));
            piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[10];
            piece.transform.position = new Vector3(3, 4.5f, -1);
            piece.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            piece.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);

            //Bishop
            piece = new GameObject("Black Bishop", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(PromotionSelection));
            piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[0];
            piece.transform.position = new Vector3(3, 4.5f, -1);
            piece.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            piece.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);

            //Knight
            piece = new GameObject("Black Knight", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(PromotionSelection));
            piece.GetComponent<SpriteRenderer>().sprite = ChessGame.sprites[4];
            piece.transform.position = new Vector3(3, 4.5f, -1);
            piece.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            piece.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
        }
    }
    private void OnDisable()
    {
        Destroy(piece);
        piece = null;
    }
}

public class PromotionSelection : MonoBehaviour
{
    private void OnMouseUp()
    {
        if(name.Contains("Queen"))
        {
            gameObject.GetComponent<PromotionMenu>().choice = 1;
        }
        else if (name.Contains("Rook"))
        {
            gameObject.GetComponent<PromotionMenu>().choice = 2;
        }
        else if (name.Contains("Bishop"))
        {
            gameObject.GetComponent<PromotionMenu>().choice = 3;
        }
        else if (name.Contains("Knight"))
        {
            gameObject.GetComponent<PromotionMenu>().choice = 4;
        }
    }
}
