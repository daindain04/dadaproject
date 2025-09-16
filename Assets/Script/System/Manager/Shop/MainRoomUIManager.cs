using UnityEngine;
using System.Collections.Generic;

public class MainRoomUIManager : MonoBehaviour
{
    [Header("메인방 UI 오브젝트들")]
    [SerializeField] private List<GameObject> mainRoomUIObjects = new List<GameObject>();

    [Header("개별 UI 컴포넌트들 (선택사항)")]
    [SerializeField] private List<MonoBehaviour> mainRoomUIComponents = new List<MonoBehaviour>();

    private static MainRoomUIManager instance;
    public static MainRoomUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MainRoomUIManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 싱글톤 패턴 적용
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 시작 시 모든 메인방 UI 활성화
        ShowMainRoomUI();
    }

    /// <summary>
    /// 메인방 UI를 모두 활성화합니다
    /// </summary>
    public void ShowMainRoomUI()
    {
        SetMainRoomUIActive(true);
        Debug.Log("메인방 UI 활성화됨");
    }

    /// <summary>
    /// 메인방 UI를 모두 비활성화합니다 (상점 진입 시 호출)
    /// </summary>
    public void HideMainRoomUI()
    {
        SetMainRoomUIActive(false);
        Debug.Log("메인방 UI 비활성화됨");
    }

    /// <summary>
    /// 메인방 UI 활성화/비활성화를 일괄 처리합니다
    /// </summary>
    /// <param name="isActive">활성화 여부</param>
    private void SetMainRoomUIActive(bool isActive)
    {
        // GameObject 리스트 처리
        foreach (GameObject uiObject in mainRoomUIObjects)
        {
            if (uiObject != null)
            {
                uiObject.SetActive(isActive);
            }
        }

        // MonoBehaviour 컴포넌트 리스트 처리
        foreach (MonoBehaviour uiComponent in mainRoomUIComponents)
        {
            if (uiComponent != null)
            {
                uiComponent.enabled = isActive;
            }
        }
    }

    /// <summary>
    /// 런타임에서 메인방 UI 오브젝트를 추가합니다
    /// </summary>
    /// <param name="uiObject">추가할 UI 오브젝트</param>
    public void AddMainRoomUIObject(GameObject uiObject)
    {
        if (uiObject != null && !mainRoomUIObjects.Contains(uiObject))
        {
            mainRoomUIObjects.Add(uiObject);
        }
    }

    /// <summary>
    /// 런타임에서 메인방 UI 컴포넌트를 추가합니다
    /// </summary>
    /// <param name="uiComponent">추가할 UI 컴포넌트</param>
    public void AddMainRoomUIComponent(MonoBehaviour uiComponent)
    {
        if (uiComponent != null && !mainRoomUIComponents.Contains(uiComponent))
        {
            mainRoomUIComponents.Add(uiComponent);
        }
    }

    /// <summary>
    /// 메인방 UI 오브젝트를 제거합니다
    /// </summary>
    /// <param name="uiObject">제거할 UI 오브젝트</param>
    public void RemoveMainRoomUIObject(GameObject uiObject)
    {
        if (mainRoomUIObjects.Contains(uiObject))
        {
            mainRoomUIObjects.Remove(uiObject);
        }
    }

    /// <summary>
    /// 메인방 UI 컴포넌트를 제거합니다
    /// </summary>
    /// <param name="uiComponent">제거할 UI 컴포넌트</param>
    public void RemoveMainRoomUIComponent(MonoBehaviour uiComponent)
    {
        if (mainRoomUIComponents.Contains(uiComponent))
        {
            mainRoomUIComponents.Remove(uiComponent);
        }
    }

    /// <summary>
    /// 현재 메인방 UI가 활성화되어 있는지 확인합니다
    /// </summary>
    /// <returns>활성화 여부</returns>
    public bool IsMainRoomUIActive()
    {
        if (mainRoomUIObjects.Count > 0)
        {
            return mainRoomUIObjects[0] != null && mainRoomUIObjects[0].activeSelf;
        }
        return false;
    }

    /// <summary>
    /// 메인방 UI 상태를 토글합니다
    /// </summary>
    public void ToggleMainRoomUI()
    {
        bool currentState = IsMainRoomUIActive();
        SetMainRoomUIActive(!currentState);

        if (currentState)
        {
            Debug.Log("메인방 UI 비활성화됨");
        }
        else
        {
            Debug.Log("메인방 UI 활성화됨");
        }
    }
}