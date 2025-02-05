using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public GameObject settingPanel; // 설정 창
    public Toggle backgroundMusicToggle; // 배경음 ON/OFF Toggle
    public Toggle effectSoundToggle; // 효과음 ON/OFF Toggle

    void Start()
    {
        // 설정 창을 처음에는 비활성화
        settingPanel.SetActive(false);

        // 저장된 사운드 설정 로드
        backgroundMusicToggle.isOn = PlayerPrefs.GetInt("BackgroundMusic", 1) == 1;
        effectSoundToggle.isOn = PlayerPrefs.GetInt("EffectSound", 1) == 1;

        // 토글 상태 변경 이벤트 리스너 등록
        backgroundMusicToggle.onValueChanged.AddListener(ToggleBackgroundMusic);
        effectSoundToggle.onValueChanged.AddListener(ToggleEffectSound);
    }

    // 설정 창 활성화
    public void OpenSettingPanel()
    {
        settingPanel.SetActive(true);
    }

    // 설정 창 비활성화
    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
    }

    // 배경음 ON/OFF
    public void ToggleBackgroundMusic(bool isOn)
    {
        PlayerPrefs.SetInt("BackgroundMusic", isOn ? 1 : 0);
        PlayerPrefs.Save();

        AudioManager.Instance.ToggleBackgroundMusic(isOn);
    }

    // 효과음 ON/OFF
    public void ToggleEffectSound(bool isOn)
    {
        PlayerPrefs.SetInt("EffectSound", isOn ? 1 : 0);
        PlayerPrefs.Save();

        AudioManager.Instance.ToggleEffectSound(isOn);
    }
}
