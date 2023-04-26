using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerMove player;
    public GameObject gameLabel; //���� ���� UI ������Ʈ ����
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

    public void OpenOptionWindow() //�ɼ� ȭ�� �ѱ�
    {
        gameOption.SetActive(true);
        Time.timeScale = 0;
        gState= GameState.Pause;
    }

    public void CloseOptionWindow() //����ϱ� �ɼ�
    {
        gameOption.SetActive(false);
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
}
