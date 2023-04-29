using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("#Game Control")]
    public float gameTime;
    public float maxGameTime = 20f;
    public Spawner spawner;

    [Header("#Player Info")]
    public int level;
    public int kill;
    public int exp;
    public int bombEA;
    public int[] nextExp = {2, 3, 5, 7, 8, 8 ,8 ,8};

    public PlayerMove player;
    public GameObject gameLabel; //게임 상태 UI 오브젝트 변수
    public GameObject scopeObject;
    public LevelUp uiLevelUp;
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

        int[] indexs = RandomNumerics(spawner.spawnPoints.Length-1, nextExp[level]);

        for (int i = 0; i < nextExp[level]; i++)
        {
            spawner.Spawn(indexs[i]);
        }
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
        if (gState != GameState.Run)
        {
            return;
        }
        else
        {
            gameTime += Time.deltaTime;

            if (gameTime > maxGameTime)
            {
                gameTime = maxGameTime;
                GameVictory();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OpenOptionWindow();
        }

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

    public void GetExp()
    {
        if (gState != GameState.Run) return;

        exp++;

        if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
            
            int[] indexs = RandomNumerics(spawner.spawnPoints.Length - 1, nextExp[level]);

            for (int i = 0; i < nextExp[level]; i++)
            {
                spawner.Spawn(indexs[i]);
            }
        }
    }
    public void OpenLevelUpWindow() //레벨업 화면 켜기
    {
        uiLevelUp.gameObject.SetActive(true);
        CursorSolve();
        Time.timeScale = 0;
        gState = GameState.Pause;
    }

    public void CloseLevelUpWindow() //레벨업 화면 끄기
    {
        uiLevelUp.gameObject.SetActive(false);
        CursorLock();
        Time.timeScale = 1f;
        gState = GameState.Run;
    }

    public void GameVictory()
    {
        //이동->대기 애니메이션 실행
        player.GetComponentInChildren<Animator>().SetFloat("MoveMotion", 0f);
        gameLabel.SetActive(true); //상태 텍스트 활성화
        gameText.text = "Game Victory"; //상태 텍스트를 Game Victory로
        gameText.color = new Color32(255, 255,100,255); //초록색으로                                            
        Transform buttons = gameText.transform.GetChild(0);//상태 텍스트의 자식 오브젝트의 트랜스폼 정보를 얻어옴

        buttons.gameObject.SetActive(true); //버튼 오브젝트를 활성화
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gState = GameState.GameOver;
    }
    public void OpenOptionWindow() //옵션 화면 켜기
    {
        gameOption.SetActive(true);
        CursorSolve();
        Time.timeScale = 0;
        gState= GameState.Pause;
    }

    public void CloseOptionWindow() //계속하기 옵션
    {
        gameOption.SetActive(false);
        CursorLock();
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
    void CursorLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void CursorSolve()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static int[] RandomNumerics(int maxCount, int n)
    {
        //0~maxCount까지의 숫자 중 겹치지 않는 n개의 난수가 필요할 때 사용
        int[] defaults = new int[maxCount]; //0~maxCount까지 순서대로 저장하는 배열
        int[] results = new int[n]; //결과 값들을 저장하는 배열

        //배열 전체에 0부터 maxCount의 값을 순서대로 저장
        for (int i = 0; i < maxCount; ++i)
        {
            defaults[i] = i;
        }

        for (int i = 0; i < n; ++i)
        {
            int index = Random.Range(0, maxCount); //임의의 숫자를 하나 뽑아서

            results[i] = defaults[index];
            defaults[index] = defaults[maxCount - 1];

            maxCount--;
        }
        return results;
    }
}
