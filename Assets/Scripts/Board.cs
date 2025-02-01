using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum STATUS {IDLE, PROCESS, DROP, SWAP, DESTROY };
public class Board : MonoBehaviour
{
    [SerializeField]
    NodeSpawner nodeSpawner;
    public List<Node> NodeList;
    
    public STATUS currentState = STATUS.IDLE;

    [SerializeField]
    public Vector2Int panelSize;
    [SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private Transform blockRect;

    private List<Block> blockList;

    public List<Node> updateNodeList;
    [SerializeField]
    public Vector2Int? dragStartNode;
    [SerializeField]
    public Vector2Int? dragEndNode;

    private void Awake()
    {
        panelSize = new Vector2Int(8, 16);
        NodeList = nodeSpawner.SpawnNode(this, panelSize);
        blockList = new List<Block>();
        dragStartNode = null;
        dragEndNode = null;
        updateNodeList = new List<Node>();
    }
    private void Start()
    {
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(nodeSpawner.GetComponent<RectTransform>());

        foreach (Node node in NodeList)
        {
            node.localPosition = node.GetComponent<RectTransform>().localPosition;
        }
        CheckNodesBlank();
        StartCoroutine(BlockDrop());
    }
    private void Update()
    {
        //Debug.Log(updateNodeList.Count());
        switch(currentState)
        {
            case STATUS.IDLE:
                IdleProcess();
                break;
            case STATUS.DROP:
                StartCoroutine(BlockDrop());
                break;
            case STATUS.SWAP:
                StartCoroutine(checkSwapStatus());
                break;
            case STATUS.DESTROY:
                StartCoroutine(blockDestroyProcess());
                break;
            case STATUS.PROCESS:
                break;
        }
    }

    void IdleProcess()
    {
        if (dragStartNode != dragEndNode && dragEndNode != null && dragStartNode != null)
        {
            if (dragStartNode.Value.y >= panelSize.y / 2 && dragEndNode.Value.y >= panelSize.y / 2)
            {
                StartCoroutine(SwapBlock());
            }
        }
    }

    void CheckNodesBlank() //생성칸 빈칸체크 후 블록 스폰
    {
        for (int y = 0; y < panelSize.y / 2; y++) 
        {
            for (int x = 0; x < panelSize.x; x++)
            {
                if (NodeList[y * panelSize.x + x].placedBlock == null)
                {
                    SpawnBlock(y * panelSize.x + x);
                }
            }
        }
       
    }


    void SpawnBlock(int xy) // 블록 스폰
    {
        Vector2Int np = NodeList[xy].point;
        GameObject clone = Instantiate(blockPrefab, blockRect);
        Block block = clone.GetComponent<Block>();
        Node node = NodeList[xy];

        clone.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 180);
        clone.GetComponent<RectTransform>().localPosition = node.localPosition;

        block.Setup();

        node.placedBlock = block;

        blockList.Add(block);
    }

    public IEnumerator BlockDrop()
    {
        updateNodeList.Clear();
        currentState = STATUS.PROCESS;

        for(int y = panelSize.y-1;y>=0;y--)
        {
            for(int x = 0;x<panelSize.x;x++)
            {
                Node node = NodeList[y *  panelSize.x + x];
                if (node.placedBlock == null)
                {
                    
                    continue; //현재 Node에 block이 없다면 패스
                }
                Node targetNode = node.FindTarget(); //현재 노드의 목적지 탐색
                if (targetNode == node)
                {
                    continue; // 현재 노드와 목적지가 같으면 패스
                }
                else // 이동
                {
                    Move(node, targetNode); //위치정보의 이동
                    updateNodeList.Add(targetNode);
                }
            }
        }
        
        foreach (Node node in updateNodeList)
        {
            if(node.placedBlock.target != null)
            {
                node.placedBlock.StartMove(); //실질적으로 보여지는 이동
            }
        }
        CheckNodesBlank();

        yield return new WaitForSeconds(0.5f);

        if(updateNodeList.Count > 0) currentState = STATUS.DESTROY;
        else currentState = STATUS.IDLE;
    }

    IEnumerator blockDestroyProcess()
    {
        currentState = STATUS.PROCESS;
        updateNodeList.Reverse();
        Debug.Log(updateNodeList.Count);
        foreach(Node node in updateNodeList)
        {
            if ((node.point.y < panelSize.y / 2) || node.placedBlock == null) continue;
            node.FindSame3();
            int r = node.sameCount[0];
            int d = node.sameCount[1];
            int l = node.sameCount[2];
            int u = node.sameCount[3];
            if (r + l >= 4) //가로 5
            {
                node.Destroysame();
                node.ToItem(IMAGES.five);
            }
            else if (d + u >= 4) //세로 5
            {
                node.Destroysame();
                node.ToItem(IMAGES.five);
            }
            else if (r + l >= 2 && u + d >= 2) //T
            {
                node.Destroysame();
                node.ToItem(IMAGES.bomb);
            }
            node.InitsameCount();
        }
        foreach (Node node in updateNodeList)
        {
            if (node.placedBlock == null) continue;
            node.FindSame3();
            int r = node.sameCount[0];
            int d = node.sameCount[1];
            int l = node.sameCount[2];
            int u = node.sameCount[3];
            if (r + l >= 3) //가로 4
            {
                node.Destroysame();
                node.ToItem(IMAGES.four);
            }
            else if (d + u >= 3) //세로 4
            {
                node.Destroysame();
                node.ToItem(IMAGES.four);
            }
            else if (r + l >= 2) // 가로 3
            {
                node.Destroysame();
                node.DestroyBlockObject();
            }
            else if(u + d >= 2) //세로 3
            {
                node.Destroysame();
                node.DestroyBlockObject();
            }
            node.InitsameCount();
        }
        updateNodeList.Clear();
        yield return new WaitForSeconds(1.0f);
        currentState = STATUS.DROP;

    }

    //void blockConnectionCheck()
    //{
    //    bool[] rowchecker = new bool[panelSize.x * panelSize.y];
    //    bool[] colchecker = new bool[panelSize.x * panelSize.y];
    //    for (int i = panelSize.y / 2; i < panelSize.y; i++)
    //    {
    //        for (int j = 0; j < panelSize.x; j++)
    //        {
    //            Node currentNode = NodeList[i * panelSize.x + j];
    //            if (rowchecker[i * panelSize.x + j] == false)
    //            {
    //                rowchecker[i * panelSize.x + j] = true;
    //                //currentNode.rowSameCount = currentNode.FindSame(ref rowchecker, currentNode.placedBlock.blockType, 0, 1,false);
    //                currentNode.rowSameCount = currentNode.FindSame2(ref rowchecker, currentNode.placedBlock.blockType, 0, 1);
    //            }
    //            if (colchecker[i * panelSize.x + j] == false)
    //            {
    //                colchecker[i * panelSize.x + j] = true;
    //                //currentNode.colSameCount = currentNode.FindSame(ref colchecker, currentNode.placedBlock.blockType, 1, 1,false);
    //                currentNode.colSameCount = currentNode.FindSame2(ref colchecker, currentNode.placedBlock.blockType, 1, 1);
    //            }
    //            //Debug.Log($"{j},{i}->({currentNode.rowSameCount},{currentNode.colSameCount})");
    //        }
    //    }

    //}



    
    public void Move(Node from, Node to)
    {
        from.placedBlock.MoveToNode(to);
        if(from.placedBlock != null)
        {
            to.placedBlock = from.placedBlock;
            from.placedBlock = null;
        }
    }

    public void Swap(Node from, Node to)
    {
        from.placedBlock.MoveToNode(to);
        to.placedBlock.MoveToNode(from);
        Block tmp = from.placedBlock;
        from.placedBlock = to.placedBlock;
        to.placedBlock = tmp;
        from.placedBlock.StartMove();
        to.placedBlock.StartMove();
    }
    private IEnumerator checkSwapStatus()
    {
        currentState = STATUS.PROCESS;
        Node from = NodeList[dragStartNode.Value.y * panelSize.x + dragStartNode.Value.x];
        Node to = NodeList[dragEndNode.Value.y * panelSize.x + dragEndNode.Value.x];
        from.FindSame3();
        to.FindSame3();

        if (from.sameCount[0] + from.sameCount[2]>=2 || from.sameCount[1] + from.sameCount[3] >= 2 ||
            to.sameCount[0] + to.sameCount[2] >= 2|| to.sameCount[1] + to.sameCount[3] >= 2)
        {
            updateNodeList.Add(to);
            updateNodeList.Add(from);
            currentState = STATUS.DESTROY;
        }
        else //원상복귀
        {
            Swap(from, to);
            from.InitsameCount();
            to.InitsameCount();
            yield return new WaitForSeconds(1.0f);
            currentState = STATUS.IDLE;
        }
        //초기화
        dragEndNode = null;
        dragStartNode = null;
    }
    private IEnumerator SwapBlock()
    {
        currentState = STATUS.PROCESS;
        updateNodeList.Clear();
        //시작, 끝 블럭 확인
        Node from = NodeList[dragStartNode.Value.y * panelSize.x + dragStartNode.Value.x];
        Node to = NodeList[dragEndNode.Value.y * panelSize.x + dragEndNode.Value.x];
        //스왑
        Swap(from, to);
        yield return new WaitForSeconds(1.0f);
        currentState = STATUS.SWAP;
        
    }

    public bool CheckAllBlockMoveFinish()
    {
        foreach(Node node in NodeList)
        {
            if(node.placedBlock != null)
            {
                if (node.placedBlock.isMoving == true) return false;
            }
        }
        return true;
    }
}
