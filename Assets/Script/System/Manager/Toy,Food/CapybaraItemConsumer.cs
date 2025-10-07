using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CapybaraItemConsumer : MonoBehaviour
{
    [Header("필수 참조")]
    public ConditionManager conditionManager;

    [Header("미리보기 UI")]
    public GameObject previewPanel;         // 미리보기 패널
    public Image itemPreviewImage;          // 아이템 미리보기 이미지 (효과가 포함된 이미지)

    [Header("설정")]
    public Vector3 worldOffset = new Vector3(0, 2f, 0);  // 카피바라 기준 월드 좌표 오프셋

    private Camera _cam;
    private Collider2D _collider;

    private void Awake()
    {
        _cam = Camera.main;
        _collider = GetComponent<Collider2D>();

        // 미리보기 패널 비활성화
        if (previewPanel) previewPanel.SetActive(false);
    }

    private void LateUpdate()
    {
        if (previewPanel && previewPanel.activeSelf)
        {
            // 카피바라 월드 좌표 + 오프셋을 스크린 좌표로 변환
            Vector3 targetWorldPos = transform.position + worldOffset;
            Vector3 screenPos = _cam.WorldToScreenPoint(targetWorldPos);

            // 미리보기 패널을 해당 스크린 좌표에 위치시키기
            previewPanel.transform.position = screenPos;
        }
    }

    public void ShowPreview(CapyItemData item)
    {
        if (!previewPanel || !item) return;

        // 미리보기 이미지 설정 (효과가 포함된 이미지)
        if (itemPreviewImage)
        {
            itemPreviewImage.sprite = item.previewSprite ? item.previewSprite : item.icon;
        }

        previewPanel.SetActive(true);
    }

    public void HidePreview()
    {
        if (previewPanel) previewPanel.SetActive(false);
    }

    public void Consume(CapyItemData item)
    {
        if (!item || conditionManager == null) return;

        // 인벤토리에서 1개 차감
        if (!Inventory.Instance.TryConsume(item, 1))
        {
            Debug.Log($"아이템이 부족합니다: {item.displayName}");
            return;
        }

        // 효과 적용
        if (item.hungerChange != 0f)
        {
            conditionManager.Feed(item.hungerChange);
            Debug.Log($"배고픔 {item.hungerChange:F1} 변화");
        }

        if (item.boredomChange != 0f)
        {
            conditionManager.Play(item.boredomChange);
            Debug.Log($"지루함 {item.boredomChange:F1} 변화");
        }

        if (item.expChange > 0)
        {
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.AddExperience(item.expChange);
                Debug.Log($"경험치 +{item.expChange}");
            }
        }

        // 미리보기 숨기기
        HidePreview();

        Debug.Log($"{item.displayName}을(를) 카피바라에게 주었습니다!");
    }

    // 드래그 감지를 위한 영역 확인
    public bool IsPointInside(Vector2 worldPoint)
    {
        return _collider && _collider.OverlapPoint(worldPoint);
    }
}