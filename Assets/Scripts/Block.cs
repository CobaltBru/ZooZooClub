using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public enum Fruits { Apple = 0, Banana, Grape, Kiwi, Orange, Pineapple };

public class Block : MonoBehaviour
{
    [SerializeField]
    private Sprite[] fruitsImages;
    [SerializeField]
    int blockType;

    private Image image;

    public Node target;

    public void Setup()
    {
        image = GetComponent<Image>();
        blockType = Random.Range(0, 6);
        image.sprite = fruitsImages[blockType];
    }
    public void MoveToNode(Node to)
    {
        target = to;
    }
    public void StartMove()
    {

    }

}
