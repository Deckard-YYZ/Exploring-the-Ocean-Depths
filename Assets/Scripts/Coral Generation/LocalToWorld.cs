using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalToWorld : MonoBehaviour
{
    public Vector3 localCoordinate;
    public GameObject[] objectPrefabs;
    public BlocksController blocksController;
    // The parent object in the scene hierarchy
    public Transform parentObject;

    public int seed;
    public int clustersPerTerrain;
    
    public int MinCluster;
    public int MaxCluster;
    public float MinClusterRadius;
    public float MaxClusterRadius;

    private int[] _terrainSeeds;
    // Start is called before the first frame update
    void Start()
    {
        #region Seed Stuffs
        
        
        System.Random secondaryRandom = new System.Random(seed);
        int childCount = parentObject.childCount;
        _terrainSeeds = new int[childCount];
        
        //Generate Seed i for each terrain i
        //TODO Use the Terrain Collider to scale up the coordinate
        for (int i = 0; i < childCount; i++)
        {
            _terrainSeeds[i] = secondaryRandom.Next();
            // Debug.Log("Generated random value: " + _terrainSeeds[i] + " with seed: " + seed);
        }
        #endregion
        
        
        int coralCounts = objectPrefabs.Length;
        int counter = 0;
        foreach (Transform childTransform in parentObject)
        {
            Terrain terrain = childTransform.GetComponent<Terrain>();
            Vector3 terrainTransPos = terrain.transform.position;

            Random.InitState(_terrainSeeds[counter]);
            // Vector3 localP, localScaled, worldP, terrainScale;
            Vector3 worldP;
            // terrainScale = childTransform.localScale;
            Vector2 twoDCoord;
            Quaternion q;
            
            System.Random coordSeed = new System.Random(_terrainSeeds[counter]);
            System.Random coralSeed = new System.Random(_terrainSeeds[counter]);
            
            List<Vector3> clusters;
            int coralIndex;
            for (int i = 0; i < clustersPerTerrain; i++)
            {
                twoDCoord = new Vector2(Random.value, Random.value);
                worldP = GetWorldPositionOnTerrain(twoDCoord, terrain);
                
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
                    
                    // GameObject newObj = Instantiate(objectPrefabs[coralIndex], worldP, q, transform);
                    // newObj.isStatic = true;

                }
                
                q = GetTerrainQuaternionAt(worldP, terrain);
                // localP = new Vector3(Random.value, Random.value, Random.value);
                // localScaled = Vector3.Scale(localP, terrainScale);
                // worldP = childTransform.TransformPoint(localScaled);
                // Debug.Log("Local coordinate " + localP + " in world coordinates is " + worldP);
                
                
                
                
                
                // Debug.Log("Local coordinate " + twoDCoord + " in world coordinates is " + worldP);
                // blocksController.PlaceAtRightParentObject(objectPrefab, worldP, q);
                // Instantiate(objectPrefab, worldP, q, transform);
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
    public Vector3 GetWorldPositionOnTerrain(Vector2 normalizedPosition, Terrain terrain)
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
