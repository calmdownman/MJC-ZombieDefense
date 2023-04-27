
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    public float rotSpeed = 200f; //회전 속도 변수
    public float recoilRange = 3f;
    // Start is called before the first frame update
    float mx = 0; //회전 값 변수
    float my = 0;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gState != GameManager.GameState.Run)
        {
            return;
        }

        MouseControl();

        //마우스 입력 값을 이용해 회전 방향을 결정
        /*
         * Vector3 dir = new Vector3(-mouse_Y, mouse_X, 0);

        //회전 방향으로 물체를 회전 시킨다.
        transform.eulerAngles += dir * rotSpeed * Time.deltaTime;

        //X축 회전(상하 회전) 값을 -90~90도 사이로 제한한다
        Vector3 rot = transform.eulerAngles;
        rot.x = Mathf.Clamp(rot.x, -90f, 90f);
        transform.eulerAngles = rot;
        */
    }

    void MouseControl()
    {
        //사용자 마우스 입력을 받음
        float mouse_X = Input.GetAxis("Mouse X");
        float mouse_Y = Input.GetAxis("Mouse Y");

        //회전값 변수에 마우스 입력 값 만큼 미리 누적을 시킨다.
        mx += mouse_X * rotSpeed * Time.deltaTime;
        my += mouse_Y * rotSpeed * Time.deltaTime;
        //마우스 상하 이동 회전 변수(my)의 값을 -90~90도 사이로 제한한다.
        my = Mathf.Clamp(my, -90f, 90f);
        transform.eulerAngles = new Vector3(-my, mx, 0);
    }

    public void RecoilFire()
    {
        mx += Random.Range(-recoilRange/3, recoilRange/3);
        my += Random.Range(0, recoilRange);
    }
}
