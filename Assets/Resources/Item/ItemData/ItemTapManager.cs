using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemTabManager : MonoBehaviour
{
    [Header("탭 버튼")]
    public Button toyTabButton;
    public Button foodTabButton;

    [Header("배경 이미지")]
    public GameObject toyPageBackground;    // 장난감 리스트 배경
    public GameObject foodPageBackground;   // 음식 리스트 배경

    [Header("아이템 표시 UI")]
    public DraggableItem itemDisplay;
    public Button leftButton;
    public Button rightButton;

    [Header("모든 아이템 데이터")]
    public CapyItemData[] allItems;  // 모든 아이템을 한 배열에 넣으면 자동으로 타입별 분류

    private List<CapyItemData> _currentCategoryItems = new List<CapyItemData>();
    private int _currentIndex = 0;
    private CapyItemType _currentType = CapyItemType.Toy;
    private bool _isSubscribed = false;

    private void Start()
    {
        // 버튼 이벤트 연결
        toyTabButton.onClick.AddListener(() => SwitchToCategory(CapyItemType.Toy));
        foodTabButton.onClick.AddListener(() => SwitchToCategory(CapyItemType.Food));
        leftButton.onClick.AddListener(PreviousItem);
        rightButton.onClick.AddListener(NextItem);

        // 인벤토리 변경 이벤트 구독
        SubscribeToInventory();

        // 초기 설정
        SwitchToCategory(CapyItemType.Toy); // 장난감부터 시작
    }

    private void OnDestroy()
    {
        UnsubscribeFromInventory();
    }

    private void SubscribeToInventory()
    {
        if (Inventory.Instance && !_isSubscribed)
        {
            Inventory.Instance.OnInventoryChanged += OnInventoryChanged;
            _isSubscribed = true;
        }
    }

    private void UnsubscribeFromInventory()
    {
        if (Inventory.Instance && _isSubscribed)
        {
            Inventory.Instance.OnInventoryChanged -= OnInventoryChanged;
            _isSubscribed = false;
        }
    }

    private void OnInventoryChanged(CapyItemData item, int newCount)
    {
        // 현재 표시 중인 아이템이 변경되었다면 UI 업데이트
        if (_currentCategoryItems.Count > 0 &&
            _currentIndex < _currentCategoryItems.Count &&
            _currentCategoryItems[_currentIndex] == item)
        {
            UpdateDisplay();
        }

        // 카테고리 새로고침 (아이템이 0개가 되어 목록에서 사라질 수 있음)
        RefreshCurrentCategory();
    }

    public void SwitchToCategory(CapyItemType itemType)
    {
        _currentType = itemType;

        // 배경 이미지 변경
        UpdateBackgroundImages();

        // 현재 카테고리 아이템 목록 업데이트
        RefreshCurrentCategory();

        // 첫 번째 아이템 표시
        _currentIndex = 0;
        UpdateDisplay();
    }

    private void UpdateBackgroundImages()
    {
        if (_currentType == CapyItemType.Toy)
        {
            // 장난감 배경 활성화, 음식 배경 비활성화
            if (toyPageBackground) toyPageBackground.SetActive(true);
            if (foodPageBackground) foodPageBackground.SetActive(false);
        }
        else
        {
            // 음식 배경 활성화, 장난감 배경 비활성화
            if (toyPageBackground) toyPageBackground.SetActive(false);
            if (foodPageBackground) foodPageBackground.SetActive(true);
        }
    }

    private void RefreshCurrentCategory()
    {
        _currentCategoryItems.Clear();

        // 현재 타입에 맞는 모든 아이템 추가 (개수와 관계없이)
        foreach (var item in allItems)
        {
            if (item.type == _currentType)
            {
                _currentCategoryItems.Add(item);
            }
        }

        // ID나 이름으로 정렬 (선택사항)
        _currentCategoryItems.Sort((a, b) => a.displayName.CompareTo(b.displayName));

        // 현재 인덱스가 범위를 벗어났다면 조정
        if (_currentIndex >= _currentCategoryItems.Count)
        {
            _currentIndex = Mathf.Max(0, _currentCategoryItems.Count - 1);
        }
    }

    public void PreviousItem()
    {
        if (_currentCategoryItems.Count <= 1) return;

        _currentIndex--;
        if (_currentIndex < 0)
        {
            _currentIndex = _currentCategoryItems.Count - 1;
        }
        UpdateDisplay();
    }

    public void NextItem()
    {
        if (_currentCategoryItems.Count <= 1) return;

        _currentIndex++;
        if (_currentIndex >= _currentCategoryItems.Count)
        {
            _currentIndex = 0;
        }
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_currentCategoryItems.Count > 0 && _currentIndex < _currentCategoryItems.Count)
        {
            var currentItem = _currentCategoryItems[_currentIndex];
            int count = Inventory.Instance.GetCount(currentItem);

            // DraggableItem에 데이터 설정
            itemDisplay.SetItemData(currentItem, count);
            itemDisplay.gameObject.SetActive(true);

            // 좌우 버튼은 아이템이 2개 이상일 때만 클릭 가능
            bool hasMultipleItems = _currentCategoryItems.Count > 1;
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(true);
            leftButton.interactable = hasMultipleItems;
            rightButton.interactable = hasMultipleItems;
        }
        else
        {
            // 아이템이 없을 때 - 빈 상태로 표시하되 프레임은 유지
            if (itemDisplay.itemImage) itemDisplay.itemImage.sprite = null;
            if (itemDisplay.countText) itemDisplay.countText.text = "0";
            itemDisplay.gameObject.SetActive(true);  // 활성 상태 유지

            // 아이템이 없어도 화살표 버튼은 항상 표시 (비활성 상태로)
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(true);
            leftButton.interactable = false;  // 클릭 불가능하게 설정
            rightButton.interactable = false; // 클릭 불가능하게 설정
        }
    }

    /// <summary>
    /// UI 업데이트를 비활성화합니다
    /// </summary>
    public void DisableUpdates()
    {
        UnsubscribeFromInventory();
    }

    /// <summary>
    /// UI 업데이트를 다시 활성화합니다
    /// </summary>
    public void EnableUpdates()
    {
        UnsubscribeFromInventory();  // 혹시 모를 중복 구독 방지
        SubscribeToInventory();

        // 복귀 시 UI 새로고침
        RefreshCurrentCategory();
        UpdateDisplay();
    }

    /// <summary>
    /// 강제로 UI를 새로고침합니다
    /// </summary>
    public void ForceRefresh()
    {
        Debug.Log("=== ForceRefresh 호출됨 ===");
        RefreshCurrentCategory();
        UpdateDisplay();
    }
}