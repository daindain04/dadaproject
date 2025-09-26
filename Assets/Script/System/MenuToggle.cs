using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuToggle : MonoBehaviour
{
    [Header("Button Lists")]
    public List<RectTransform> mainRoomButtons = new();  // 메인방에서 보일 버튼들
    public List<RectTransform> kitchenRoomButtons = new(); // 주방에서 보일 버튼들

    [Header("Room Switch Buttons")]
    public GameObject MoveToKitchenB;
    public GameObject MoveToMainB;

    [Header("UI Images to Change")]
    public List<Image> uiImagesToChange = new(); // 방 전환시 바뀔 UI 이미지들

    [Header("Sprites for Different Rooms")]
    public List<Sprite> mainRoomSprites = new(); // 메인방용 스프라이트들
    public List<Sprite> kitchenRoomSprites = new(); // 주방용 스프라이트들

    private List<RectTransform> currentList; // 현재 활성화 대상
    public float interval = 0.05f;
    public float scaleSpeed = 10f;
    private bool isVisible = true;

    private void Start()
    {
        // 먼저 모든 버튼을 보이는 상태로 초기화
        SetRoom("Main");

        // 현재 리스트의 모든 버튼을 보이는 상태로 설정
        foreach (var btn in currentList)
        {
            btn.gameObject.SetActive(true);
            btn.localScale = Vector3.one;
        }

        isVisible = true; // 보이는 상태로 시작
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

            // Room Switch 버튼들은 별도로 관리하지 않음 (리스트에서 관리)
            // if (MoveToKitchenB != null) MoveToKitchenB.SetActive(true);
            // if (MoveToMainB != null) MoveToMainB.SetActive(false);

            // UI 이미지를 메인방용으로 변경
            ChangeUISprites(mainRoomSprites);
        }
        else if (room == "Kitchen")
        {
            foreach (var btn in mainRoomButtons)
            {
                btn.gameObject.SetActive(false);
                btn.localScale = new Vector3(1, 0, 1);
            }
            currentList = kitchenRoomButtons;

            // Room Switch 버튼들은 별도로 관리하지 않음 (리스트에서 관리)
            // if (MoveToKitchenB != null) MoveToKitchenB.SetActive(false);
            // if (MoveToMainB != null) MoveToMainB.SetActive(true);

            // UI 이미지를 주방용으로 변경
            ChangeUISprites(kitchenRoomSprites);
        }
        Debug.Log("SetRoom 호출됨: " + room);
    }

    // UI 스프라이트 변경 메서드
    private void ChangeUISprites(List<Sprite> newSprites)
    {
        for (int i = 0; i < uiImagesToChange.Count && i < newSprites.Count; i++)
        {
            if (uiImagesToChange[i] != null && newSprites[i] != null)
            {
                uiImagesToChange[i].sprite = newSprites[i];
            }
        }
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