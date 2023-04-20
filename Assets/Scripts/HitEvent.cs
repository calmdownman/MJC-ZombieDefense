using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEvent : MonoBehaviour
{
    public EnemyFSM efsm;
    public GameObject bullet;
    public Transform ts;
    public void PlayerHit()
    {
        //�÷��̾�� �������� ������ ���� �̺�Ʈ �Լ�
        efsm.AttackAction();
    }

    public void ShotBullet()
    {
       GameObject instantBullet = Instantiate(bullet, ts.position, ts.rotation);
       Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
       rigidBullet.velocity = ts.forward * 20;
    }
}
