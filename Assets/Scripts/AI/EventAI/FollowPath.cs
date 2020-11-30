using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] Transform[] points = new Transform[0];
    // Start is called before the first frame update

    Transform currentTargetPoint;
    int currentPointIndex;
    void OnEnable()
    {
        currentTargetPoint = points[0];
    }

    // Update is called once per frame
    void Update()
    {
        Vector3.MoveTowards(transform.position,currentTargetPoint.position, speed*Time.deltaTime);
        if(Vector3.Distance(transform.position,currentTargetPoint.position) < .1f){
            if(currentPointIndex<points.Length){
                currentPointIndex++;
            }else{
                currentPointIndex = 0;
            }
            currentTargetPoint = points[currentPointIndex];

        }
    }
}
