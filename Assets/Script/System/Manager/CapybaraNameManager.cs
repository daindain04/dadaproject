using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class CapybaraNameManager : MonoBehaviour
{
    public TMP_InputField nameInputField;  // 입력창
    
 
    public GameObject NamePanel; // 이름설정창

    private const int MAX_NAME_LENGTH = 10;  // 최대 10글자
    private const string PLAYER_PREFS_KEY = "CapybaraName";  // PlayerPrefs 키

    private void Start()
    {

        NamePanel.SetActive(false);

    

        // 저장된 이름 불러오기
        if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
        {
            string savedName = PlayerPrefs.GetString(PLAYER_PREFS_KEY);
           
            nameInputField.text = savedName;  // 입력창에도 기존 이름 표시
        }
    }

    // 이름설정 창 활성화
    public void OpenNamePanel()
    {
        NamePanel.SetActive(true);
    }

    // 이름설정 창 비활성화
    public void CloseNamePanel()
    {
        NamePanel.SetActive(false);
    }

    public void SetCapybaraName()
    {
        string inputName = nameInputField.text.Trim(); // 공백 제거

        // 한글과 영어만 포함하는지 검사
        if (!IsValidName(inputName))
        {
           
            return;
        }

        // 이름 길이 제한 검사
        if (inputName.Length > MAX_NAME_LENGTH)
        {
            
            return;
        }

       

        // 이름 저장 (PlayerPrefs 사용)
        PlayerPrefs.SetString(PLAYER_PREFS_KEY, inputName);
        PlayerPrefs.Save(); // 저장 실행
    }

    private bool IsValidName(string name)
    {
        // 한글(가-힣) 또는 영어(a-z, A-Z)만 허용
        return Regex.IsMatch(name, "^[가-힣a-zA-Z]+$");
    }
}
