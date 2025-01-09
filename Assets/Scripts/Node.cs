using NUnit.Framework;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2 localPosition;
    public Vector2Int?[] NeighborNodes;
    public Vector2Int point;

    public void setup(Vector2Int?[] neighborNodes, Vector2Int point)
    {
        NeighborNodes = neighborNodes;
        this.point = point;

    }
}
