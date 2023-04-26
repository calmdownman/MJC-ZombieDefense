using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerMove player;
    public GameObject gameLabel; //게임 상태 UI 오브젝트 변수
    Text gameText; //게임 상태 UI텍스트 컴포넌트 변수

    public enum GameState
    {
        Ready,
        Run,
        Pause,
        GameOver,
    }

    public GameState gState; //게임 상태 상수
    public GameObject gameOption; //옵션 화면 UI 오브젝트 변수

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        gState = GameState.Ready; //초기 게임상태는 준비
        gameText = gameLabel.GetComponent<Text>();
        gameText.text = gState.ToString();
        gameText.color = new Color32(255,185,0,255);
        StartCoroutine(ReadyToStart());
    }

    IEnumerator ReadyToStart() 
    {
        yield return new WaitForSeconds(2f);
        gameText.text = "Go!"; //상태 텍스트 변경
        yield return new WaitForSeconds(0.5f);
        gameLabel.SetActive(false);
        gState = GameState.Run;

    }

    // Update is called once per frame
    void Update()
    {
        if(player.hp <= 0) //플레이어가 죽었다면
        {
            //이동->대기 애니메이션 실행
            player.GetComponentInChildren<Animator>().SetFloat("MoveMotion", 0f);
            gameLabel.SetActive(true); //상태 텍스트 활성화
            gameText.text = "Game Over"; //상태 텍스트를 Game Over로
            gameText.color = new Color32(255,0,0,255); //붉은색으로
            //상태 텍스트의 자식 오브젝트의 트랜스폼 정보를 얻어옴
            Transform buttons = gameText.transform.GetChild(0);
            //버튼 오브젝트를 활성화
            buttons.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gState = GameState.GameOver;
        }
    }

    public void OpenOptionWindow() //옵션 화면 켜기
    {
        gameOption.SetActive(true);
        Time.timeScale = 0;
        gState= GameState.Pause;
    }

    public void CloseOptionWindow() //계속하기 옵션
    {
        gameOption.SetActive(false);
        Time.timeScale = 1f;
        gState = GameState.Run;
    }

    public void RestartGame() //다시하기 옵션
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame() //게임 종료 옵션
    {
        Application.Quit();
    }
}
