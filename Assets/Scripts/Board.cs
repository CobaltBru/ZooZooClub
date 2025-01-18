using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    NodeSpawner nodeSpawner;
    public List<Node> NodeList;
    public Queue<Node> updateNodeQueue;

    [SerializeField]
    public Vector2Int panelSize;
    [SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private Transform blockRect;

    private List<Block> blockList;

    [SerializeField]
    public bool moving = false;
    public Vector2Int? dragStartNode;
    public Vector2Int? dragEndNode;

    public Vector2Int currentMoveBlock1;
    public Vector2Int currentMoveBlock2;

    private void Awake()
    {
        panelSize = new Vector2Int(8, 16);
        NodeList = nodeSpawner.SpawnNode(this, panelSize);
        blockList = new List<Block>();
        dragStartNode = null;
        dragEndNode = null;
    }
    private void Start()
    {
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(nodeSpawner.GetComponent<RectTransform>());

        foreach (Node node in NodeList)
        {
            node.localPosition = node.GetComponent<RectTransform>().localPosition;
        }
        CheckNodesBlank();
    }
    private void Update()
    {
        
        BlockPrecess();
        CheckNodesBlank();
        if(!moving) SwapBlock();
    }
    void CheckNodesBlank()
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
        
        //for(int x = 0;x<panelSize.x;x++)
        //{
        //    Debug.Log($"{x},{needSpawnNode[x]}");
        //}

    }


    void SpawnBlock(int xy)
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

    public void BlockPrecess()
    {
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
                    moving = true;
                    //updateNodeQueue.Enqueue(targetNode);
                    Move(node, targetNode); //위치정보의 이동
                }
            }
        }
        
        foreach (Block block in blockList)
        {
            if(block.target != null)
            {
                block.StartMove(); //실질적으로 보여지는 이동
            }
        }
        moving = false;
    }

    void blockDestroyProcess()
    {
        while(updateNodeQueue.Count != 0)
        {
            Node node = updateNodeQueue.Dequeue();

        }
    }

    void blockDestroyCheck()
    {
        bool[] rowchecker = new bool[panelSize.x * panelSize.y];
        bool[] colchecker = new bool[panelSize.x * panelSize.y];
        for (int i = panelSize.y / 2; i < panelSize.y; i++)
        {
            for (int j = 0; j < panelSize.x; j++)
            {
                Node currentNode = NodeList[i * panelSize.x + j];
                if (rowchecker[i * panelSize.x + j] == false)
                {
                    rowchecker[i * panelSize.x + j] = true;
                    currentNode.rowSameCount = currentNode.FindSame(ref rowchecker, currentNode.placedBlock.blockType, 0, 1,0);
                }
                if (colchecker[i * panelSize.x + j] == false)
                {
                    colchecker[i * panelSize.x + j] = true;
                    currentNode.colSameCount = currentNode.FindSame(ref colchecker, currentNode.placedBlock.blockType, 1, 1,0);
                }
                Debug.Log($"{j},{i}->({currentNode.rowSameCount},{currentNode.colSameCount})");
            }
        }

    }

    //private void DestroyBlocks()
    //{
    //    Queue<Node> queue = new Queue<Node>();
    //    Vector2Int? itemPos = null;
    //    for (int i = panelSize.y / 2; i < panelSize.y; i++)
    //    {
    //        for (int j = 0; j < panelSize.x; j++)
    //        {
    //            Node currentNode = NodeList[i * panelSize.x + j];
    //            queue.Enqueue(currentNode);
    //            if(currentNode.rowSameCount >=3)
    //            {
    //                for (int x = j + 1; x < j + currentNode.rowSameCount; x++) 
    //                {

    //                }
    //            }
    //            if (currentNode.colSameCount >= 3)
    //            {
    //                for (int y = i + 1; y < i + currentNode.colSameCount; y++) 
    //                {

    //                }
    //            }

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

    private void SwapBlock()
    {
        //드래그가 짧거나, 드래그한 블럭이 없거나, 범위 밖이면 return
        if (dragStartNode == dragEndNode || dragEndNode == null || dragStartNode == null) return;
        if (dragStartNode.Value.y < panelSize.y / 2 || dragEndNode.Value.y < panelSize.y / 2) return;
        //움직이는중 마킹
        moving = true;
        //시작, 끝 블럭 확인
        Node from = NodeList[dragStartNode.Value.y * panelSize.x + dragStartNode.Value.x];
        Node to = NodeList[dragEndNode.Value.y * panelSize.x + dragEndNode.Value.x];
        //현재 스왑한 블럭 저장
        currentMoveBlock1 = dragStartNode.Value;
        currentMoveBlock2 = dragEndNode.Value;

        //스왑
        Swap(from, to);
        //Swap 후 제거할 블럭이 생겼는지 체크
        blockDestroyCheck();

        //초기화
        dragEndNode = null;
        dragStartNode = null;
        moving = false;
        
    }
}
