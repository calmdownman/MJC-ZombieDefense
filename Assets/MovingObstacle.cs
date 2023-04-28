using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Range(0, 5)]
    public float speed;
    [Range(0, 2)]
    public float waitDuration;
    Vector3 targetPos;

    public GameObject ways;
    public Transform[] wayPoints;
    int pointIndex;
    int pointCount;
    int direction = 1;

   
    int speedMultiplier = 1;
    void Awake()
    {
        wayPoints = new Transform[ways.transform.childCount];
        for (int i = 0; i < ways.transform.childCount; i++) 
        {
            wayPoints[i] = ways.transform.GetChild(i).transform;
        }
    }

    private void Start()
    {
        pointCount = wayPoints.Length;
        pointIndex = 1;
        targetPos = wayPoints[pointIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var step = speedMultiplier * speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        if(transform.position == targetPos )
        {
            NextPoint();
        }
    }

    void NextPoint()
    {
        if (pointIndex == pointCount-1)
        {
            direction = -1;
        }

        if (pointIndex == 0)
        {
            direction = 1;
        }

        pointIndex += direction;
        targetPos = wayPoints[pointIndex].transform.position;
        StartCoroutine(WaitNextPoint());
    }

    IEnumerator WaitNextPoint()
    {
        speedMultiplier = 0;
        yield return new WaitForSeconds(waitDuration);
        speedMultiplier = 1;
    }
}
