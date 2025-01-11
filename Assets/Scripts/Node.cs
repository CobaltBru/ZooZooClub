using NUnit.Framework;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2 localPosition; //노드의 실제 좌표
    public Vector2Int?[] NeighborNodes; //이웃노드의 인덱스
    public Vector2Int point; //현재 노드의 인덱스

    public Block placedBlock; //현재 노드에 위치한 블럭의 정보

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
