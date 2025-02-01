using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Events;

public enum IMAGES { Apple = 0, Banana, Grape, Kiwi, Orange, Pineapple, five = 6, four, bomb };

public class Block : MonoBehaviour
{
    [SerializeField]
    private Sprite[] imageList;
    [SerializeField]
    public IMAGES blockType;

    private Image image;

    public Node target;

    public bool needDestroy = false;

    public bool isMoving = false;
    public void Setup()
    {
        image = GetComponent<Image>();
        blockType = (IMAGES)Random.Range(0, 6);
        image.sprite = imageList[(int)blockType];
    }
    public void MoveToNode(Node to)
    {
        target = to;
    }
    public void StartMove()
    {
        float moveTime = 0.5f;
        isMoving = true;
        StartCoroutine(OnDropDownAnimation(target.localPosition, moveTime, EndMove));
    }

    public void EndMove()
    {
        target = null;
        isMoving = false;
    }

    private IEnumerator OnDropDownAnimation(Vector3 end, float time,UnityAction Action)
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
        if(Action != null) Action.Invoke();
    }
    public void DestroyBlock()
    {
        gameObject.SetActive(false);
        Destroy(this);
    }
    public void ChangeImage(IMAGES type)
    {
        blockType = type;
        image.sprite = imageList[(int)type];
    }
}
