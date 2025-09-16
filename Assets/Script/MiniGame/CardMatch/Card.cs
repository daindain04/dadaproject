using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour, IPointerClickHandler
{
    public Image front;
    public Image back;
    public Sprite Face { get; private set; }
    System.Func<Card, bool> canClick; // Action에서 Func로 변경 (bool 반환)
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

        // 먼저 클릭이 허용되는지 확인
        if (canClick?.Invoke(this) != true) return;

        // 허용된 경우에만 카드 뒤집기
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
