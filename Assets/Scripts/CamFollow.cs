
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform[] target; //��ǥ Ʈ������
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
      
        //ī�޶��� ��ġ�� ��ǥ Ʈ������ ��ġ�� ��ġ��Ų��
        transform.position = target[swap].position;
    }
}
