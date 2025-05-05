using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] private Transform PuzzleField;
    [SerializeField] private GameObject btn;

    private void OnEnable()
    {
        if (PuzzleField.childCount > 0) return; // 이미 생성된 경우 재생성 방지

        for (int i = 0; i < 12; i++)
        {
            GameObject button = Instantiate(btn);
            button.name = "" + i;
            button.tag = "PuzzleButton"; // 꼭 필요!
            button.transform.SetParent(PuzzleField, false);
        }

        Debug.Log("카드 버튼 생성 완료");
    }

}

