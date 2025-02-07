using UnityEngine;

public class Items : MonoBehaviour
{
    [SerializeField]
    Board board;

    void IsItemMix(Node node, IMAGES img1, IMAGES img2)
    {
        if (img1 == IMAGES.four && img2 == IMAGES.four) Mix_RawColItem(node);
        else if ((((img1 == IMAGES.four) || (img1 == IMAGES.four)) && (img2 == IMAGES.bomb)) ||
            (((img2 == IMAGES.four) || (img2 == IMAGES.four)) && (img1 == IMAGES.bomb))) Mix_RowColBombItem(node);

    }

    void RawItem(Node point)
    {

    }

    void ColItem(Node point)
    {

    }

    void BombItem(Node point)
    {

    }

    void StarItem(Node Point)
    {

    }

    void Mix_RawColItem(Node point)
    {

    }

    void Mix_RowColBombItem(Node point)
    {

    }


}
