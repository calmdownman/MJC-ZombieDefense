using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public InputField id;
    public InputField password;
    public Text notify; //검사 텍스트 변수

    // Start is called before the first frame update
    void Start()
    {
        notify.text= string.Empty;
    }

    public void SaveUserData()
    {
        if(!CheckInput(id.text, password.text)) return; 

        if(!PlayerPrefs.HasKey(id.text))
        {
            //사용자의 아이디는 키로 패스워드는 값으로 설정
            PlayerPrefs.SetString(id.text, password.text);
            notify.text = "아이디 생성이 완료되었습니다.";
        } else
        {
            notify.text = "이미 존재하는 아이디입니다.";
        }
    }

    public void CheckUserData()
    {
        if (!CheckInput(id.text, password.text)) return;

        string pass = PlayerPrefs.GetString(id.text);
        if(password.text == pass)
        {
            SceneManager.LoadScene(1);
        } else
        {
            notify.text = "입력하신 아이디와 패스워드가 일치하지 않습니다.";
        }
    }

    bool CheckInput(string id, string pwd) 
    { 
        if(id=="" || pwd == "") 
        {
            notify.text = "아이디 또는 패스워드를 입력하세요.";
            return false;
        } else return true;
    }

}
