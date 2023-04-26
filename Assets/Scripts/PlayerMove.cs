using System.Collections;

using UnityEngine;
using UnityEngine.UI;


public class PlayerMove : MonoBehaviour
{
    [Header("Status")]
    public float moveSpeed = 7f; //이동 속도 변수
    public float jumpPower = 10f;
    public bool isJumping = false; //점프 상태변수
    public int hp = 20; //플레이어 체력 변수
    int maxHP = 20; //플레이어의 최대 체력

    [Header("Audio Clip")]
    [SerializeField]
    private AudioClip acWalk;

    CharacterController cc; // 캐릭터컨트롤러 변수
    float gravity = -20f; //중력 변수
    float yVelocity = 0; //수직 속력 변수
    public Slider hpSlider; //hp 슬라이더 변수

    public GameObject hitEffect; //좀비에게 맞을 때
    Animator anim;
    private AudioSource audioSource; //사운드 재생 제어

    void Start()
    {
       hp = maxHP; //시작 시 현재 체력 최대 체력으로 초기화
       //캐릭터 컨트롤러 컴포넌트 받아오기
       cc = GetComponent<CharacterController>();
       //Player의 자식 오브젝트에 있는 애니메이터 컴포넌트 할당
       anim = GetComponentInChildren<Animator>();
       audioSource = GetComponent<AudioSource>();
       audioSource.clip = acWalk;
    }

    // Update is called once per frame
    void Update()
    {
        //게임 상태가 게임 중 상태가 아니라면 Update()를 종료 시켜 사용자 입력 못받게함
        if (GameManager.Instance.gState != GameManager.GameState.Run )
        {
            return;
        }

        //사용자 입력을 받는다.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        if( h != 0 || v != 0) 
        {
            if (audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        } else
        { //멈췄을 때 사운드가 재생중이면 정지
            if (audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }

        //이동 방향 설정
        Vector3 dir = new Vector3(h,0,v);
        dir = dir.normalized;
        anim.SetFloat("MoveMotion", dir.magnitude);
        //메인 카메라를 기준으로 방향을 변환한다
        dir = Camera.main.transform.TransformDirection(dir);
        //이동 속도에 맞춰 이동한다
        //transform.position += dir * moveSpeed * Time.deltaTime;

        //만일 바닥에 닿아있다면
        if(cc.collisionFlags == CollisionFlags.Below)
        {
            if (isJumping) //만일 점핑 중이라면
            {
                isJumping = false; //점프전 상태로 초기화
            }
            yVelocity = 0;
        }

        //만일 키보드 스페이스바를 눌렀고, 점프하지 않은 상태라면
        if (Input.GetButtonDown("Jump") && !isJumping) //만일 키보드 SpaceBar를 눌렀다면
        {
            yVelocity = jumpPower; //수직 속도에 점프력을 적용
            isJumping = true;
        }

        yVelocity += gravity * Time.deltaTime; //캐릭터 수직속도에 중력 값을 적용
        //Debug.Log(yVelocity.ToString());
        dir.y = yVelocity; //중력값을 적용한 수직속도 할당
        //이동 속도에 맞춰 이동
        cc.Move(dir*moveSpeed*Time.deltaTime);

        hpSlider.value = (float)hp/(float)maxHP;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("EnemyBullet")) return;

        DamageAction(other.gameObject.GetComponent<Missile>().damage);
        Destroy(other.gameObject);
    }

    public void DamageAction(int damage)
    {
        hp -= damage; //에너미의 공격력 만큼 플레이어 체력을 감소
        if (hp > 0)
        {
            StartCoroutine(PlayHitEffect());
        }
      
    }

    IEnumerator PlayHitEffect()
    {
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        hitEffect.SetActive(false);
    }
}
