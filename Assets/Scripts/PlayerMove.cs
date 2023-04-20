using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 7f; //�̵� �ӵ� ����
    CharacterController cc; // ĳ������Ʈ�ѷ� ����
    float gravity = -20f; //�߷� ����
    float yVelocity = 0; //���� �ӷ� ����
    public float jumpPower = 10f;
    public bool isJumping = false; //���� ���º���

    public int hp = 20; //�÷��̾� ü�� ����
    int maxHP = 20; //�÷��̾��� �ִ� ü��
    public Slider hpSlider; //hp �����̴� ����

    public GameObject hitEffect; //���񿡰� ���� ��
    Animator anim;

    void Start()
    {
       //ĳ���� ��Ʈ�ѷ� ������Ʈ �޾ƿ���
       cc = GetComponent<CharacterController>();
       //Player�� �ڽ� ������Ʈ�� �ִ� �ִϸ����� ������Ʈ �Ҵ�
       anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //���� ���°� ���� �� ���°� �ƴ϶�� Update()�� ���� ���� ����� �Է� ���ް���
        if (GameManager.Instance.gState != GameManager.GameState.Run)
        {
            return;
        }

        //����� �Է��� �޴´�.
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        //�̵� ���� ����
        Vector3 dir = new Vector3(h,0,v);
        dir = dir.normalized;
        anim.SetFloat("MoveMotion", dir.magnitude);
        //���� ī�޶� �������� ������ ��ȯ�Ѵ�
        dir = Camera.main.transform.TransformDirection(dir);
        //�̵� �ӵ��� ���� �̵��Ѵ�
        //transform.position += dir * moveSpeed * Time.deltaTime;

        //���� �ٴڿ� ����ִٸ�
        if(cc.collisionFlags == CollisionFlags.Below)
        {
            if (isJumping) //���� ���� ���̶��
            {
                isJumping = false; //������ ���·� �ʱ�ȭ
            }
            yVelocity = 0;
        }

        //���� Ű���� �����̽��ٸ� ������, �������� ���� ���¶��
        if (Input.GetButtonDown("Jump") && !isJumping) //���� Ű���� SpaceBar�� �����ٸ�
        {
            yVelocity = jumpPower; //���� �ӵ��� �������� ����
            isJumping = true;
        }

        yVelocity += gravity * Time.deltaTime; //ĳ���� �����ӵ��� �߷� ���� ����
        //Debug.Log(yVelocity.ToString());
        dir.y = yVelocity; //�߷°��� ������ �����ӵ� �Ҵ�
        //�̵� �ӵ��� ���� �̵�
        cc.Move(dir*moveSpeed*Time.deltaTime);

        hpSlider.value = (float)hp/(float)maxHP;
    }
    public void DamageAction(int damage)
    {
        hp -= damage; //���ʹ��� ���ݷ� ��ŭ �÷��̾� ü���� ����
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