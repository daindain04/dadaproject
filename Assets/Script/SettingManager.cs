using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public GameObject settingPanel; // ���� â
    public Toggle backgroundMusicToggle; // ����� ON/OFF Toggle
    public Toggle effectSoundToggle; // ȿ���� ON/OFF Toggle

    void Start()
    {
        // ���� â�� ó������ ��Ȱ��ȭ
        settingPanel.SetActive(false);

        // ����� ���� ���� �ε�
        backgroundMusicToggle.isOn = PlayerPrefs.GetInt("BackgroundMusic", 1) == 1;
        effectSoundToggle.isOn = PlayerPrefs.GetInt("EffectSound", 1) == 1;

        // ��� ���� ���� �̺�Ʈ ������ ���
        backgroundMusicToggle.onValueChanged.AddListener(ToggleBackgroundMusic);
        effectSoundToggle.onValueChanged.AddListener(ToggleEffectSound);
    }

    // ���� â Ȱ��ȭ
    public void OpenSettingPanel()
    {
        settingPanel.SetActive(true);
    }

    // ���� â ��Ȱ��ȭ
    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
    }

    // ����� ON/OFF
    public void ToggleBackgroundMusic(bool isOn)
    {
        PlayerPrefs.SetInt("BackgroundMusic", isOn ? 1 : 0);
        PlayerPrefs.Save();

        AudioManager.Instance.ToggleBackgroundMusic(isOn);
    }

    // ȿ���� ON/OFF
    public void ToggleEffectSound(bool isOn)
    {
        PlayerPrefs.SetInt("EffectSound", isOn ? 1 : 0);
        PlayerPrefs.Save();

        AudioManager.Instance.ToggleEffectSound(isOn);
    }
}
