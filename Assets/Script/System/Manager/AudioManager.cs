using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource backgroundMusicSource;
    public AudioSource effectSoundSource;

    void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 저장된 사운드 설정 불러오기
        bool isBackgroundMusicOn = PlayerPrefs.GetInt("BackgroundMusic", 1) == 1;
        bool isEffectSoundOn = PlayerPrefs.GetInt("EffectSound", 1) == 1;

        ToggleBackgroundMusic(isBackgroundMusicOn);
        ToggleEffectSound(isEffectSoundOn);
    }

    public void ToggleBackgroundMusic(bool isOn)
    {
        backgroundMusicSource.mute = !isOn;
    }

    public void ToggleEffectSound(bool isOn)
    {
        effectSoundSource.mute = !isOn;
    }

    public void PlayEffectSound(AudioClip clip)
    {
        if (!effectSoundSource.mute)
        {
            effectSoundSource.PlayOneShot(clip);
        }
    }
}
