using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingNextScene : MonoBehaviour
{
    public int sceneNumber = 2; //진행할 씬 번호
    public Slider loadingBar;
    public Text loadingText;

    void Start()
    {
        StartCoroutine(TransitionNextScene(sceneNumber));
    }

    IEnumerator TransitionNextScene(int num)
    {
        //지정된 씬을 비동기 형식으로 로드한다.
        AsyncOperation ao = SceneManager.LoadSceneAsync(num);
        //로드되는 씬의 모습이 화면에 보이지 않게 한다.
        ao.allowSceneActivation= false;
        while(!ao.isDone) //로딩이 완료될 때까지 진행과정을 표시
        {
            loadingBar.value = ao.progress;
            loadingText.text =
                (ao.progress*100f).ToString() + "%";
            if(ao.progress >= 0.9f)
            {
                //로드된 씬을 화면에 보이게 함
                ao.allowSceneActivation= true;
            }
            yield return null; //다음 프레임이 될 때까지 기다림
        }
    }
}
