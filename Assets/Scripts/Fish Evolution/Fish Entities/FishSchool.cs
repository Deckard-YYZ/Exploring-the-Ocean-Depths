using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FishSchool : MonoBehaviour
{
    [Header("Setup for Spawning")]
    [SerializeField] private FishSchoolUnit schoolFishPrefab;
    public int schoolSize;
    public int minSchoolSize;
    public int maxSchoolSize;
    public Vector3 spawnBounds;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float spawnDistanceFromPlayer;


    [Header("Speed Variables")]
    [Range(0f, 10f)]
    [SerializeField] private float _minSpeed;
    public float minSpeed { get { return _minSpeed; } }
    [Range(0f, 10f)]
    [SerializeField] private float _maxSpeed;
    public float maxSpeed { get { return _maxSpeed; } }


    [Header("Detection Distances")]
    [Range(0f, 10f)]
    [SerializeField] private float _cohesionDistance;

    public float cohesionDistance { get { return _cohesionDistance; } }

    [Range(0f, 10f)]
    [SerializeField] private float _avoidanceDistance;

    public float avoidanceDistance { get { return _avoidanceDistance; } }

    [Range(0f, 10f)]
    [SerializeField] private float _alignmentDistance;

    public float alignmentDistance { get { return _alignmentDistance; } }

    [Range(0f, 100f)]
    [SerializeField] private float _boundsDistance;

    public float boundsDistance { get { return _boundsDistance; } }

    [Range(0f, 100f)]
    [SerializeField] private float _obstacleDistance;

    public float obstacleDistance { get { return _obstacleDistance; } }




    [Header("Behaviour Weights")]
    [Range(0f, 10f)]
    [SerializeField] private float _cohesionWeight;

    public float cohesionWeight { get { return _cohesionWeight; } }

    [Range(0f, 10f)]
    [SerializeField] private float _avoidanceWeight;

    public float avoidanceWeight { get { return _avoidanceWeight; } }

    [Range(0f, 10f)]
    [SerializeField] private float _alignmentWeight;

    public float alignmentWeight { get { return _alignmentWeight; } }

    [Range(0f, 10f)]
    [SerializeField] private float _boundsWeight;

    public float boundsWeight { get { return _boundsWeight; } }

    [Range(0f, 100f)]
    [SerializeField] private float _obstacleWeight;

    public float obstacleWeight { get { return _obstacleWeight; } }

    private bool isSpawned = false;

    public FishSchoolUnit[] allFish { get; set; } 

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Fish Evolution") 
            playerTransform = FindObjectOfType<InputManager>().gameObject.transform;
        else
        {
            playerTransform = FindObjectOfType<PlayerMovement>().gameObject.transform;
        }
        schoolSize = Random.Range(minSchoolSize, maxSchoolSize);
    }

    private void Update()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) <= spawnDistanceFromPlayer)
        {
            if (isSpawned)
            {
                for (int i = 0; i < allFish.Length; i++)
                {
                    allFish[i].MoveFishUnit();
                }
            }
            else
            {
                GenerateFish();
                isSpawned = true;
            }
        }
        else
        {
            if (isSpawned)
            {
                foreach(Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
                isSpawned = false;
            }
        }
    }

    private void GenerateFish()
    {
        allFish = new FishSchoolUnit[schoolSize];

        for (int i = 0; i < schoolSize; i++)
        {
            var randomVector = UnityEngine.Random.insideUnitSphere;

            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawnPosition = transform.position + randomVector;
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            allFish[i] = Instantiate(schoolFishPrefab, spawnPosition, rotation);
            allFish[i].assignFishUnit(this);
            allFish[i].transform.SetParent(transform);

            allFish[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
        }
    }


}
