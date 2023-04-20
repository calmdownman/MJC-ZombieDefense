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
    EnemyState m_State; //���ʹ� ���º���

    public float findDistance = 8f; //�÷��̾� �߰� ����
    Transform player; //�÷��̾� Ʈ������
    public float attackDistance = 2f; //���� ����
    //public float moveSpeed = 5f;
    CharacterController cc; //ĳ���� ��Ʈ�ѷ� ������Ʈ

    float currentTime = 0; //���� ���� �ð�
    float attackDelay = 2f; //���� ������ �ð�
    public int attackPower = 3;

    Vector3 originPos; //���ʹ��� �ʱ���ġ
    Quaternion oringRot; //���ʹ��� �ʱ�ȸ��
    public float moveDistance = 20f; //�̵� ���� ����
    [SerializeField]
    int hp = 15; //������ ���� ü��
    [SerializeField]
    int maxHp = 15; //������ �ִ� ü��
    public Slider hpSlider;

    Animator anim;
    NavMeshAgent smith; //������̼� ������Ʈ ����

    // Start is called before the first frame update
    void Start()
    {
        m_State= EnemyState.Idle;
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        originPos = transform.position;
        oringRot = transform.rotation;
        //�ڽ� ������Ʈ�� �ִ��͸����� ������Ʈ �޾ƿ���
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

    void Idle() //��� ���� �Լ�, �÷��̾ 8���� ������ �������� �˻�
    {
        if(Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State= EnemyState.Move;
            print("���� ��ȯ: Idle->Move");
            anim.SetTrigger("IdleToMove");
        }
    }
    void Move()
    {
         if (Vector3.Distance(transform.position, originPos) > moveDistance)
        {
            m_State = EnemyState.Return;
            print("���� ��ȯ:Move -> Return");
        }
        //�÷��̾ ���ݹ��� ���̶�� �÷��̾ ���� �̵�
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
           /* Vector3 dir = (player.position - transform.position).normalized; //�̵� ���� ����
            cc.Move(dir * moveSpeed * Time.deltaTime); //�̵�
            transform.forward = dir; //�÷��̾ ���� ���� ��ȯ*/

            smith.isStopped= true; //����޽� ������Ʈ�� �̵��� ����
            smith.ResetPath(); //��� �ʱ�ȭ
            //������̼��� �������� �ּ� �Ÿ��� ����. 2m ������ �۵���
            smith.stoppingDistance= attackDistance;
            //������̼� �������� �÷��̾� ��ġ�� �̵�
            smith.destination = player.position;
        }
        else //2���� �̳���� ���� ���¸� �������� ��ȯ
        {
            m_State= EnemyState.Attack;
            print("���� ��ȯ:Move->Attack");
            currentTime= attackDelay; //�����ð��� �̸� 2�ʷ� �����Ͽ� ���� �� �ٷ� ����
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
                print("����");
                currentTime = 0;
                anim.SetTrigger("StartAttack");
            }
        }
        else //���ݹ��� ���̶�� ���߰� �ǽ�
        {
            m_State = EnemyState.Move;
            print("������ȯ: Attack -> Move");
            currentTime= 0;
            anim.SetTrigger("AttackToMove");
        }
    }

    public void AttackAction()
    {
        //�÷��̾��� ������ �Լ� ����
        player.GetComponent<PlayerMove>().DamageAction(attackPower);
    }

    void Return()
    {
        //�ʱ���ġ���� �Ÿ��� 0.1f �̻��̶�� �ʱ� ��ġ ������ ����
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
            print("���� ��ȯ: Return->Idle");
            anim.SetTrigger("MoveToIdle"); //��� �ִϸ��̼� ����
        }
    }

    public void HitEnemy(int hitPower) //���� �¾��� �� ȣ��Ǵ� �Լ�
    {
        //�ǰ�, ���, ���� ���¶�� �ƹ��� ó���� ���� �ʴ´�
        if(m_State== EnemyState.Damaged || m_State == EnemyState.Die || 
           m_State == EnemyState.Return) {return;}
          

        hp -= hitPower;

        smith.isStopped= true;
        smith.ResetPath();

        if (hp > 0) //�ѿ� �¾��� �� ���� ü���� 0���� ũ��
        {
            m_State = EnemyState.Damaged;
            print("���� ��ȯ: Any State -> Damaged");
            anim.SetTrigger("Damaged");
            Damaged();
        }
        else //0���� �۴ٸ�
        {
            m_State = EnemyState.Die;
            print("���� ��ȯ: Any State -> Die");
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
        //�ǰ� �ִϸ��̼� ��� �ð���ŭ ���
        yield return new WaitForSeconds(1f);
        m_State = EnemyState.Move;
        print("���� ��ȯ:Damaged -> Move");
    }

    void Die()
    {
        StopAllCoroutines(); //������ ����ǰ� �ִ� �ڷ�ƾ�Լ��� ��� ����
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        //������Ʈ�� SetActive()�� ��/Ȱ��ȭ, ������Ʈ enabled�� ��/Ȱ��ȭ
        cc.enabled = false; //���� ĳ������Ʈ�ѷ� ��Ȱ��ȭ
        yield return new WaitForSeconds(2f);
        print("�Ҹ�");
        Destroy(gameObject);
    }
}
