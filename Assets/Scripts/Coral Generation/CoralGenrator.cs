using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class CoralGenrator : MonoBehaviour
{
    public BlocksController blocksController;
    public Vector3 localCoordinate;
    public GameObject[] objectPrefabs;
    // The parent object in the scene hierarchy
    public Transform TerrainsParent;
    // public Transform CoralsParentObj;

    public int MinCluster;
    public int MaxCluster;
    public float MinClusterRadius;
    public float MaxClusterRadius;
    public int seed;
    public int clustersPerTerrain;

    public float minHeight;//The start height for placing corals

    private int[] _terrainSeeds;
    private string _scriptablePath = "Assets/Resources/";
    // Start is called before the first frame update
    void Start()
    {
        #region Seed Stuffs
        System.Random seedPerTerrain = new System.Random(seed);
        int childCount = TerrainsParent.childCount;
        _terrainSeeds = new int[childCount];
        
        //Generate Seed i for each terrain i
        //TODO Use the Terrain Collider to scale up the coordinate
        for (int i = 0; i < childCount; i++)
        {
            _terrainSeeds[i] = seedPerTerrain.Next();
            Debug.Log("Generated random value: " + _terrainSeeds[i] + " with seed: " + seed);
        }
        //NOTE This is the seed to generate clusters around a coordinate
        Random.InitState(12345);
        #endregion

        int coralCounts = objectPrefabs.Length;
        
        int counter = 0;
        foreach (Transform childTransform in TerrainsParent)
        {
            Terrain terrain = childTransform.GetComponent<Terrain>();
            SortedHeight scriptable = FindScriptableObjectByName(childTransform.name);
            List<Vector3> sortedHeight = scriptable.vecs;
            // SortedHeight sortedHeight = FindScriptableObjectByName(childTransform.name);
            
            //Use Secondary seed Get Secondary Random Generator
            System.Random coordSeed = new System.Random(_terrainSeeds[counter]);
            System.Random coralSeed = new System.Random(_terrainSeeds[counter]);
            
            //Get Info from our Sorted Height Scriptable Object
            float maxH = sortedHeight[^1].y;//end expression: last one
            int approxIndex =(int) (minHeight / maxH * sortedHeight.Count);
            int firstGreaterOrEqualIndex = BinarySearchFirstGreaterOrEqual(sortedHeight, minHeight, approxIndex);
            int suitableNumCoords = sortedHeight.Count - 1 - firstGreaterOrEqualIndex;
            int clusterThisTerrain = clustersPerTerrain > suitableNumCoords ? suitableNumCoords : clustersPerTerrain;
            
            Vector3 terrainTransPos = terrain.transform.position;

            int sortedHeightRandomIndex, coralIndex;
            Vector3 worldP;
            // terrainScale = childTransform.localScale;
            Quaternion q;
            List<Vector3> clusters;
            for (int i = 0; i < clusterThisTerrain; i++)
            {
                sortedHeightRandomIndex = coordSeed.Next(firstGreaterOrEqualIndex, sortedHeight.Count);
                worldP = sortedHeight[sortedHeightRandomIndex];
                // q = GetTerrainQuaternionAt(worldP, terrain);

                int NumThisCluster = coordSeed.Next(MinCluster, MaxCluster);
                float normalized = (NumThisCluster - MinCluster) / (float)(MaxCluster - MinCluster);
                float mappedValue = normalized * (MaxClusterRadius - MinClusterRadius) + MinClusterRadius;
                clusters = GenerateRandomPositions(worldP, mappedValue, NumThisCluster);
                Vector3 temp;
                for (int j = 0; j < clusters.Count; j++) {
                    temp = clusters[j];
                    temp.y = terrain.SampleHeight(clusters[j]) + terrainTransPos.y;
                    // q = GetTerrainQuaternionAt(temp, terrain);
                    // clusters[j] = temp;
                    q = Quaternion.identity;
                    coralIndex = coralSeed.Next(0, coralCounts);
                    blocksController.PlaceAtRightParentObject(objectPrefabs[coralIndex], temp, q);
                }

                // Instantiate(objectPrefab, worldP, q, CoralsParentObj);
            }
            // Random.Range(0f, 1f);
            // // Instantiate the object at the child's position and rotation
            // worldCoordinate = childTransform.TransformPoint(localCoordinate);
            // GameObject spawnedObject = Instantiate(objectPrefab, worldCoordinate, Quaternion.identity, transform);
            // Debug.Log("Local coordinate " + localCoordinate + " in world coordinates is " + worldCoordinate);
            counter++;
        }
        // Vector3 worldCoordinate = transform.TransformPoint(localCoordinate);
        // Debug.Log("Local coordinate " + localCoordinate + " in world coordinates is " + worldCoordinate);
        // GameObject spawnedObject = Instantiate(objectPrefab, worldCoordinate, Quaternion.identity, parentObject);
    }
    public Vector3 GetWorldPositionOnTerrain(Vector2 normalizedPosition, Terrain terrain) {
        Vector3 tDataSize = terrain.terrainData.size;
        Vector3 terrainTransPos = terrain.transform.position;
        // Calculate world x and z coordinates based on normalized position
        float worldX = normalizedPosition.x * tDataSize.x + terrainTransPos.x;
        float worldZ = normalizedPosition.y * tDataSize.z + terrainTransPos.z;
    
        // Get the height of the terrain at this world position
        Vector3 value = new Vector3(worldX, 0, worldZ);
        // float worldY = terrain.SampleHeight(value) + terrainTransPos.y;
        // value.y = worldY;
        value.y = terrain.SampleHeight(value) + terrainTransPos.y;
        // Return the 3D world position
        return value;
    }
    
   
    

    Quaternion GetTerrainQuaternionAt(Vector3 worldPosition, Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        // Convert to terrain local space
        Vector3 terrainLocalPosition = worldPosition - terrain.transform.position;
        int mapX = Mathf.RoundToInt(terrainLocalPosition.x / terrainData.size.x * terrainData.heightmapResolution);
        int mapZ = Mathf.RoundToInt(terrainLocalPosition.z / terrainData.size.z * terrainData.heightmapResolution);

        // Sample the heights to approximate the normal
        float heightXPlus = terrainData.GetHeight(mapX + 1, mapZ);
        float heightXMinus = terrainData.GetHeight(mapX - 1, mapZ);
        float heightZPlus = terrainData.GetHeight(mapX, mapZ + 1);
        float heightZMinus = terrainData.GetHeight(mapX, mapZ - 1);

        Vector3 gradientX = new Vector3(1f, heightXPlus - heightXMinus, 0f);
        Vector3 gradientZ = new Vector3(0f, heightZPlus - heightZMinus, 1f);
        Vector3 normal = Vector3.Cross(gradientZ, gradientX).normalized;

        return Quaternion.FromToRotation(Vector3.up, normal);
    }
    
    SortedHeight FindScriptableObjectByName(string scriptableObjectName)
    {
        // Get all ScriptableObject assets in the folder
        // string[] assetPaths = Directory.GetFiles(_scriptablePath, "*.asset");
        string[] assetPaths = new[] { "Terrain", "Terrain_(0.00, 0.00, 10000.00)", "Terrain_(10000.00, 0.00, 0.00)","Terrain_(10000.00, 0.00, 10000.00)"};

        foreach (string assetPath in assetPaths)
        {
            // string fileName = Path.GetFileNameWithoutExtension(assetPath);
            if (assetPath != scriptableObjectName) continue;
            // Load the ScriptableObject asset
            //SortedHeight scriptableObject = AssetDatabase.LoadAssetAtPath<SortedHeight>(assetPath);
            SortedHeight scriptableObject = Resources.Load<SortedHeight>(assetPath);

            return scriptableObject;
        }

        Debug.LogWarning("ScriptableObject not found with name: " + scriptableObjectName + " in folder: " + _scriptablePath);
        return null;
    }
    
    
    int BinarySearchFirstGreaterOrEqual(List<Vector3> list, float value, int startIndex) {
        int left = startIndex;
        int right = list.Count - 1;
    
        // Check if the value at startIndex is greater than or equal to the target value
        if (list[startIndex].y >= value) {
            // Binary search back to an index that is less than value
            while (left <= right) {
                int mid = left + (right - left) / 2;
                if (list[mid].y < value) {
                    // If current element is less than value, move right
                    left = mid + 1;
                } else {
                    // If current element is greater than or equal to value, move left
                    right = mid - 1;
                }
            }
            // Update startIndex to the last index less than value
            startIndex = left;
        }
    
        left = startIndex;
        right = list.Count - 1;
    
        // Perform binary search from startIndex
        while (left <= right) {
            int mid = left + (right - left) / 2;
            if (list[mid].y >= value) {
                // If current element is greater than or equal to value, move left
                right = mid - 1; 
            } else {
                // If current element is less than value, move right
                left = mid + 1;
            }
        }
        // If no element is greater than or equal to value, return -1
        if (left >= list.Count) {
            return -1;
        }
        return left;
    }

    //  Binary search to find the first element(INDEX) greater than or equal to value
    // int BinarySearchFirstGreaterOrEqual(List<Vector3> list, float value) {
    //     int left = 0;
    //     int right = list.Count - 1;
    //     while (left <= right) {
    //         int mid = left + (right - left) / 2;
    //         if (list[mid].y >= value) {
    //             // If current element is greater than or equal to value, move left
    //             right = mid - 1; 
    //         }else {
    //             // If current element is less than value, move right
    //             left = mid + 1;
    //         }
    //     }
    //     // If no element is greater than or equal to value, return -1
    //     if (left >= list.Count) {
    //         return -1;
    //     }
    //     return left;
    // }
    
    List<Vector3> GenerateRandomPositions(Vector3 center, float radius, int numPoints) {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < numPoints; i++) {
            float angle = Random.value * 2 * Mathf.PI;  // Random angle
            float distance = Mathf.Sqrt(Random.value) * radius;  // Random distance within the radius
            //NOTE y sets to 0 for calculating the y at the terrain
            Vector3 randomPoint = new Vector3(center.x + distance * Mathf.Cos(angle), 0, center.z + distance * Mathf.Sin(angle));
            positions.Add(randomPoint);
        }
        return positions;
    }
    

}
