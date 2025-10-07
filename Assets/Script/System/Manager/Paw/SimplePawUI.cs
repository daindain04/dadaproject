using UnityEngine;
using UnityEngine.UI;

public class SimplePawUI : MonoBehaviour
{
    void Awake()
    {
        // 클릭 이벤트 차단하지 않도록 설정
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = false;
        }

        // 기본 설정
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = Vector3.one * 0.5f; // 초기 크기
        }
    }
}