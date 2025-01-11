using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class NodeSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject nodePrefab;
    [SerializeField]
    private RectTransform nodeRect;

    public List<Node> SpawnNode(Board board, Vector2Int panelSize)
    {
        List<Node> nodeList = new List<Node>(panelSize.x * panelSize.y);

        for (int y = 0; y < panelSize.y; y++) 
        {
            for (int x = 0; x < panelSize.x; x++) 
            {
                GameObject clone = Instantiate(nodePrefab, nodeRect.transform);
                Vector2Int point = new Vector2Int(x,y); // 생성된 Node의 좌표

                Vector2Int?[] neighborNodes = new Vector2Int?[4]; //생성된 Node의 주변좌표

                Vector2Int right = point + Vector2Int.right;
                Vector2Int down = point + Vector2Int.up; //좌표가 좌상단부터 시작하기 때문에
                Vector2Int left = point + Vector2Int.left;
                Vector2Int up = point + Vector2Int.down;

                if (IsValid(right, panelSize)) neighborNodes[0] = right;
                if (IsValid(down, panelSize)) neighborNodes[1] = down;
                if (IsValid(left, panelSize)) neighborNodes[2] = left;
                if (IsValid(up, panelSize)) neighborNodes[3] = up;

                Node node = clone.GetComponent<Node>();
                node.Setup(board, neighborNodes, point);

                clone.name = $"[{node.point.y}, {node.point.x}]";

                nodeList.Add(node);
            }
        }
        return nodeList;
        
    }
    bool IsValid(Vector2Int point, Vector2Int panelSize)
    {
        if (point.x < 0 || point.x >= panelSize.x || point.y < 0 || point.y >= panelSize.y)
            return false;
        else
        {
            return true;
        }
    }
}
