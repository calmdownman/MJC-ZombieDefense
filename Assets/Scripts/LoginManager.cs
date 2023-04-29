using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public InputField id;
    public InputField password;
    public Text notify; //�˻� �ؽ�Ʈ ����

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
            //������� ���̵�� Ű�� �н������ ������ ����
            PlayerPrefs.SetString(id.text, password.text);
            notify.text = "���̵� ������ �Ϸ�Ǿ����ϴ�.";
        } else
        {
            notify.text = "�̹� �����ϴ� ���̵��Դϴ�.";
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
            notify.text = "�Է��Ͻ� ���̵�� �н����尡 ��ġ���� �ʽ��ϴ�.";
        }
    }

    bool CheckInput(string id, string pwd) 
    { 
        if(id=="" || pwd == "") 
        {
            notify.text = "���̵� �Ǵ� �н����带 �Է��ϼ���.";
            return false;
        } else return true;
    }

}
