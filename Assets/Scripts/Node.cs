using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 localPosition; //노드의 실제 좌표
    public Vector2Int?[] NeighborNodes; //이웃노드의 인덱스
    public Vector2Int point; //현재 노드의 인덱스

    public Block placedBlock; //현재 노드에 위치한 블럭의 정보

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
