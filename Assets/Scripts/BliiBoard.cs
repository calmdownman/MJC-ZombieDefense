using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BliiBoard : MonoBehaviour
{ 
    void Update()
    {
        //�ڱ��ڽ�(���ʹ��� ü�¹� ĵ����)�� ������ ī�޶��� ����� ��ġ��Ų��.
        transform.forward = Camera.main.transform.forward;
    }
}
