using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    //제거될 시간 변수
    public float destroyTime = 1.5f;
    float currentTime = 0f; //경과 시간 측정용 변수

    // Update is called once per frame
    void Update()
    {
        if (currentTime > destroyTime)  //경과시간이 제거될 시간보다 높으면
        { 
            Destroy(gameObject); //자기 자신을 제거
        }
        currentTime += Time.deltaTime; //경과 시간을 누적
    }
}
