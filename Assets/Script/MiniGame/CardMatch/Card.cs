using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour, IPointerClickHandler
{
    public Image front;
    public Image back;

    public Sprite Face { get; private set; }
    System.Action<Card> onClick;
    bool isMatched;

    public void Init(Sprite face, System.Action<Card> callback)
    {
        Face = face;
        front.sprite = face;
        onClick = callback;
        back.gameObject.SetActive(true);
        front.gameObject.SetActive(false);
        isMatched = false;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (isMatched || !back.gameObject.activeSelf) return;
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
