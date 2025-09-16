using UnityEngine;
using System.Collections.Generic;

public class MainRoomUIManager : MonoBehaviour
{
    [Header("���ι� UI ������Ʈ��")]
    [SerializeField] private List<GameObject> mainRoomUIObjects = new List<GameObject>();

    [Header("���� UI ������Ʈ�� (���û���)")]
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
        // �̱��� ���� ����
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
        // ���� �� ��� ���ι� UI Ȱ��ȭ
        ShowMainRoomUI();
    }

    /// <summary>
    /// ���ι� UI�� ��� Ȱ��ȭ�մϴ�
    /// </summary>
    public void ShowMainRoomUI()
    {
        SetMainRoomUIActive(true);
        Debug.Log("���ι� UI Ȱ��ȭ��");
    }

    /// <summary>
    /// ���ι� UI�� ��� ��Ȱ��ȭ�մϴ� (���� ���� �� ȣ��)
    /// </summary>
    public void HideMainRoomUI()
    {
        SetMainRoomUIActive(false);
        Debug.Log("���ι� UI ��Ȱ��ȭ��");
    }

    /// <summary>
    /// ���ι� UI Ȱ��ȭ/��Ȱ��ȭ�� �ϰ� ó���մϴ�
    /// </summary>
    /// <param name="isActive">Ȱ��ȭ ����</param>
    private void SetMainRoomUIActive(bool isActive)
    {
        // GameObject ����Ʈ ó��
        foreach (GameObject uiObject in mainRoomUIObjects)
        {
            if (uiObject != null)
            {
                uiObject.SetActive(isActive);
            }
        }

        // MonoBehaviour ������Ʈ ����Ʈ ó��
        foreach (MonoBehaviour uiComponent in mainRoomUIComponents)
        {
            if (uiComponent != null)
            {
                uiComponent.enabled = isActive;
            }
        }
    }

    /// <summary>
    /// ��Ÿ�ӿ��� ���ι� UI ������Ʈ�� �߰��մϴ�
    /// </summary>
    /// <param name="uiObject">�߰��� UI ������Ʈ</param>
    public void AddMainRoomUIObject(GameObject uiObject)
    {
        if (uiObject != null && !mainRoomUIObjects.Contains(uiObject))
        {
            mainRoomUIObjects.Add(uiObject);
        }
    }

    /// <summary>
    /// ��Ÿ�ӿ��� ���ι� UI ������Ʈ�� �߰��մϴ�
    /// </summary>
    /// <param name="uiComponent">�߰��� UI ������Ʈ</param>
    public void AddMainRoomUIComponent(MonoBehaviour uiComponent)
    {
        if (uiComponent != null && !mainRoomUIComponents.Contains(uiComponent))
        {
            mainRoomUIComponents.Add(uiComponent);
        }
    }

    /// <summary>
    /// ���ι� UI ������Ʈ�� �����մϴ�
    /// </summary>
    /// <param name="uiObject">������ UI ������Ʈ</param>
    public void RemoveMainRoomUIObject(GameObject uiObject)
    {
        if (mainRoomUIObjects.Contains(uiObject))
        {
            mainRoomUIObjects.Remove(uiObject);
        }
    }

    /// <summary>
    /// ���ι� UI ������Ʈ�� �����մϴ�
    /// </summary>
    /// <param name="uiComponent">������ UI ������Ʈ</param>
    public void RemoveMainRoomUIComponent(MonoBehaviour uiComponent)
    {
        if (mainRoomUIComponents.Contains(uiComponent))
        {
            mainRoomUIComponents.Remove(uiComponent);
        }
    }

    /// <summary>
    /// ���� ���ι� UI�� Ȱ��ȭ�Ǿ� �ִ��� Ȯ���մϴ�
    /// </summary>
    /// <returns>Ȱ��ȭ ����</returns>
    public bool IsMainRoomUIActive()
    {
        if (mainRoomUIObjects.Count > 0)
        {
            return mainRoomUIObjects[0] != null && mainRoomUIObjects[0].activeSelf;
        }
        return false;
    }

    /// <summary>
    /// ���ι� UI ���¸� ����մϴ�
    /// </summary>
    public void ToggleMainRoomUI()
    {
        bool currentState = IsMainRoomUIActive();
        SetMainRoomUIActive(!currentState);

        if (currentState)
        {
            Debug.Log("���ι� UI ��Ȱ��ȭ��");
        }
        else
        {
            Debug.Log("���ι� UI Ȱ��ȭ��");
        }
    }
}