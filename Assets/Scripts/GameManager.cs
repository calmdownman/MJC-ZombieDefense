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
    public GameObject gameLabel; //���� ���� UI ������Ʈ ����
    public GameObject scopeObject;
    public LevelUp uiLevelUp;
    Text gameText; //���� ���� UI�ؽ�Ʈ ������Ʈ ����

    public enum GameState
    {
        Ready,
        Run,
        Pause,
        GameOver,
    }

    public GameState gState; //���� ���� ���
    public GameObject gameOption; //�ɼ� ȭ�� UI ������Ʈ ����

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        gState = GameState.Ready; //�ʱ� ���ӻ��´� �غ�
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
        gameText.text = "Go!"; //���� �ؽ�Ʈ ����
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

        if(player.hp <= 0) //�÷��̾ �׾��ٸ�
        {
            //�̵�->��� �ִϸ��̼� ����
            player.GetComponentInChildren<Animator>().SetFloat("MoveMotion", 0f);
            gameLabel.SetActive(true); //���� �ؽ�Ʈ Ȱ��ȭ
            gameText.text = "Game Over"; //���� �ؽ�Ʈ�� Game Over��
            gameText.color = new Color32(255,0,0,255); //����������
            //���� �ؽ�Ʈ�� �ڽ� ������Ʈ�� Ʈ������ ������ ����
            Transform buttons = gameText.transform.GetChild(0);
            //��ư ������Ʈ�� Ȱ��ȭ
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
    public void OpenLevelUpWindow() //������ ȭ�� �ѱ�
    {
        uiLevelUp.gameObject.SetActive(true);
        CursorSolve();
        Time.timeScale = 0;
        gState = GameState.Pause;
    }

    public void CloseLevelUpWindow() //������ ȭ�� ����
    {
        uiLevelUp.gameObject.SetActive(false);
        CursorLock();
        Time.timeScale = 1f;
        gState = GameState.Run;
    }

    public void GameVictory()
    {
        //�̵�->��� �ִϸ��̼� ����
        player.GetComponentInChildren<Animator>().SetFloat("MoveMotion", 0f);
        gameLabel.SetActive(true); //���� �ؽ�Ʈ Ȱ��ȭ
        gameText.text = "Game Victory"; //���� �ؽ�Ʈ�� Game Victory��
        gameText.color = new Color32(255, 255,100,255); //�ʷϻ�����                                            
        Transform buttons = gameText.transform.GetChild(0);//���� �ؽ�Ʈ�� �ڽ� ������Ʈ�� Ʈ������ ������ ����

        buttons.gameObject.SetActive(true); //��ư ������Ʈ�� Ȱ��ȭ
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gState = GameState.GameOver;
    }
    public void OpenOptionWindow() //�ɼ� ȭ�� �ѱ�
    {
        gameOption.SetActive(true);
        CursorSolve();
        Time.timeScale = 0;
        gState= GameState.Pause;
    }

    public void CloseOptionWindow() //����ϱ� �ɼ�
    {
        gameOption.SetActive(false);
        CursorLock();
        Time.timeScale = 1f;
        gState = GameState.Run;
    }

    public void RestartGame() //�ٽ��ϱ� �ɼ�
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame() //���� ���� �ɼ�
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
        //0~maxCount������ ���� �� ��ġ�� �ʴ� n���� ������ �ʿ��� �� ���
        int[] defaults = new int[maxCount]; //0~maxCount���� ������� �����ϴ� �迭
        int[] results = new int[n]; //��� ������ �����ϴ� �迭

        //�迭 ��ü�� 0���� maxCount�� ���� ������� ����
        for (int i = 0; i < maxCount; ++i)
        {
            defaults[i] = i;
        }

        for (int i = 0; i < n; ++i)
        {
            int index = Random.Range(0, maxCount); //������ ���ڸ� �ϳ� �̾Ƽ�

            results[i] = defaults[index];
            defaults[index] = defaults[maxCount - 1];

            maxCount--;
        }
        return results;
    }
}
