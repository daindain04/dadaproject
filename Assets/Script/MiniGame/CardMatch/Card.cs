using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int id;
    public Image frontImage;
    public Sprite frontSprite;
    public GameObject frontGO;
    public GameObject backGO;

    private CardGameManager gameManager;
    private bool isMatched = false;

    public void Init(CardGameManager manager, int cardId, Sprite sprite)
    {
        gameManager = manager;
        id = cardId;
        frontSprite = sprite;
        frontImage.sprite = sprite;
    }

    public void OnClick()
    {
        if (isMatched || !gameManager.CanReveal(this)) return;
        Reveal();
    }

    public void Reveal()
    {
        frontGO.SetActive(true);
        backGO.SetActive(false);
    }

    public void Hide()
    {
        frontGO.SetActive(false);
        backGO.SetActive(true);
    }

    public void SetMatched()
    {
        isMatched = true;
    }

    public bool IsMatched()
    {
        return isMatched;
    }
}
