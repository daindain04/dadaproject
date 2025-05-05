using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MenuToggle : MonoBehaviour
{
    public List<RectTransform> mainRoomButtons = new();  // ���ι濡�� ���� ��ư��
    public List<RectTransform> kitchenRoomButtons = new(); // �ֹ濡�� ���� ��ư��

    public GameObject MoveToKitchenB;
    public GameObject MoveToMainB;

    private List<RectTransform> currentList; // ���� Ȱ��ȭ ���
    public float interval = 0.05f;
    public float scaleSpeed = 10f;
    private bool isVisible = true;

    private void Start()
    {
        SetRoom("Main"); // ������ ���ι��̶�� ����
    }

    public void SetRoom(string room)
    {
        StopAllCoroutines(); // ���� �ִϸ��̼� �ߴ�
        isVisible = false; // ����Ʈ ���� �ʱ�ȭ

        if (room == "Main")
        {
            // ��� ��ư�� ���� ���ְ�
            foreach (var btn in kitchenRoomButtons)
            {
                btn.gameObject.SetActive(false);
                btn.localScale = new Vector3(1, 0, 1);
            }

            // ����Ʈ ��ȯ
            currentList = mainRoomButtons;

            // ���� ��ȯ
            if (MoveToKitchenB != null) MoveToKitchenB.SetActive(true);
            if (MoveToMainB != null) MoveToMainB.SetActive(false);
        }
        else if (room == "Kitchen")
        {
            foreach (var btn in mainRoomButtons)
            {
                btn.gameObject.SetActive(false);
                btn.localScale = new Vector3(1, 0, 1);
            }

            currentList = kitchenRoomButtons;

            if (MoveToKitchenB != null) MoveToKitchenB.SetActive(false);
            if (MoveToMainB != null) MoveToMainB.SetActive(true);
        }

        Debug.Log("SetRoom ȣ���: " + room);
    }

    public void ToggleMenu()
    {
        StopAllCoroutines();
        if (isVisible)
            StartCoroutine(HideButtons(currentList));
        else
            StartCoroutine(ShowButtons(currentList));

        isVisible = !isVisible;
    }

    private IEnumerator ShowButtons(List<RectTransform> list)
    {
        foreach (RectTransform btn in list)
        {
            btn.gameObject.SetActive(true);
            btn.localScale = new Vector3(1, 0, 1);
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * scaleSpeed;
                float scaleY = Mathf.Lerp(0, 1, t);
                btn.localScale = new Vector3(1, scaleY, 1);
                yield return null;
            }
            btn.localScale = Vector3.one;
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator HideButtons(List<RectTransform> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            RectTransform btn = list[i];
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * scaleSpeed;
                float scaleY = Mathf.Lerp(1, 0, t);
                btn.localScale = new Vector3(1, scaleY, 1);
                yield return null;
            }
            btn.localScale = new Vector3(1, 0, 1);
            btn.gameObject.SetActive(false);
            yield return new WaitForSeconds(interval);
        }
    }
}
