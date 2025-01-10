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
        panelSize = new Vector2Int(8, 9);
        NodeList = nodeSpawner.SpawnNode(panelSize);
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

    void CheckNodesBlank()
    {
        for(int x = 0;x<panelSize.x;x++)
        {
            if (NodeList[x].placedBlock == null)
            {
                SpawnBlock(x);
            }
        }
        //for(int x = 0;x<panelSize.x;x++)
        //{
        //    Debug.Log($"{x},{needSpawnNode[x]}");
        //}

    }


    void SpawnBlock(int line)
    {
        Vector2Int np = NodeList[line].point;
        GameObject clone = Instantiate(blockPrefab, blockRect);
        Block block = clone.GetComponent<Block>();
        Node node = NodeList[line];

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
                if (node.placedBlock == null) continue;
                Node targetNode = node.FindTarget();
                if (targetNode == node) continue;
                else
                {

                }
            }
        }
    }
    public void Move(Node from, Node to)
    {
        
    }
}
