using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionUIController : MonoBehaviour
{
    public GameObject gaugeHunger;
    public GameObject gaugeBoredom;
    public GameObject gaugeHappiness;

    public void ShowHunger()
    {
        gaugeHunger.SetActive(true);
        gaugeBoredom.SetActive(false);
        gaugeHappiness.SetActive(false);
    }

    public void ShowBoredom()
    {
        gaugeHunger.SetActive(false);
        gaugeBoredom.SetActive(true);
        gaugeHappiness.SetActive(false);
    }

    public void ShowHappiness()
    {
        gaugeHunger.SetActive(false);
        gaugeBoredom.SetActive(false);
        gaugeHappiness.SetActive(true);
    }
}
