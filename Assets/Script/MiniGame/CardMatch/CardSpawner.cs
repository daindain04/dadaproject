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
        if (PuzzleField.childCount > 0) return; // �̹� ������ ��� ����� ����

        for (int i = 0; i < 12; i++)
        {
            GameObject button = Instantiate(btn);
            button.name = "" + i;
            button.tag = "PuzzleButton"; // �� �ʿ�!
            button.transform.SetParent(PuzzleField, false);
        }

        Debug.Log("ī�� ��ư ���� �Ϸ�");
    }

}

