using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    NodeSpawner nodeSpawner;
    public List<Node> NodeList;
    [SerializeField]
    public Vector2Int panelSize;
    [SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private Transform blockRect;

    private List<Block> blockList;


    private void Awake()
    {
        panelSize = new Vector2Int(8, 16);
        NodeList = nodeSpawner.SpawnNode(this, panelSize);
        blockList = new List<Block>();
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
        for(int y = 0;y<panelSize.y;y++)
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
                }
            }
        }

        foreach(Block block in blockList)
        {
            if(block.target != null)
            {
                block.StartMove(); //실질적으로 보여지는 이동
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
}
