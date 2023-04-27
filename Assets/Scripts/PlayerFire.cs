using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    public Text wModeText;
    private AudioSource audioSource; //���� ��� ����

    [Header("Bomb")]
    public GameObject firePosition; //�߻� ��ġ
    public GameObject bombFactory; //��ô ���� ������Ʈ
    public float throwPower = 15f;
    public int rotatePower = 10;

    public GameObject bulletEffect; //�ǰ� ����Ʈ ������Ʈ
    ParticleSystem ps; //�ǰ� ����Ʈ ��ƼŬ �ý���
    public int weaponPower = 5;

    Animator anim;
    public GameObject[] eff_Flash; //�� �߻� ȿ�� ������Ʈ �迭

    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting; //���� ����
    [SerializeField]
    private AudioClip audioClipFire;

    private float lastAttackTime = 0; //������ �߻�ð� üũ��

    private CamRotate recoil_Script;

    enum WeaponMode
    {
        Normal,
        Sniper,
        Rifle
    }
    WeaponMode wMode;
    bool ZoomMode = false; //ī�޶� Ȯ�� Ȯ�ο� ����
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
                    //����ź�� ������ �� ����ź ���� ��ġ�� firePosition���� �Ѵ�
                    GameObject bomb = Instantiate(bombFactory);
                    bomb.transform.position = firePosition.transform.position;

                    //����ź ������Ʈ�� ������ٵ� ������ ����
                    Rigidbody rb = bomb.GetComponent<Rigidbody>();
                    //AddForce�� �̿��� ����ź �̵�
                    rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
                    rb.AddTorque(Vector3.back * rotatePower, ForceMode.Impulse); //����ź ������ �� ȸ��
                    break;

                case WeaponMode.Sniper:
                    if (!ZoomMode)
                    {
                        Camera.main.fieldOfView = 15f;
                        ZoomMode = true;
                    }
                    else //�� ����� ī�޶� Ȯ�븦 ���� ���·�
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
            //���� ����
            if(weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            } 
            //�ܹ� ����
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
            //���콺 ���� Ŭ�� (���� ����)
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
            //�ٰ����� ���� ������ �� ����
            if(anim.GetFloat("MoveMotion") > 0)
            {
                return;
            }

            //���̸� ������ �� �߻�� ��ġ�� ���� ������ ����
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitInfo = new RaycastHit(); //���̿� �ε��� ������ ������ ������ ����ü

            if (Physics.Raycast(ray, out hitInfo))
            {
                //���̰� �ε��� ������Ʈ�� Enemy���
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyFSM eFSM = hitInfo.transform.GetComponent<EnemyFSM>();
                    eFSM.HitEnemy(weaponPower);
                }
                else
                {
                    //�ǰ� ����Ʈ�� ��ġ�� ���̿� �ε��� �������� �̵�
                    bulletEffect.transform.position = hitInfo.point;
                    //�ǰ� ����Ʈ�� forward ������ ���̰� �ε��� ������ ���� ���Ϳ� ��ġ��Ų��
                    bulletEffect.transform.forward = hitInfo.normal;
                    ps.Play(); //�ǰ� ����Ʈ �÷���
                }
            }
            StartCoroutine(ShootEffectOn(0.05f)); //�� ����Ʈ �ǽ�

            //�����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
            lastAttackTime = Time.time;
            //���� �ִϸ��̼� ���
            anim.Play("Fire", -1, 0);
            //���� ���� ���
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
