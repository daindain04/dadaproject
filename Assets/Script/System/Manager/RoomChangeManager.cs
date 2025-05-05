using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomChangeManager : MonoBehaviour
{


    [Header("�� ������Ʈ")]
    public GameObject mainFurniture;
    public GameObject kitchenFurniture;

    [Header("�ε� �г�")]
    public GameObject loadingToKitchen;
    public GameObject loadingToMain;

    [Header("�ε� ��")]
    public Image loadingBarToKitchen;
    public Image loadingBarToMain;

    [Header("�̵� ��ư")]
    public Button moveToKitchenButton;
    public Button moveToMainButton;

    [Header("�ε� �ð�")]
    public float loadingDuration = 5f;



    [Header("���� UI")]
    public GameObject buttonGroup;
    public GameObject coinBar;
    public GameObject gemBar;
    public GameObject expBar;

    [Header("ī�ǹٶ� �ȱ�")]
    public RectTransform capyWalkToKitchen;
    public RectTransform capyWalkToMain;

    public Animator capyAnimatorToKitchen;
    public Animator capyAnimatorToMain;

    // ���ι� �� �ֹ�
    public Vector2 walkStartToKitchenPos;
    public Vector2 walkEndToKitchenPos;

    // �ֹ� �� ���ι�
    public Vector2 walkStartToMainPos;
    public Vector2 walkEndToMainPos;

    public MenuToggle menuToggle;

    private void Start()
    {
        UpdateButtonStates();
    }

    // ���ι� �� �ֹ�
    public void MoveToKitchen()
    {
        StartCoroutine(TransitionToKitchen());
    }

    // �ֹ� �� ���ι�
    public void MoveToMain()
    {
        StartCoroutine(TransitionToMain());
    }


    // ���ι� �� �ֹ� �̵�
    public void GoToKitchen()
    {
        // ... �ε� ó�� ��
        menuToggle.SetRoom("Kitchen");
    }

    // �ֹ� �� ���ι� �̵�
    public void GoToMainRoom()
    {
        // ... �ε� ó�� ��
        menuToggle.SetRoom("Main");
    }


    private IEnumerator TransitionToKitchen()
    {
        // ���� UI ����
        buttonGroup.SetActive(false);
        coinBar.SetActive(false);
        gemBar.SetActive(false);
        expBar.SetActive(false);

        loadingToKitchen.SetActive(true);
        moveToKitchenButton.gameObject.SetActive(false);
        moveToMainButton.gameObject.SetActive(false);

        // Kitchen �� ī�ǹٶ� ���
        capyWalkToKitchen.localScale = new Vector3(-1, 1, 1);
        capyWalkToKitchen.anchoredPosition = walkStartToKitchenPos;
        capyAnimatorToKitchen.speed = 0.5f;
        loadingBarToKitchen.fillAmount = 0f;

        float timer = 0f;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / loadingDuration);
            loadingBarToKitchen.fillAmount = t;
            capyWalkToKitchen.anchoredPosition = Vector2.Lerp(walkStartToKitchenPos, walkEndToKitchenPos, t);

            yield return null;
        }

        mainFurniture.SetActive(false);
        kitchenFurniture.SetActive(true);
        loadingToKitchen.SetActive(false);

        buttonGroup.SetActive(true);
        coinBar.SetActive(true);
        gemBar.SetActive(true);
        expBar.SetActive(true);

        UpdateButtonStates();
        GoToKitchen();
    }

    private IEnumerator TransitionToMain()
    {
        buttonGroup.SetActive(false);
        coinBar.SetActive(false);
        gemBar.SetActive(false);
        expBar.SetActive(false);

        loadingToMain.SetActive(true);
        moveToKitchenButton.gameObject.SetActive(false);
        moveToMainButton.gameObject.SetActive(false);

        // Main �� ī�ǹٶ� ���
        capyWalkToMain.localScale = new Vector3(1, 1, 1);
        capyWalkToMain.anchoredPosition = walkStartToMainPos;
        capyAnimatorToMain.speed = 0.5f;
        loadingBarToMain.fillAmount = 0f;

        float timer = 0f;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / loadingDuration);
            loadingBarToMain.fillAmount = t;
            capyWalkToMain.anchoredPosition = Vector2.Lerp(walkStartToMainPos, walkEndToMainPos, t);

            yield return null;
        }

        kitchenFurniture.SetActive(false);
        mainFurniture.SetActive(true);
        loadingToMain.SetActive(false);

        buttonGroup.SetActive(true);
        coinBar.SetActive(true);
        gemBar.SetActive(true);
        expBar.SetActive(true);

        UpdateButtonStates();
        GoToMainRoom();
    }

    private void UpdateButtonStates()
    {
        if (mainFurniture.activeSelf)
        {
            moveToKitchenButton.gameObject.SetActive(true);
            moveToMainButton.gameObject.SetActive(false);
        }
        else if (kitchenFurniture.activeSelf)
        {
            moveToKitchenButton.gameObject.SetActive(false);
            moveToMainButton.gameObject.SetActive(true);
        }
    }
}
