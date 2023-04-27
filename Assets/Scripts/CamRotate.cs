
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    public float rotSpeed = 200f; //ȸ�� �ӵ� ����
    public float recoilRange = 3f;
    // Start is called before the first frame update
    float mx = 0; //ȸ�� �� ����
    float my = 0;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gState != GameManager.GameState.Run)
        {
            return;
        }

        MouseControl();

        //���콺 �Է� ���� �̿��� ȸ�� ������ ����
        /*
         * Vector3 dir = new Vector3(-mouse_Y, mouse_X, 0);

        //ȸ�� �������� ��ü�� ȸ�� ��Ų��.
        transform.eulerAngles += dir * rotSpeed * Time.deltaTime;

        //X�� ȸ��(���� ȸ��) ���� -90~90�� ���̷� �����Ѵ�
        Vector3 rot = transform.eulerAngles;
        rot.x = Mathf.Clamp(rot.x, -90f, 90f);
        transform.eulerAngles = rot;
        */
    }

    void MouseControl()
    {
        //����� ���콺 �Է��� ����
        float mouse_X = Input.GetAxis("Mouse X");
        float mouse_Y = Input.GetAxis("Mouse Y");

        //ȸ���� ������ ���콺 �Է� �� ��ŭ �̸� ������ ��Ų��.
        mx += mouse_X * rotSpeed * Time.deltaTime;
        my += mouse_Y * rotSpeed * Time.deltaTime;
        //���콺 ���� �̵� ȸ�� ����(my)�� ���� -90~90�� ���̷� �����Ѵ�.
        my = Mathf.Clamp(my, -90f, 90f);
        transform.eulerAngles = new Vector3(-my, mx, 0);
    }

    public void RecoilFire()
    {
        mx += Random.Range(-recoilRange/3, recoilRange/3);
        my += Random.Range(0, recoilRange);
    }
}
