using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;

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

    public int rowSameCount = 0;
    public int colSameCount = 0;
    public int[] sameCount;
    public void Setup(Board board, Vector2Int?[] neighborNodes, Vector2Int point)
    {
        this.board = board;
        NeighborNodes = neighborNodes;
        this.point = point;
        sameCount = new int[4] { 0, 0, 0, 0 };
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

    //public int FindSame(ref bool[] checker,IMAGES type, int way, int counter, bool destroyFlag)
    //{
    //    if ((way == 0 || way == 2))
    //    {
    //        if (destroyFlag) rowSameCount = 1;
    //        else rowSameCount = counter;
    //    }
    //    else if ((way == 1 || way == 3))
    //    {
    //        if (destroyFlag) colSameCount = 1;
    //        else colSameCount = counter;
    //    }

    //    if(NeighborNodes[way].HasValue == true)
    //    {
    //        Vector2Int np = NeighborNodes[way].Value;
    //        Node node = board.NodeList[np.y * board.panelSize.x + np.x];
    //        if (node.placedBlock.blockType == type)
    //        {
    //            if(destroyFlag == false)checker[node.point.y * board.panelSize.x + node.point.x] = true;
    //            if (destroyFlag == true && placedBlock != null) DestroyBlockObject();
    //            if (way == 0 || way == 2) return destroyFlag ? node.FindSame(ref checker, type, way, counter + 1, destroyFlag) : rowSameCount = node.FindSame(ref checker, type, way, counter + 1, destroyFlag);
    //            if (way == 1 || way == 3) return destroyFlag ? node.FindSame(ref checker, type, way, counter + 1, destroyFlag) : colSameCount = node.FindSame(ref checker, type, way, counter + 1, destroyFlag);
    //        }
    //        else return counter;
    //    }
    //    return counter;
    //}

    //public int FindSame2(ref bool[] checker, int type, int way, int counter)
    //{
    //    checker[point.y * board.panelSize.x + point.x] = true;
    //    if (NeighborNodes[way].HasValue)
    //    {
    //        Vector2Int nextNodePoint = NeighborNodes[way].Value;
    //        Node nextNode = board.NodeList[nextNodePoint.y * board.panelSize.x + nextNodePoint.x];

    //        if (nextNode.placedBlock.blockType == type && !checker[nextNode.point.y * board.panelSize.x + nextNode.point.x])
    //        {
    //            counter = nextNode.FindSame2(ref checker, type, way, counter + 1);
    //        }

    //    }
    //    if (way == 0) rowSameCount = counter; // 가로
    //    else if (way == 1) colSameCount = counter; // 세로

    //    return counter;
    //}

    public void FindSame3() //true -> destroy
    {
        if(placedBlock == null) return;
        InitsameCount();
        IMAGES type = placedBlock.blockType;

        for (int i = 0; i < 4; i++)
        {
            Node node = this;
            while (true)
            {
                if (!node.NeighborNodes[i].HasValue) break;
                Vector2Int nextNodePoint = node.NeighborNodes[i].Value;
                if (nextNodePoint.y < board.panelSize.y / 2) break;
                node = board.NodeList[nextNodePoint.y * board.panelSize.x + nextNodePoint.x];
                if (node.placedBlock == null) break;
                if (node.placedBlock.blockType != type) break;
                else
                {
                    sameCount[i]++;
                }
            }
        }
    }

    public void Destroysame()
    {
        if (placedBlock == null) return;
        IMAGES type = placedBlock.blockType;
        //Debug.Log($"[{point.x},{point.y}]");
        if (sameCount[0] + sameCount[2] >= 2) 
        {
            Node node = this;
            for (int i = 0; i < sameCount[0];i++)
            {
                Vector2Int nextNodePoint = node.NeighborNodes[0].Value;
                node = board.NodeList[nextNodePoint.y * board.panelSize.x + nextNodePoint.x];
                if (node.placedBlock == null) continue;
                node.DestroyBlockObject();
                node.InitsameCount();
                //Debug.Log($"[{point.x},{point.y}]//[{nextNodePoint.x},{nextNodePoint.y}]");
            }
            node = this;
            for (int i = 0; i < sameCount[2]; i++)
            {
                Vector2Int nextNodePoint = node.NeighborNodes[2].Value;
                node = board.NodeList[nextNodePoint.y * board.panelSize.x + nextNodePoint.x];
                if (node.placedBlock == null) continue;
                node.DestroyBlockObject();
                node.InitsameCount();
                //Debug.Log($"[{point.x},{point.y}]//[{nextNodePoint.x},{nextNodePoint.y}]");
            }
        }
        if (sameCount[1] + sameCount[3] >= 2)
        {
            Node node = this;
            for (int i = 0; i < sameCount[1]; i++)
            {
                Vector2Int nextNodePoint = node.NeighborNodes[1].Value;
                node = board.NodeList[nextNodePoint.y * board.panelSize.x + nextNodePoint.x];
                if (node.placedBlock == null) continue;
                node.DestroyBlockObject();
                node.InitsameCount();
                //Debug.Log($"[{point.x},{point.y}]//[{nextNodePoint.x},{nextNodePoint.y}]");
            }
            node = this;
            for (int i = 0; i < sameCount[3]; i++)
            {
                Vector2Int nextNodePoint = node.NeighborNodes[3].Value;
                node = board.NodeList[nextNodePoint.y * board.panelSize.x + nextNodePoint.x];
                if (node.placedBlock == null) continue;
                node.DestroyBlockObject();
                node.InitsameCount();
                //Debug.Log($"[{point.x},{point.y}]//[{nextNodePoint.x},{nextNodePoint.y}]");
            }
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (board.currentState != STATUS.IDLE) return;
        board.dragStartNode = point;
        touchStart = Input.mousePosition;
    }

    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (board.currentState != STATUS.IDLE) return;
        touchEnd = Input.mousePosition;
        float deltaX = touchEnd.x - touchStart.x;
        float deltaY = touchEnd.y - touchStart.y;

        if(Mathf.Abs(deltaX) < dragDistance && Mathf.Abs(deltaY) < dragDistance) //무효
        {
            board.dragEndNode = point;
            return;
        }

        if(Mathf.Abs(deltaX)>Mathf.Abs(deltaY)) //수평이동판정
        {
            if (Mathf.Sign(deltaX) >= 0) board.dragEndNode = NeighborNodes[0] ?? point;
            else board.dragEndNode = NeighborNodes[2] ?? point;
        }
        else //수직이동판정
        {
            if (Mathf.Sign(deltaY) >= 0) board.dragEndNode = NeighborNodes[3] ?? point;
            else board.dragEndNode = NeighborNodes[1] ?? point;
        }
    }
    public void DestroyBlockObject()
    {
        if (placedBlock == null) return;
        placedBlock.DestroyBlock();
        placedBlock = null;
    }

    public void InitsameCount()
    {
        for (int i = 0; i < 4; i++) sameCount[i] = 0;
    }

    public void ToItem(IMAGES type)
    {
        placedBlock.ChangeImage(type);
    }
}
