using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionManager : MonoBehaviour
{
    public Image hungerFill;
    public Image boredomFill;
    public Image happinessFill;

    private float hunger = 70f;      // ← 초기값 변경
    private float boredom = 70f;     // ← 초기값 변경
    private float timer = 0f;

    private float maxWidth;

    void Start()
    {
        maxWidth = hungerFill.rectTransform.sizeDelta.x;

        // 초기 시작 시 바로 반영
        UpdateGauge(hungerFill, hunger);
        UpdateGauge(boredomFill, boredom);
        UpdateGauge(happinessFill, (hunger + boredom) / 2f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 60f)
        {
            hunger -= 2f;
            boredom -= 1f;
            timer = 0f;
        }

        hunger = Mathf.Clamp(hunger, 0f, 100f);
        boredom = Mathf.Clamp(boredom, 0f, 100f);
        float happiness = (hunger + boredom) / 2f;

        UpdateGauge(hungerFill, hunger);
        UpdateGauge(boredomFill, boredom);
        UpdateGauge(happinessFill, happiness);
    }

    void UpdateGauge(Image gaugeImage, float value)
    {
        Vector2 size = gaugeImage.rectTransform.sizeDelta;
        size.x = maxWidth * (value / 100f);
        gaugeImage.rectTransform.sizeDelta = size;
    }

    public void Feed(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0f, 100f);
    }

    public void Play(float amount)
    {
        boredom = Mathf.Clamp(boredom + amount, 0f, 100f);
    }
}
