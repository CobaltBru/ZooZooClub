using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerClickHandler
{
    public Vector2 localPosition; //����� ���� ��ǥ
    public Vector2Int?[] NeighborNodes; //�̿������ �ε���
    public Vector2Int point; //���� ����� �ε���

    public Block placedBlock; //���� ��忡 ��ġ�� ���� ����

    private Board board;

    public bool clickAble = false;

    public void Setup(Board board, Vector2Int?[] neighborNodes, Vector2Int point)
    {
        this.board = board;
        NeighborNodes = neighborNodes;
        this.point = point;

    }
    

    public Node FindTarget()
    {
        if (NeighborNodes[1].HasValue == true) // ��尡 ����
        {
            
            Vector2Int np = NeighborNodes[1].Value; // �Ʒ� ��� Ȯ��
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if(point.y >= board.panelSize.y/2)
        {
            Debug.Log("in");
        }
        else
        {
            Debug.Log("out");
        }
    }
}
