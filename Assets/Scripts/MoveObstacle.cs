using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstacle : MonoBehaviour
{
    Vector3 originTransform;
    Vector3 directionTransform;
    public int time;
    // Start is called before the first frame update
    private void Awake()
    {
        originTransform = transform.position;
        directionTransform = originTransform - new Vector3(0,1.7f,0);
    }

    private void OnEnable()
    {
        StartCoroutine(MovingDirection(time));
    }

    IEnumerator MovingDirection(int duration)
    {
        var runtime = 0.0f;

        while (runtime < duration)
        {
            runtime += Time.deltaTime;
            transform.position = Vector3.Slerp(originTransform, directionTransform, runtime/duration);
            yield return null;
        }

    }

}
