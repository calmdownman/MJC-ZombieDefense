
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform[] target; //목표 트랜스폼
    int swap = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) 
        {
            if(swap == 0)
            {
                swap = 1;
            } else
            {
                swap = 0;
            }
        } 
      
        //카메라의 위치를 목표 트랜스의 위치와 일치시킨다
        transform.position = target[swap].position;
    }
}
