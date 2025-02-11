using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Card : MonoBehaviour
{
    public int cardValue; // ī�� ���� (1~6)
    public bool isMatched = false;
    public Button button;
    public Text cardText;
    private bool isFlipped = false;

    public void SetCard(int value)
    {
        cardValue = value;
        cardText.text = "?"; // �⺻���� ����
    }

    public void FlipCard()
    {
        if (isMatched || isFlipped) return;

        isFlipped = true;
        cardText.text = cardValue.ToString();
        GameManager.instance.CardSelected(this);
    }

    public void HideCard()
    {
        isFlipped = false;
        cardText.text = "?";
    }
}
