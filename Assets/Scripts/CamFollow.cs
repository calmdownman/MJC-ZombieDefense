using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform target; //목표 트랜스폼

    // Update is called once per frame
    void Update()
    {
        //카메라의 위치를 목표 트랜스의 위치와 일치시킨다
        transform.position = target.position;
    }
}
