using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 localPosition; //����� ���� ��ǥ
    public Vector2Int?[] NeighborNodes; //�̿������ �ε���
    public Vector2Int point; //���� ����� �ε���

    public Block placedBlock; //���� ��忡 ��ġ�� ���� ����

    private Board board;

    public bool clickAble = false;

    private float dragDistance = 25;
    private Vector3 touchStart;
    private Vector3 touchEnd;
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

    public int FindSame(bool[] checker,int type, int way)
    {
        if(NeighborNodes[way].HasValue == true)
        {
            Vector2Int np = NeighborNodes[way].Value;
            Node node = board.NodeList[np.y * board.panelSize.x + np.x];
            if (placedBlock.blockType == type)
            {
                checker[point.y * board.panelSize.x + point.x] = true;
                return 1 + node.FindSame(checker, type, way);
            }
            else return 0;
        }
        return 0;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        
        board.dragStartNode = point;
        touchStart = Input.mousePosition;
        //Debug.Log($"{touchStart}");
    }

    
    public void OnPointerUp(PointerEventData eventData)
    {
        touchEnd = Input.mousePosition;
        //Debug.Log($"{touchEnd}");
        float deltaX = touchEnd.x - touchStart.x;
        float deltaY = touchEnd.y - touchStart.y;

        if(Mathf.Abs(deltaX) < dragDistance && Mathf.Abs(deltaY) < dragDistance)
        {
            board.dragEndNode = point;
            return;
        }

        if(Mathf.Abs(deltaX)>Mathf.Abs(deltaY))
        {
            if (Mathf.Sign(deltaX) >= 0) board.dragEndNode = NeighborNodes[0] ?? point;
            else board.dragEndNode = NeighborNodes[2] ?? point;
        }
        else
        {
            if (Mathf.Sign(deltaY) >= 0) board.dragEndNode = NeighborNodes[3] ?? point;
            else board.dragEndNode = NeighborNodes[1] ?? point;
        }
    }
}
