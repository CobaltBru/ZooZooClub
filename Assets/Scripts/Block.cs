using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public enum Fruits { Apple = 0, Banana, Grape, Kiwi, Orange, Pineapple };
public enum Items { five = 0, four, bomb};

public class Block : MonoBehaviour
{
    [SerializeField]
    private Sprite[] fruitsImages;
    [SerializeField]
    private Sprite[] itemImages;
    [SerializeField]
    public int blockType;

    private Image image;

    public Node target;

    public bool needDestroy = false;
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
        float moveTime = 1f;
        StartCoroutine(DropDownAnimation(target.localPosition, moveTime));
        target = null;
    }

    private IEnumerator DropDownAnimation(Vector3 end, float time)
    {
        float current = 0;
        float percent = 0;
        Vector3 start = GetComponent<RectTransform>().localPosition;

        while (percent < 1) 
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.localPosition  = Vector3.Lerp(start,end, percent);

            yield return null;
        }
    }
    public void DestroyBlock()
    {
        gameObject.SetActive(false);
        Destroy(this);
    }
    public void ChangeImage(int type)
    {
        blockType = type;
        image.sprite = itemImages[type];
    }
}
