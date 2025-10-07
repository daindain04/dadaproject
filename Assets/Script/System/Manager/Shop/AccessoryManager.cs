using UnityEngine;
using System.Collections.Generic;

public class AccessoryManager : MonoBehaviour
{
    public static AccessoryManager Instance { get; private set; }

    [Header("카피바라 악세사리")]
    public List<GameObject> accessories = new List<GameObject>(); // 9개의 악세사리 오브젝트

    private const string CURRENT_ACCESSORY_KEY = "CurrentAccessory";
    private int currentAccessoryIndex = -1; // -1은 착용 안함

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
            return;
        }

        // Awake에서 악세사리 리스트 초기화 (자식 오브젝트 자동 수집)
        CollectAccessories();
        InitializeAccessories();
    }

    private void Start()
    {
        // 게임 시작 또는 씬 로드 시 저장된 악세사리 복원
        LoadCurrentAccessory();
    }

    // 자식 오브젝트에서 악세사리 자동 수집
    private void CollectAccessories()
    {
        // Inspector에 이미 할당되어 있으면 그대로 사용
        if (accessories.Count > 0 && accessories[0] != null)
        {
            Debug.Log($"악세사리 {accessories.Count}개가 이미 할당되어 있습니다.");
            return;
        }

        // 자식 오브젝트 자동 수집
        accessories.Clear();
        foreach (Transform child in transform)
        {
            accessories.Add(child.gameObject);
        }

        Debug.Log($"악세사리 {accessories.Count}개를 자동으로 수집했습니다.");
    }

    // 모든 악세사리 비활성화로 초기화
    private void InitializeAccessories()
    {
        foreach (var accessory in accessories)
        {
            if (accessory != null)
            {
                accessory.SetActive(false);
            }
        }
    }

    // 저장된 악세사리 정보 불러오기
    private void LoadCurrentAccessory()
    {
        currentAccessoryIndex = PlayerPrefs.GetInt(CURRENT_ACCESSORY_KEY, -1);

        if (currentAccessoryIndex >= 0 && currentAccessoryIndex < accessories.Count)
        {
            ActivateAccessory(currentAccessoryIndex);
        }
    }

    // 악세사리 활성화 (구매 시 호출)
    public void EquipAccessory(int accessoryIndex)
    {
        if (accessoryIndex < 0 || accessoryIndex >= accessories.Count)
        {
            Debug.LogError($"잘못된 악세사리 인덱스: {accessoryIndex}");
            return;
        }

        // 기존 악세사리 비활성화
        if (currentAccessoryIndex >= 0 && currentAccessoryIndex < accessories.Count)
        {
            accessories[currentAccessoryIndex].SetActive(false);
        }

        // 새 악세사리 활성화
        ActivateAccessory(accessoryIndex);

        // 현재 악세사리 인덱스 저장
        currentAccessoryIndex = accessoryIndex;
        PlayerPrefs.SetInt(CURRENT_ACCESSORY_KEY, currentAccessoryIndex);
        PlayerPrefs.Save();

        Debug.Log($"악세사리 착용: {accessories[accessoryIndex].name}");
    }

    private void ActivateAccessory(int index)
    {
        if (index >= 0 && index < accessories.Count && accessories[index] != null)
        {
            accessories[index].SetActive(true);
        }
    }

    // 현재 착용 중인 악세사리 인덱스 반환
    public int GetCurrentAccessoryIndex()
    {
        return currentAccessoryIndex;
    }

    // 악세사리를 착용 중인지 확인
    public bool IsWearingAccessory()
    {
        return currentAccessoryIndex >= 0;
    }

    // 모든 악세사리 초기화 (구매 데이터 리셋 시 호출)
    public void ResetAllAccessories()
    {
        // 모든 악세사리 비활성화
        foreach (var accessory in accessories)
        {
            if (accessory != null)
            {
                accessory.SetActive(false);
            }
        }

        // 저장된 데이터 삭제
        currentAccessoryIndex = -1;
        PlayerPrefs.DeleteKey(CURRENT_ACCESSORY_KEY);
        PlayerPrefs.Save();

        Debug.Log("모든 악세사리가 초기화되었습니다.");
    }
}