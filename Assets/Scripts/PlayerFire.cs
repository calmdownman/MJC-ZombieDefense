using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    public Text wModeText;

    [Header("Bomb")]
    public GameObject firePosition; //발사 위치
    public GameObject bombFactory; //투척 무기 오브젝트
    public float throwPower = 15f;
    public int rotatePower = 10;

    public GameObject bulletEffect; //피격 이펙트 오브젝트
    ParticleSystem ps; //피격 이펙트 파티클 시스템
    public int weaponPower = 5;

    Animator anim;
    public GameObject[] eff_Flash; //총 발사 효과 오브젝트 배열

    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting;

    private float lastAttackTime = 0;

    enum WeaponMode
    {
        Normal,
        Sniper
    }
    WeaponMode wMode;
    bool ZoomMode = false; //카메라 확대 확인용 변수
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ps = bulletEffect.GetComponent<ParticleSystem>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gState != GameManager.GameState.Run)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            switch (wMode)
            {
                case WeaponMode.Normal:
                    //수류탄을 생성한 후 수류탄 생성 위치를 firePosition으로 한다
                    GameObject bomb = Instantiate(bombFactory);
                    bomb.transform.position = firePosition.transform.position;

                    //수류탄 오브젝트의 리지드바디 정보를 얻어옴
                    Rigidbody rb = bomb.GetComponent<Rigidbody>();
                    //AddForce를 이용해 수류탄 이동
                    rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
                    rb.AddTorque(Vector3.back * rotatePower, ForceMode.Impulse); //수류탄 던져질 때 회전
                    break;

                case WeaponMode.Sniper:
                    if (!ZoomMode)
                    {
                        Camera.main.fieldOfView = 15f;
                        ZoomMode = true;
                    }
                    else //줌 모드라면 카메라 확대를 원래 상태로
                    {
                        Camera.main.fieldOfView = 60f;
                        ZoomMode = false;
                    }
                    break;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (anim.GetFloat("MoveMotion") == 0)
            {
                anim.SetTrigger("Attack");
            }
            //레이를 생성한 후 발사될 위치와 진행 방향을 설정
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitInfo = new RaycastHit(); //레이와 부딪힌 상대방의 정보를 저장할 구조체

            if (Physics.Raycast(ray, out hitInfo))
            {
                //레이가 부딪히 오브젝트가 Enemy라면
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyFSM eFSM = hitInfo.transform.GetComponent<EnemyFSM>();
                    eFSM.HitEnemy(weaponPower);
                }
                else
                {
                    //피격 이펙트의 위치를 레이와 부딪힌 지점으로 이동
                    bulletEffect.transform.position = hitInfo.point;
                    //피격 이펙트의 forward 방향을 레이가 부딪힌 지점의 법선 벡터와 일치시킨다
                    bulletEffect.transform.forward = hitInfo.normal;
                    ps.Play(); //피격 이펙트 플레이
                }
            }
            StartCoroutine(ShootEffectOn(0.05f)); //총 이펙트 실시
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            wMode = WeaponMode.Normal;
            Camera.main.fieldOfView = 60f;
            wModeText.text = "Normal Mode";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            wMode = WeaponMode.Sniper;
            wModeText.text = "Sniper Mode";
        }
    }

    public void StartWeaponAction(int type = 0) {
        if (type == 0)
        {
            //연속 공격
            if(weaponSetting.isAutomaticAttack==true)
            {
                
            }
        }
    }


    IEnumerator ShootEffectOn(float duration)
    {
        int num = Random.Range(0, eff_Flash.Length);
        eff_Flash[num].SetActive(true);
        yield return new WaitForSeconds(duration);
        eff_Flash[num].SetActive(false);
    }
}
