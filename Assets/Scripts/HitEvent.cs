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
        //플레이어에게 데미지를 입히기 위한 이벤트 함수
        efsm.AttackAction();
    }

    public void ShotBullet()
    {
       GameObject instantBullet = Instantiate(bullet, ts.position, ts.rotation);
       Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
       rigidBullet.velocity = ts.forward * 20;
    }
}
