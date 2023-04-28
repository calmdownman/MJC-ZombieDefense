using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    public Text wModeText;
    private AudioSource audioSource; //Fire Postion 

    [Header("Bomb")]
    public GameObject firePosition; 
    public GameObject bombFactory; 
    public float throwPower = 15f;
    public int rotatePower = 10;

    public GameObject bulletEffect;
    public GameObject booldEffect;
    ParticleSystem ps,bs; 
    public int weaponPower = 5;

    Animator anim;
    public GameObject[] eff_Flash;

    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting; 
    [SerializeField]
    private AudioClip audioClipFire; 

    private float lastAttackTime = 0; 
    private CamRotate recoil_Script;

    enum WeaponMode
    {
        Normal,
        Sniper,
        Rifle
    }
    WeaponMode wMode;
    bool ZoomMode = false; 
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ps = bulletEffect.GetComponent<ParticleSystem>();
        bs = booldEffect.GetComponent<ParticleSystem>();
        anim = GetComponentInChildren<Animator>();
        audioSource = transform.Find("FirePosition").GetComponent<AudioSource>();

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
                    GameObject bomb = Instantiate(bombFactory);
                    bomb.transform.position = firePosition.transform.position;

                    Rigidbody rb = bomb.GetComponent<Rigidbody>();

                    rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
                    rb.AddTorque(Vector3.back * rotatePower, ForceMode.Impulse); 
                    break;

                case WeaponMode.Sniper:
                    if (!ZoomMode)
                    {
                        GameManager.Instance.scopeObject.SetActive(true);
                        Camera.main.fieldOfView = 15f;
                        ZoomMode = true;
                    }
                    break;
            }
        }

        if (Input.GetMouseButtonUp(1)) 
        {
            switch (wMode)
            {
                case WeaponMode.Sniper:
                    if (ZoomMode) 
                    {
                        GameManager.Instance.scopeObject.SetActive(false);
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
       
            if(weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            } 
          
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

            if(anim.GetFloat("MoveMotion") > 0)
            {
                return;
            }

         
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitInfo = new RaycastHit(); 

            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyFSM eFSM = hitInfo.transform.GetComponent<EnemyFSM>();
                    booldEffect.transform.position = hitInfo.point;
                    booldEffect.transform.forward = hitInfo.normal;
                    bs.Play();
                    eFSM.HitEnemy(weaponPower);
                }
                else
                {
                    bulletEffect.transform.position = hitInfo.point;
                    bulletEffect.transform.forward = hitInfo.normal;
                    ps.Play(); 
                }
            }
            StartCoroutine(ShootEffectOn(0.05f)); 

            lastAttackTime = Time.time;
            anim.Play("Fire", -1, 0);
            PlaySound(audioClipFire);
            //연사 시 총 반동
            if(wMode == WeaponMode.Rifle) recoil_Script.RecoilFire();
        }
    }

    IEnumerator ShootEffectOn(float duration)
    {
        int num = Random.Range(0, eff_Flash.Length);
        eff_Flash[num].SetActive(true);
        yield return new WaitForSeconds(duration);
        eff_Flash[num].SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
