using NUnit.Framework;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2 localPosition;
    public Vector2Int?[] NeighborNodes;
    public Vector2Int point;

    public Block placedBlock;

    private Board board;

    public void Setup(Board board, Vector2Int?[] neighborNodes, Vector2Int point)
    {
        this.board = board;
        NeighborNodes = neighborNodes;
        this.point = point;

    }

    public Node FindTarget()
    {
        if (NeighborNodes[1].HasValue == true) // 노드가 존재
        {
            
            Vector2Int np = NeighborNodes[1].Value; // 아래 노드 확인
            Node neighborNode = board.NodeList[np.y * board.panelSize.x + np.x];
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
