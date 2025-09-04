using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEntityMovement : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public int minMoveX;
    public int maxMoveX;
    public int minMoveY;
    public int maxMoveY;
    public int minMoveZ;
    public int maxMoveZ;

    public Transform movePoint;
    public Transform baseMovePoint;
    public float moveTimer = -1;
    private void Start()
    {
        baseMovePoint = transform;
    }

    // Update is called once per frame
    void Update()
    {
        SwimTowards();
        SwimRotate();
    }
    void SwimTowards() {
        if (moveTimer < 0 || Vector3.Distance(movePoint.transform.position, transform.position) < 3)
        {
            moveTimer = Random.Range(5f, 10f);
           
            movePoint.transform.position = transform.position + new Vector3(Random.Range(minMoveX, maxMoveX), Random.Range(minMoveY, maxMoveY), Random.Range(minMoveZ, maxMoveZ));
        }
        else
        {
            moveTimer -= Time.deltaTime;
            Vector3 direction = movePoint.transform.position - transform.position;
            direction.Normalize();
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    void SwimRotate()
    {
        Vector3 direction = movePoint.transform.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }
}
