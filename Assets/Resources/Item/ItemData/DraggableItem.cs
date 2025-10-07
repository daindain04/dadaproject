using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("아이템 정보")]
    public CapyItemData itemData;

    [Header("UI 참조")]
    public Image itemImage;
    public TextMeshProUGUI countText;

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Vector3 _originalPosition;
    private Transform _originalParent;
    private GameObject _dragInstance;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetItemData(CapyItemData data, int count)
    {

        itemData = data;

        if (data != null)
        {
            if (itemImage) itemImage.sprite = data.icon;
            if (countText) countText.text = count.ToString();
        }
        else
        {
            // data가 null인 경우 (빈 상태)
            if (itemImage) itemImage.sprite = null;
            if (countText) countText.text = "0";
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemData == null || Inventory.Instance.GetCount(itemData) <= 0) return;

        // 드래그용 이미지만 생성 (전체 프레임이 아닌 아이템 이미지만)
        GameObject dragImageObject = new GameObject("DragImage");
        dragImageObject.transform.SetParent(_canvas.transform, false);

        Image dragImage = dragImageObject.AddComponent<Image>();
        dragImage.sprite = itemData.icon; // 아이템 아이콘만 사용
        dragImage.raycastTarget = false;

        // 드래그 이미지 크기 설정 (원본보다 조금 작게)
        RectTransform dragRect = dragImageObject.GetComponent<RectTransform>();
        Vector2 originalSize = itemImage.rectTransform.sizeDelta;
        dragRect.sizeDelta = originalSize * 0.8f; // 80% 크기로 축소

        // 투명도 설정
        CanvasGroup dragCanvasGroup = dragImageObject.AddComponent<CanvasGroup>();
        dragCanvasGroup.alpha = 0.8f;
        dragCanvasGroup.blocksRaycasts = false;

        _dragInstance = dragImageObject;

        // 원본 프레임은 약간 투명하게
        _canvasGroup.alpha = 0.7f;

        // 카피바라에게 미리보기 표시
        var consumer = FindObjectOfType<CapybaraItemConsumer>();
        if (consumer) consumer.ShowPreview(itemData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragInstance)
        {
            _dragInstance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 복사본 제거
        if (_dragInstance)
        {
            Destroy(_dragInstance);
        }

        // 원본 투명도 복원
        _canvasGroup.alpha = 1f;

        // 카피바라에게 드롭했는지 확인
        var consumer = FindObjectOfType<CapybaraItemConsumer>();
        if (consumer)
        {
            // 카피바라 영역에 드롭했는지 레이캐스트로 확인
            if (IsOverCapybara(eventData.position))
            {
                consumer.Consume(itemData);
                // 아이템 사용 후 UI 업데이트
                UpdateUI();
            }
            consumer.HidePreview();
        }
    }

    private bool IsOverCapybara(Vector2 screenPosition)
    {
        // 스크린 좌표를 월드 좌표로 변환
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Camera.main.nearClipPlane));
        worldPos.z = 0;

        // 카피바라 콜라이더와 겹치는지 확인
        var consumer = FindObjectOfType<CapybaraItemConsumer>();
        if (consumer)
        {
            var collider = consumer.GetComponent<Collider2D>();
            if (collider && collider.OverlapPoint(worldPos))
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateUI()
    {
        if (itemData)
        {
            int currentCount = Inventory.Instance.GetCount(itemData);
            SetItemData(itemData, currentCount);
        }
    }

}