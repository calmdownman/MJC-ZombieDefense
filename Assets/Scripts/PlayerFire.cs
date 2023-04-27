using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    public Text wModeText;
    private AudioSource audioSource; //사운드 재생 제어

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
    private WeaponSetting weaponSetting; //무기 설정
    [SerializeField]
    private AudioClip audioClipFire;

    private float lastAttackTime = 0; //마지막 발사시간 체크용

    private CamRotate recoil_Script;

    enum WeaponMode
    {
        Normal,
        Sniper,
        Rifle
    }
    WeaponMode wMode;
    bool ZoomMode = false; //카메라 확대 확인용 변수
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ps = bulletEffect.GetComponent<ParticleSystem>();
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        recoil_Script = Camera.main.transform.GetComponent<CamRotate>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gState != GameManager.GameState.Run)
        {
            return;
        }

        UpdateWeaponAction();

        if (Input.GetMouseButtonDown(1))
        {
            switch (wMode)
            {
                case WeaponMode.Rifle:
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponSetting.isAutomaticAttack = false;
            wMode = WeaponMode.Normal;
            Camera.main.fieldOfView = 60f;
            wModeText.text = "Normal Mode";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponSetting.isAutomaticAttack = false;
            wMode = WeaponMode.Sniper;
            wModeText.text = "Sniper Mode";
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            wMode = WeaponMode.Rifle;
            wModeText.text = "Rifle Mode";
            weaponSetting.isAutomaticAttack = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
           anim.SetTrigger("Reload");
        }

        

    }

    private void UpdateWeaponAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartWeaponAction();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopWeaponAction();
        }
    }

        public void StartWeaponAction(int type = 0) {
        if (type == 0)
        {
            //연속 공격
            if(weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            } 
            //단발 공격
            else
            {
                OnAttack();
            }
        }
    }

    public void StopWeaponAction(int type = 0)
    {
        if (type == 0)
        {
            //마우스 왼쪽 클릭 (공격 종료)
            if (weaponSetting.isAutomaticAttack == true)
            {
                StopCoroutine("OnAttackLoop");
            }
        }
    }

    IEnumerator OnAttackLoop()
    {
        while (true)
        {
            OnAttack();
            yield return null;
        }
    }

    private void OnAttack() {
        if (Time.time - lastAttackTime > weaponSetting.attackRate) { 
            //뛰고있을 때는 공격할 수 없다
            if(anim.GetFloat("MoveMotion") > 0)
            {
                return;
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

            //공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
            lastAttackTime = Time.time;
            //무기 애니메이션 재생
            anim.Play("Fire", -1, 0);
            //공격 사운드 재생
            audioSource.Play();

            recoil_Script.RecoilFire();
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
