using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction : MonoBehaviour
{
    public GameObject bombEffect; //폭발 이펙트 프리팹 변수
    public int attackPower = 10; //수류탄 공격력
    public float explosionRadius = 5f; //폭발 효과 반경
    private void OnCollisionEnter(Collision collision)
    {
        //수류탄 반경 5미터 안에 있는 오브젝트들 중에서 레이어가 10(Enemy)인 오브젝트들을 반환
        Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius, 1<<10);
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].GetComponent<EnemyFSM>().HitEnemy(attackPower);
        }

        //폭발 프리팹을 생성
        GameObject eff = Instantiate(bombEffect);
        //폭발 프리팹을 수류탄 오브젝트의 위치와 동일하게 한다.
        eff.transform.position = transform.position;
        Destroy(gameObject);//자신을 제거
    }
}
