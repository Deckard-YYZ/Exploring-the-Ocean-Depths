using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomFishSpawner : MonoBehaviour
{
    public int localFishMaximum = 8;
    public GameObject[] fishPrefabs;
    public GameObject fishCollection;
    public LayerMask fishLayer;

    public int maximumFishAllowed;

    public int spawnRange;
    public float spawnHeight;

    private bool isMinigame = false;

    // Start is called before the first frame update
    void Start()
    {
        SpawnInitialFish();
        if (SceneManager.GetActiveScene().name == "Fish Evolution")
        {
            isMinigame = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckAndSpawnFish();
    }


    void SpawnInitialFish()
    {
        for (int i = 0; i < localFishMaximum; i++)
        {
            int randomIndex = Random.Range(0, fishPrefabs.Length);
            Vector3 randomOffset = Random.insideUnitSphere * spawnRange;

            Vector3 spawnLocation = transform.position + new Vector3(randomOffset.x, Random.Range(isMinigame ? -spawnHeight : 0, spawnHeight), randomOffset.z);

            GameObject fish = Instantiate(fishPrefabs[randomIndex], spawnLocation, Quaternion.identity);

            fish.transform.SetParent(fishCollection.transform);
        }
    }
    void CheckAndSpawnFish()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, spawnRange, fishLayer);

        if (colliders.Length < localFishMaximum-2)
        {
            for (int i = colliders.Length; i < localFishMaximum; i++)
            {
                int randomIndex = Random.Range(0, fishPrefabs.Length);
                Vector3 playerPosition = transform.position;
                Quaternion playerRotation = transform.rotation;

                float spawnAngleRadians = Mathf.Clamp(90f, 0f, 180f) * Mathf.Deg2Rad; 
                float randomAngleOffset = Random.Range(-spawnAngleRadians / 2f, spawnAngleRadians / 2f); 
                Vector3 spawnDirection = Quaternion.Euler(0f, randomAngleOffset * Mathf.Rad2Deg, 0f) * playerRotation * Vector3.forward;
                Vector3 spawnPosition = playerPosition + new Vector3(0f, Random.Range(-spawnHeight * 2  , spawnHeight * 2), 0f) + spawnDirection * spawnRange;

                GameObject fish = Instantiate(fishPrefabs[randomIndex], spawnPosition, Quaternion.identity);
                FishSchool schoolStats = fish.GetComponent<FishSchool>();

                if (randomIndex == 2)
                {
                    schoolStats.schoolSize = Random.Range(10, 50);
                    schoolStats.spawnBounds = new Vector3(Random.Range(8, 15), Random.Range(4, 6), Random.Range(8, 15));
                }
                else if (randomIndex == 4)
                {
                    schoolStats.schoolSize = Random.Range(1, 4);
                    schoolStats.spawnBounds = new Vector3(Random.Range(45, 60), Random.Range(4, 6), Random.Range(45, 60));

                }
                else
                {
                    schoolStats.schoolSize = Random.Range(3, 10);
                    schoolStats.spawnBounds = new Vector3(Random.Range(20, 40), Random.Range(4, 6), Random.Range(24, 40));

                }

                fish.transform.SetParent(fishCollection.transform);
            }
        }

        if (fishCollection.transform.childCount > maximumFishAllowed)
        {
            Destroy(fishCollection.transform.GetChild(0).gameObject);
        }
    }
}
