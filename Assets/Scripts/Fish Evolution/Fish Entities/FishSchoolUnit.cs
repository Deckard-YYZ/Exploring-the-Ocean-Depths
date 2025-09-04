using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSchoolUnit : MonoBehaviour
{
    [SerializeField] private float FOVAngle;
    [SerializeField] private float smoothDamp;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Vector3[] directionsToCheckWhenAvoidingObstacles;

    private List<FishSchoolUnit> cohesionNeighbours = new List<FishSchoolUnit>();
    private List<FishSchoolUnit> avoidanceNeighbours = new List<FishSchoolUnit>();
    private List<FishSchoolUnit> alignmentNeighbours = new List<FishSchoolUnit>();
    private FishSchool assignedSchool;
    private Vector3 currentVelocity;
    private Vector3 currentObstacleAvoidanceVector;
    private float speed;

    public Transform ThisTransform { get; set; }

    private void Awake()
    {
        ThisTransform = transform;
    }

    public void assignFishUnit(FishSchool school)
    {
        assignedSchool = school;
    }

    public void InitializeSpeed(float speed)
    {
        this.speed = speed;
    }
    public void MoveFishUnit()
    {
        FindNeighbours();
        CalculateSpeed();

        var cohesionVector = CalculateCohesionVector() * assignedSchool.cohesionWeight;
        var avoidanceVector = CalculateAvoidanceVector() * assignedSchool.avoidanceWeight;
        var alignmentVector = CalculateAlignmentVector() * assignedSchool.alignmentWeight;
        var boundsVector = CalculateBoundsVector() * assignedSchool.boundsWeight;
        var obstacleVector = CalculateObstacleVector() * assignedSchool.obstacleWeight;

        var moveVector = cohesionVector + avoidanceVector + alignmentVector + boundsVector + obstacleVector;
        moveVector = Vector3.SmoothDamp(ThisTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;
        if (moveVector == Vector3.zero)
            moveVector = transform.forward;

        Quaternion rotation = Quaternion.LookRotation(moveVector, Vector3.up);
        ThisTransform.rotation = rotation;
        ThisTransform.forward = moveVector;
        ThisTransform.position += moveVector * Time.deltaTime;
    }

    private void CalculateSpeed()
    {
        if (cohesionNeighbours.Count == 0)
        {
            return;
        }
        speed = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            speed += cohesionNeighbours[i].speed;
        }
        speed /= cohesionNeighbours.Count;
        speed = Mathf.Clamp(speed, assignedSchool.minSpeed, assignedSchool.maxSpeed);
    }

    private void FindNeighbours()
    {
        cohesionNeighbours.Clear();
        avoidanceNeighbours.Clear();
        alignmentNeighbours.Clear();
        var allFishUnits = assignedSchool.allFish;
        for (int i = 0; i < allFishUnits.Length; i++)
        {
            var currentFishUnit = allFishUnits[i];
            if (currentFishUnit != this)
            {
                float currentNeighbourDistanceSqr = Vector3.SqrMagnitude(currentFishUnit.ThisTransform.position - ThisTransform.position);
                if (currentNeighbourDistanceSqr <= assignedSchool.cohesionDistance * assignedSchool.cohesionDistance)
                {
                    cohesionNeighbours.Add(currentFishUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedSchool.avoidanceDistance * assignedSchool.avoidanceDistance)
                {
                    avoidanceNeighbours.Add(currentFishUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedSchool.alignmentDistance * assignedSchool.alignmentDistance)
                {
                    alignmentNeighbours.Add(currentFishUnit);
                }
            }
        }
    }

    private Vector3 CalculateCohesionVector()
    {

        var cohesionVector = Vector3.zero;
        
        if (cohesionNeighbours.Count == 0)
        {
            return cohesionVector;
        }

        int neighboursInFOV = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++) 
        { 
            if (IsInFOV(cohesionNeighbours[i].ThisTransform.position))
            {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbours[i].ThisTransform.position;
            }
        }

        if (neighboursInFOV == 0)
        {
            return cohesionVector;
        }

        cohesionVector /= neighboursInFOV;
        cohesionVector -= ThisTransform.position;
        cohesionVector = Vector3.Normalize(cohesionVector);

        return cohesionVector;

    }

    private Vector3 CalculateAvoidanceVector()
    {
        var avoidanceVector = Vector3.zero;
        if (avoidanceNeighbours.Count == 0)
            return avoidanceVector;
        int neighboursInFOV = 0;
        for (int i = 0; i < avoidanceNeighbours.Count; i++)
        {
            if (IsInFOV(alignmentNeighbours[i].ThisTransform.forward))
            {
                neighboursInFOV++;
                avoidanceVector += (ThisTransform.position - avoidanceNeighbours[i].ThisTransform.position);
            }
        }
        if (neighboursInFOV == 0)
            return Vector3.zero;
        avoidanceVector /= neighboursInFOV;
        avoidanceVector = avoidanceVector.normalized;
        return avoidanceVector;
    }
    
    private Vector3 CalculateAlignmentVector()
    {
        var alignmentVector = ThisTransform.forward;
        if (alignmentNeighbours.Count == 0)
            return alignmentVector;
        int neighboursInFOV = 0;
        for (int i = 0; i < alignmentNeighbours.Count; i++)
        {
            if (IsInFOV(alignmentNeighbours[i].ThisTransform.forward))
            {
                neighboursInFOV++;
                alignmentVector += alignmentNeighbours[i].ThisTransform.forward;
            }
        }
        if (neighboursInFOV == 0)
            return ThisTransform.forward;
        alignmentVector /= neighboursInFOV;
        alignmentVector = alignmentVector.normalized;
        return alignmentVector;
    }

    private Vector3 CalculateBoundsVector()
    {
        var offsetToCenter = assignedSchool.transform.position - ThisTransform.position;
        bool isNearCenter = offsetToCenter.magnitude >= assignedSchool.boundsDistance * 0.9f;
        return isNearCenter ? offsetToCenter.normalized : Vector3.zero;

    }

    private Vector3 CalculateObstacleVector()
    {
        var obstacleVector = Vector3.zero;
        RaycastHit hit;

        if (Physics.Raycast(ThisTransform.position, ThisTransform.forward, out hit, assignedSchool.obstacleDistance, obstacleMask))
        {
            obstacleVector = FindBestDirectionToAvoid();
        }
        else
        {
            currentObstacleAvoidanceVector = Vector3.zero;
        }
        return obstacleVector;
    }

    private Vector3 FindBestDirectionToAvoid()
    {
        if (currentObstacleAvoidanceVector != Vector3.zero)
        {
            RaycastHit hit;
            if (Physics.Raycast(ThisTransform.position, ThisTransform.forward, out hit, assignedSchool.obstacleDistance, obstacleMask))
            {
                return currentObstacleAvoidanceVector;
            }
        }
        float maxDistance = int.MinValue;
        var selectedDirection = Vector3.zero;

        for (int i = 0; i< directionsToCheckWhenAvoidingObstacles.Length; i++)
        {
            RaycastHit hit;
            var currentDirection = ThisTransform.TransformDirection(directionsToCheckWhenAvoidingObstacles[i].normalized);
            if (Physics.Raycast(ThisTransform.position, currentDirection, out hit, assignedSchool.obstacleDistance, obstacleMask))
            {
                float currentDistance = (hit.point - ThisTransform.position).sqrMagnitude;
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    selectedDirection = currentDirection;
                }
            }
            else
            {
                selectedDirection = currentDirection;
                currentObstacleAvoidanceVector = currentDirection.normalized;
                return selectedDirection.normalized;
            }
        }
        return selectedDirection.normalized;
    }

    private bool IsInFOV(Vector3 position)
    {
        return Vector3.Angle(ThisTransform.forward, position - ThisTransform.position) <= FOVAngle;
    }
}
