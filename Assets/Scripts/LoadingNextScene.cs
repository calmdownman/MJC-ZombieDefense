using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingNextScene : MonoBehaviour
{
    public int sceneNumber = 2; //������ �� ��ȣ
    public Slider loadingBar;
    public Text loadingText;

    void Start()
    {
        StartCoroutine(TransitionNextScene(sceneNumber));
    }

    IEnumerator TransitionNextScene(int num)
    {
        //������ ���� �񵿱� �������� �ε��Ѵ�.
        AsyncOperation ao = SceneManager.LoadSceneAsync(num);
        //�ε�Ǵ� ���� ����� ȭ�鿡 ������ �ʰ� �Ѵ�.
        ao.allowSceneActivation= false;
        while(!ao.isDone) //�ε��� �Ϸ�� ������ ��������� ǥ��
        {
            loadingBar.value = ao.progress;
            loadingText.text =
                (ao.progress*100f).ToString() + "%";
            if(ao.progress >= 0.9f)
            {
                //�ε�� ���� ȭ�鿡 ���̰� ��
                ao.allowSceneActivation= true;
            }
            yield return null; //���� �������� �� ������ ��ٸ�
        }
    }
}
