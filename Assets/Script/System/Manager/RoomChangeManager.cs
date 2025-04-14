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

    [Header("ī�ǹٶ� �ȱ�")]
    public RectTransform capyWalk;
    public Animator capyAnimator;

    // ���ι� �� �ֹ�
    public Vector2 walkStartToKitchenPos;
    public Vector2 walkEndToKitchenPos;

    // �ֹ� �� ���ι�
    public Vector2 walkStartToMainPos;
    public Vector2 walkEndToMainPos;

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

    private IEnumerator TransitionToKitchen()
    {
        loadingToKitchen.SetActive(true);
        moveToKitchenButton.gameObject.SetActive(false);
        moveToMainButton.gameObject.SetActive(false);

        capyWalk.localScale = new Vector3(-1, 1, 1); // �� ����

        if (capyWalk != null)
            capyWalk.anchoredPosition = walkStartToKitchenPos;
        if (capyAnimator != null)
            capyAnimator.speed = 0.5f; // �ִϸ��̼� ������
        if (loadingBarToKitchen != null)
            loadingBarToKitchen.fillAmount = 0f;

        float timer = 0f;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / loadingDuration);

            if (loadingBarToKitchen != null)
                loadingBarToKitchen.fillAmount = t;

            if (capyWalk != null)
                capyWalk.anchoredPosition = Vector2.Lerp(walkStartToKitchenPos, walkEndToKitchenPos, t);

            yield return null;
        }

        mainFurniture.SetActive(false);
        kitchenFurniture.SetActive(true);
        loadingToKitchen.SetActive(false);

        UpdateButtonStates();
    }

    private IEnumerator TransitionToMain()
    {
        loadingToMain.SetActive(true);
        moveToKitchenButton.gameObject.SetActive(false);
        moveToMainButton.gameObject.SetActive(false);

        //  �����ʿ��� �������� �ȱ�
        capyWalk.localScale = new Vector3(1, 1, 1); // ���� ����

        if (capyWalk != null)
            capyWalk.anchoredPosition = walkStartToMainPos; // �����ʿ��� ���

        if (capyAnimator != null)
            capyAnimator.speed = 0.5f;

        if (loadingBarToMain != null)
            loadingBarToMain.fillAmount = 0f;

        float timer = 0f;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / loadingDuration);

            if (loadingBarToMain != null)
                loadingBarToMain.fillAmount = t;

            if (capyWalk != null)
                capyWalk.anchoredPosition = Vector2.Lerp(walkStartToMainPos, walkEndToMainPos, t); // ���������� �̵�

            yield return null;
        }

        kitchenFurniture.SetActive(false);
        mainFurniture.SetActive(true);
        loadingToMain.SetActive(false);

        UpdateButtonStates();
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
