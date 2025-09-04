using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class SortedHeightObjectGenerator : MonoBehaviour
{
    private string _scriptablePath = "Assets/Scripts/Coral Generation/Data/";
    public Transform parentObject; 
    // Start is called before the first frame update
    void Start()
    {
        List<Vector3> list;
        foreach (Transform trans in parentObject)
        {
            list = GenerateHeightsScriptableObjects(trans);
            SortedHeight height = ScriptableObject.CreateInstance<SortedHeight>();
            height.vecs = list;
            string createPath = _scriptablePath + trans.name + ".asset";
            AssetDatabase.CreateAsset(height, createPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    
    //NOTE it uses world coordinates now, may wanna change to normalized local coordinates
    List<Vector3> GenerateHeightsScriptableObjects(Transform terrainTrans)
    {
        List<Vector3> list = new List<Vector3>();
        Terrain terrain = terrainTrans.GetComponent<Terrain>();
        Vector2 norm2DCoord;
        TerrainData terrainData = terrain.terrainData;
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = heightmapWidth;
        float terrainWidth = terrainData.size.x;
        float terrainHeight = terrainData.size.z;
        float terrainY = terrain.transform.position.y;
        float terrainX = terrain.transform.position.x;
        float terrainZ = terrain.transform.position.z;
        //2D array
        float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);
        float worldHeight, worldX, worldZ;
        for (int x = 0; x < heightmapWidth; x+=2) {
            for (int z = 0; z < heightmapHeight; z+=2) {
                // worldHeight = heights[x, z] * terrainData.size.y + terrain.transform.position.y;
                // if (worldHeight > heightThreshold) {
                //     Debug.Log("Height at (" + x + ", " + y + ") exceeds threshold: " + worldHeight);
                // }
                
                // // Normalize the heightmap coordinates
                // float normalizedX = (float)x / (heightmapWidth - 1);
                // float normalizedZ = (float)z / (heightmapWidth - 1);
                // // Convert normalized coordinates to local coordinates
                // float localX = normalizedX * terrainWidth;
                // float localZ = normalizedZ * terrainHeight;

                worldX = (float)x / (heightmapWidth - 1) * terrainWidth + terrainX;
                worldZ = (float)z / (heightmapWidth - 1) * terrainHeight + terrainZ;
                worldHeight = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrainY;
                list.Add(new Vector3(worldX,worldHeight,worldZ));
            }
        }
        //NOTE THIS IS ASCENDING ORDER
        list.Sort((v1, v2) => v1.y.CompareTo(v2.y));
        return list;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif