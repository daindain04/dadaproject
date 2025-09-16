using UnityEngine;
using UnityEngine.UI;

public class CapybaraItemConsumer : MonoBehaviour
{
    [Header("필수")]
    public ConditionManager conditionManager;   // Hunger/Boredom 게이지

    [Header("미리보기(선택)")]
    public Image hoverPreviewImage;             // Canvas의 Image (비활성 시작)
    public Vector3 screenOffset = new Vector3(0, 80, 0);

    private Camera _cam;

    private void Awake() => _cam = Camera.main;

    private void LateUpdate()
    {
        if (hoverPreviewImage && hoverPreviewImage.enabled)
            hoverPreviewImage.rectTransform.position =
                _cam.WorldToScreenPoint(transform.position) + screenOffset;
    }

    // 드래그가 카피바라 위에 올라왔을 때
    public void ShowPreview(CapyItemData item)
    {
        if (!hoverPreviewImage || !item) return;
        hoverPreviewImage.sprite = item.previewSprite ? item.previewSprite : item.icon;
        hoverPreviewImage.enabled = true;
    }

    public void HidePreview()
    {
        if (hoverPreviewImage) hoverPreviewImage.enabled = false;
    }

    // 실제 드롭 성공 시
    public void Consume(CapyItemData item)
    {
        if (!item || conditionManager == null) return;

        // 인벤토리에서 1개 차감 (없으면 종료)
        if (!Inventory.Instance.TryConsume(item, 1)) return;

        // 1) 게이지 반영
        if (item.hungerChange != 0f) conditionManager.Feed(item.hungerChange);
        if (item.boredomChange != 0f) conditionManager.Play(item.boredomChange);

        // 2) 경험치 반영 (MoneyManager 사용)
        if (item.expChange > 0)
        {
            if (MoneyManager.Instance != null)
                MoneyManager.Instance.AddExperience(item.expChange);
            else
                Debug.LogWarning("[CapybaraItemConsumer] MoneyManager.Instance 가 없습니다.");
        }

        // 3) 미리보기 끄기
        HidePreview();
    }
}
