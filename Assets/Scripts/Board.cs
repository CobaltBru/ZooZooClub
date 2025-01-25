using System;
using System.Collections;
using System.Collections.Generic;
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

    public List<Vector2Int?> updateNodeList;
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
        updateNodeList = new List<Vector2Int?>();
    }
    private void Start()
    {
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(nodeSpawner.GetComponent<RectTransform>());

        foreach (Node node in NodeList)
        {
            node.localPosition = node.GetComponent<RectTransform>().localPosition;
        }
        CheckNodesBlank();
        BlockDrop();
    }
    private void Update()
    {
        //Debug.Log(currentState);
        switch(currentState)
        {
            case STATUS.IDLE:
                IdleProcess();
                break;
            case STATUS.DROP:
                BlockDrop();
                break;
            case STATUS.SWAP:
                StartCoroutine(checkSwapStatus());
                break;
            case STATUS.DESTROY:
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

    void CheckNodesBlank() //����ĭ ��ĭüũ �� ��� ����
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


    void SpawnBlock(int xy) // ��� ����
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

    public void BlockDrop()
    {
        updateNodeList.Clear();
        for(int y = panelSize.y-1;y>=0;y--)
        {
            for(int x = 0;x<panelSize.x;x++)
            {
                Node node = NodeList[y *  panelSize.x + x];
                if (node.placedBlock == null)
                {
                    
                    continue; //���� Node�� block�� ���ٸ� �н�
                }
                Node targetNode = node.FindTarget(); //���� ����� ������ Ž��
                if (targetNode == node)
                {
                    continue; // ���� ���� �������� ������ �н�
                }
                else // �̵�
                {
                    if(y >= panelSize.y / 2) updateNodeList.Add(node.point);
                    Move(node, targetNode); //��ġ������ �̵�
                }
            }
        }
        
        foreach (Block block in blockList)
        {
            if(block.target != null)
            {
                block.StartMove(); //���������� �������� �̵�
            }
        }
    }

    void blockDestroyProcess()
    {
        updateNodeList.Sort((a, b) =>
        {
            if (!a.HasValue && !b.HasValue) return 0;   // �� �� null
            if (!a.HasValue) return 1;                 // a�� null�̸� b�� ������
            if (!b.HasValue) return -1;                // b�� null�̸� a�� ������

            // ��������: y�� �켱, ������ x�� ��
            int compareY = b.Value.y.CompareTo(a.Value.y);
            return compareY != 0 ? compareY : b.Value.x.CompareTo(a.Value.x);
        });

        for (int i = 0; i < updateNodeList.Count; i++) //5��, TL ������ ���� ��������
        {
            Node node = NodeList[updateNodeList[i].Value.y * panelSize.x + updateNodeList[i].Value.x];
            
            if (node.placedBlock == null) continue;
            bool[] tmp = new bool[0];
            if (node.rowSameCount >= 5)
            {
                Debug.Log("5 -");
                node.FindSame(ref tmp, node.placedBlock.blockType, 0, 0, true);
                node.FindSame(ref tmp, node.placedBlock.blockType, 2, 0, true);
                node.placedBlock.ChangeImage((int)Items.five);
                Debug.Log(node.point);
            }
            else if(node.colSameCount >= 5)
            {
                Debug.Log("5 l");
                node.FindSame(ref tmp, node.placedBlock.blockType, 1, 0, true);
                node.FindSame(ref tmp, node.placedBlock.blockType, 3, 0, true);
                node.placedBlock.ChangeImage((int)Items.five);
                Debug.Log(node.point);
            }
            else if (node.rowSameCount >= 3 && node.colSameCount >= 3)
            {

            }

            
        }
        for (int i = 0; i < updateNodeList.Count; i++)
        {
            Node node = NodeList[updateNodeList[i].Value.y * panelSize.x + updateNodeList[i].Value.x];
            if (node.placedBlock == null) continue;

            if (node.rowSameCount >= 4)
            {

            }
            else if (node.colSameCount >= 4)
            {

            }
            else if (node.colSameCount >= 3)
            {

            }
            else if (node.colSameCount >= 3)
            {

            }
        }
    }

    void blockConnectionCheck()
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
                    //currentNode.rowSameCount = currentNode.FindSame(ref rowchecker, currentNode.placedBlock.blockType, 0, 1,false);
                    currentNode.rowSameCount = currentNode.FindSame2(ref rowchecker, currentNode.placedBlock.blockType, 0, 1);
                }
                if (colchecker[i * panelSize.x + j] == false)
                {
                    colchecker[i * panelSize.x + j] = true;
                    //currentNode.colSameCount = currentNode.FindSame(ref colchecker, currentNode.placedBlock.blockType, 1, 1,false);
                    currentNode.colSameCount = currentNode.FindSame2(ref colchecker, currentNode.placedBlock.blockType, 1, 1);
                }
                //Debug.Log($"{j},{i}->({currentNode.rowSameCount},{currentNode.colSameCount})");
            }
        }

    }



    
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
        //blockConnectionCheck();
        from.FindSame3();
        to.FindSame3();

        //if (from.rowSameCount >= 3 || from.colSameCount >= 3 || to.rowSameCount >= 3 || to.colSameCount >= 3)
        //{
        //    currentState = STATUS.DESTROY;
        //}
        if (from.sameCount[0] + from.sameCount[2]>=2 || from.sameCount[1] + from.sameCount[3] >= 2 ||
            to.sameCount[0] + to.sameCount[2] >= 2|| to.sameCount[1] + to.sameCount[3] >= 2)
        {
            currentState = STATUS.DESTROY;
        }
        else //���󺹱�
        {
            Swap(from, to);
            yield return new WaitForSeconds(2.0f);
            currentState = STATUS.IDLE;
        }
        //�ʱ�ȭ
        dragEndNode = null;
        dragStartNode = null;
    }
    private IEnumerator SwapBlock()
    {
        currentState = STATUS.PROCESS;
        updateNodeList.Clear();
        //����, �� �� Ȯ��
        Node from = NodeList[dragStartNode.Value.y * panelSize.x + dragStartNode.Value.x];
        Node to = NodeList[dragEndNode.Value.y * panelSize.x + dragEndNode.Value.x];
        //���� ������ �� ����
        updateNodeList.Add(dragStartNode.Value);
        updateNodeList.Add(dragEndNode.Value);

        //����
        Swap(from, to);
        yield return new WaitForSeconds(2.0f);
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
