using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public enum Type {A,B,C};
    public Type enemyType;
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    EnemyState m_State; //에너미 상태변수

    public float findDistance = 8f; //플레이어 발견 범위
    Transform player; //플레이어 트랜스폼
    public float attackDistance = 2f; //공격 범위
    //public float moveSpeed = 5f;
    CharacterController cc; //캐릭터 컨트롤러 컴포넌트

    float currentTime = 0; //공격 누적 시간
    float attackDelay = 2f; //공격 딜레이 시간
    public int attackPower = 3;

    Vector3 originPos; //에너미의 초기위치
    Quaternion oringRot; //에너미의 초기회전
    public float moveDistance = 20f; //이동 가능 범위
    [SerializeField]
    int hp = 15; //좀비의 현재 체력
    [SerializeField]
    int maxHp = 15; //좀비의 최대 체력
    public Slider hpSlider;

    Animator anim;
    NavMeshAgent smith; //내비게이션 에이전트 변수

    // Start is called before the first frame update
    void Start()
    {
        m_State= EnemyState.Idle;
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        originPos = transform.position;
        oringRot = transform.rotation;
        //자식 오브젝트의 애니터메이터 컴포넌트 받아오기
        anim = transform.GetComponentInChildren<Animator>();
        smith= GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case EnemyState.Idle: Idle(); break;
            case EnemyState.Move: Move(); break;
            case EnemyState.Attack: Attack(); break;
            case EnemyState.Return: Return(); break;
            //case EnemyState.Damaged: Damaged(); break;
            //case EnemyState.Die: Die(); break;
        }
        hpSlider.value = (float)hp / (float)maxHp;
    }

    void Idle() //대기 상태 함수, 플레이어가 8미터 안으로 들어오는지 검사
    {
        if(Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State= EnemyState.Move;
            print("상태 전환: Idle->Move");
            anim.SetTrigger("IdleToMove");
        }
    }
    void Move()
    {
         if (Vector3.Distance(transform.position, originPos) > moveDistance)
        {
            m_State = EnemyState.Return;
            print("상태 전환:Move -> Return");
        }
        //플레이어가 공격범위 밖이라면 플레이어를 향해 이동
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
           /* Vector3 dir = (player.position - transform.position).normalized; //이동 방향 설정
            cc.Move(dir * moveSpeed * Time.deltaTime); //이동
            transform.forward = dir; //플레이어를 향해 방향 전환*/

            smith.isStopped= true; //내비메쉬 에이전트의 이동을 멈춤
            smith.ResetPath(); //경로 초기화
            //내비게이션의 목적지를 최소 거리로 설정. 2m 까지만 작동함
            smith.stoppingDistance= attackDistance;
            //내비게이션 목적지를 플레이어 위치로 이동
            smith.destination = player.position;
        }
        else //2미터 이내라면 현재 상태를 공격으로 전환
        {
            m_State= EnemyState.Attack;
            print("상태 전환:Move->Attack");
            currentTime= attackDelay; //누적시간을 미리 2초로 변경하여 접근 시 바로 공격
            anim.SetTrigger("MoveToAttackDelay");
        }
    }

    void Attack()
    {
        if(Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                print("공격");
                currentTime = 0;
                anim.SetTrigger("StartAttack");
            }
        }
        else //공격범위 밖이라면 재추격 실시
        {
            m_State = EnemyState.Move;
            print("상태전환: Attack -> Move");
            currentTime= 0;
            anim.SetTrigger("AttackToMove");
        }
    }

    public void AttackAction()
    {
        //플레이어의 데미지 함수 실행
        player.GetComponent<PlayerMove>().DamageAction(attackPower);
    }

    void Return()
    {
        //초기위치에서 거리가 0.1f 이상이라면 초기 위치 쪽으로 복귀
        if (Vector3.Distance(transform.position, originPos) > 0.1f)
        {
           /* Vector3 dir = (originPos- transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);
            transform.forward = dir;*/
            smith.destination = originPos;
            smith.stoppingDistance= 0;
        }
        else
        {
            smith.isStopped= true;
            smith.ResetPath();

            transform.position= originPos;
            transform.rotation = oringRot;
            m_State= EnemyState.Idle;
            hp = maxHp;
            print("상태 전환: Return->Idle");
            anim.SetTrigger("MoveToIdle"); //대기 애니메이션 실행
        }
    }

    public void HitEnemy(int hitPower) //좀비가 맞았을 때 호출되는 함수
    {
        //피격, 사망, 복귀 상태라면 아무런 처리를 하지 않는다
        if(m_State== EnemyState.Damaged || m_State == EnemyState.Die || 
           m_State == EnemyState.Return) {return;}
          

        hp -= hitPower;

        smith.isStopped= true;
        smith.ResetPath();

        if (hp > 0) //총에 맞았을 때 좀비 체력이 0보다 크면
        {
            m_State = EnemyState.Damaged;
            print("상태 전환: Any State -> Damaged");
            anim.SetTrigger("Damaged");
            Damaged();
        }
        else //0보다 작다면
        {
            m_State = EnemyState.Die;
            print("상태 전환: Any State -> Die");
            anim.SetTrigger("Die");
            Die();
        }
    }

    void Damaged()
    {
        StartCoroutine(DamageProcess());
    }

    IEnumerator DamageProcess()
    {
        //피격 애니메이션 재생 시간만큼 대기
        yield return new WaitForSeconds(1f);
        m_State = EnemyState.Move;
        print("상태 전환:Damaged -> Move");
    }

    void Die()
    {
        StopAllCoroutines(); //기존에 실행되고 있던 코루틴함수들 모두 종료
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        //오브젝트는 SetActive()로 비/활성화, 컴포넌트 enabled로 비/활성화
        cc.enabled = false; //좀비 캐릭터컨트롤러 비활성화
        yield return new WaitForSeconds(2f);
        print("소멸");
        Destroy(gameObject);
    }
}
