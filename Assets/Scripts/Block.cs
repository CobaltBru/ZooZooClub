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
        float moveTime = 1000f;
        isMoving = true;
        StartCoroutine(OnDropDownAnimation(target.localPosition, moveTime, EndMove));
    }

    public void EndMove()
    {
        target = null;
        isMoving = false;
    }

    private IEnumerator OnDropDownAnimation(Vector3 end, float speed,UnityAction Action)
    {
        
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 start = rectTransform.localPosition;

        float distance = Vector3.Distance(start, end); // ��ü �̵� �Ÿ�
        float duration = distance / speed; // �̵� �ð��� ���� �ӵ��� ����
        float current = 0f;

        while (current < duration)
        {
            current += Time.deltaTime;
            float percent = current / duration;
            rectTransform.localPosition = Vector3.Lerp(start, end, percent);
            yield return null;
        }

        rectTransform.localPosition = end; // ���� ��ġ ����

        Action?.Invoke();
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
