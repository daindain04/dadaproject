using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour, IPointerClickHandler
{
    public Image front;
    public Image back;
    public Sprite Face { get; private set; }
    System.Func<Card, bool> canClick; // Action���� Func�� ���� (bool ��ȯ)
    System.Action<Card> onClick;
    bool isMatched;

    public void Init(Sprite face, System.Func<Card, bool> canClickCallback, System.Action<Card> clickCallback)
    {
        Face = face;
        front.sprite = face;
        canClick = canClickCallback;
        onClick = clickCallback;
        back.gameObject.SetActive(true);
        front.gameObject.SetActive(false);
        isMatched = false;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (isMatched || !back.gameObject.activeSelf) return;

        // ���� Ŭ���� ���Ǵ��� Ȯ��
        if (canClick?.Invoke(this) != true) return;

        // ���� ��쿡�� ī�� ������
        back.gameObject.SetActive(false);
        front.gameObject.SetActive(true);
        onClick?.Invoke(this);
    }

    public void FlipBack()
    {
        if (isMatched) return;
        back.gameObject.SetActive(true);
        front.gameObject.SetActive(false);
    }

    public void SetMatched()
    {
        isMatched = true;
    }
}
