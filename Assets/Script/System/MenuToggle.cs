using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MenuToggle : MonoBehaviour
{
    public List<RectTransform> mainRoomButtons = new();  // 메인방에서 보일 버튼들
    public List<RectTransform> kitchenRoomButtons = new(); // 주방에서 보일 버튼들

    public GameObject MoveToKitchenB;
    public GameObject MoveToMainB;

    private List<RectTransform> currentList; // 현재 활성화 대상
    public float interval = 0.05f;
    public float scaleSpeed = 10f;
    private bool isVisible = true;

    private void Start()
    {
        SetRoom("Main"); // 시작은 메인방이라고 가정
    }

    public void SetRoom(string room)
    {
        StopAllCoroutines(); // 이전 애니메이션 중단
        isVisible = false; // 리스트 상태 초기화

        if (room == "Main")
        {
            // 모든 버튼을 먼저 꺼주고
            foreach (var btn in kitchenRoomButtons)
            {
                btn.gameObject.SetActive(false);
                btn.localScale = new Vector3(1, 0, 1);
            }

            // 리스트 전환
            currentList = mainRoomButtons;

            // 상태 전환
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

        Debug.Log("SetRoom 호출됨: " + room);
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
