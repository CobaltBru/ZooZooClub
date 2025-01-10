using NUnit.Framework;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2 localPosition;
    public Vector2Int?[] NeighborNodes;
    public Vector2Int point;

    public Block placedBlock;
    [SerializeField]
    private Board board;

    public void Setup(Vector2Int?[] neighborNodes, Vector2Int point)
    {
        NeighborNodes = neighborNodes;
        this.point = point;

    }

    public Node FindTarget()
    {
        if (NeighborNodes[1].HasValue == true)
        {
            Vector2Int np = NeighborNodes[1].Value;
            Node neighborNode = board.NodeList[np.y * board.panelSize.x + point.x];
            if(neighborNode.placedBlock == null)
            {
                return neighborNode.FindTarget();
            }
            else
            {
                return this;
            }
        }
        else
        {
            return this;
        }
    }
}
