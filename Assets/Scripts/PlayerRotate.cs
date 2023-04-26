
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float rotSpeed = 200f;
    float mx = 0; //ȸ���� ����(Y�ุ ȸ��)
    void Update()
    {
        if (GameManager.Instance.gState != GameManager.GameState.Run)
        {
            return;
        }

        float mouse_X = Input.GetAxis("Mouse X");
        //ȸ�� �� ������ ���콺 �Է� ����ŭ �̸� ���� ��Ų��.
        mx += mouse_X * rotSpeed * Time.deltaTime;
        //ȸ�� �������� Y�� ȸ���� �Ѵ�.
        transform.eulerAngles = new Vector3(0, mx, 0);
    }
}
