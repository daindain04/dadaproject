using UnityEngine;
using UnityEngine.UI;

public class CapybaraItemConsumer : MonoBehaviour
{
    [Header("�ʼ�")]
    public ConditionManager conditionManager;   // Hunger/Boredom ������

    [Header("�̸�����(����)")]
    public Image hoverPreviewImage;             // Canvas�� Image (��Ȱ�� ����)
    public Vector3 screenOffset = new Vector3(0, 80, 0);

    private Camera _cam;

    private void Awake() => _cam = Camera.main;

    private void LateUpdate()
    {
        if (hoverPreviewImage && hoverPreviewImage.enabled)
            hoverPreviewImage.rectTransform.position =
                _cam.WorldToScreenPoint(transform.position) + screenOffset;
    }

    // �巡�װ� ī�ǹٶ� ���� �ö���� ��
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

    // ���� ��� ���� ��
    public void Consume(CapyItemData item)
    {
        if (!item || conditionManager == null) return;

        // �κ��丮���� 1�� ���� (������ ����)
        if (!Inventory.Instance.TryConsume(item, 1)) return;

        // 1) ������ �ݿ�
        if (item.hungerChange != 0f) conditionManager.Feed(item.hungerChange);
        if (item.boredomChange != 0f) conditionManager.Play(item.boredomChange);

        // 2) ����ġ �ݿ� (MoneyManager ���)
        if (item.expChange > 0)
        {
            if (MoneyManager.Instance != null)
                MoneyManager.Instance.AddExperience(item.expChange);
            else
                Debug.LogWarning("[CapybaraItemConsumer] MoneyManager.Instance �� �����ϴ�.");
        }

        // 3) �̸����� ����
        HidePreview();
    }
}
