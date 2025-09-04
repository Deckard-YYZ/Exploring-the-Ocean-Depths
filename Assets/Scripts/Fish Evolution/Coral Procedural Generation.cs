using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoralProceduralGeneration : MonoBehaviour
{

    public int coralFieldSize;
    public int elementSpacing;
    public int maxCoralSize;

    public CoralElement[] coralElements;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = (int)transform.position.x - coralFieldSize/2; x < (int)transform.position.x + coralFieldSize/2; x += elementSpacing) 
        {
            for (int z = (int)transform.position.z - coralFieldSize / 2; z < (int)transform.position.z + coralFieldSize / 2; z += elementSpacing)
            {
                int coralSize = Random.Range(0, maxCoralSize);
                for (int i = 0; i < coralSize; i++) 
                {
                    CoralElement coral = coralElements[Random.Range(0, coralElements.Length)];
                    Vector3 position = new Vector3(x + i, coral.coralPrefab.transform.position.y, z + i);
                    position.y += Terrain.activeTerrain.SampleHeight(position);

                    Vector3 offset = new Vector3(Random.Range(-2, 2), 0f, Random.Range(-2, 2));

                    Vector3 rotation = new Vector3(Random.Range(0, 5f), Random.Range(0, 360), Random.Range(0, 5f));

                    Vector3 scale = coral.coralPrefab.transform.localScale * Random.Range(0.75f, 1.25f);
                    GameObject newCoral = Instantiate(coral.coralPrefab);
                    newCoral.transform.position = position + offset;
                    newCoral.transform.eulerAngles += rotation;
                    newCoral.transform.localScale = scale;

                    newCoral.transform.SetParent(transform);
                }
            }
        }
    }

   

    [System.Serializable]
    public class CoralElement
    {
        public string name;
        public GameObject coralPrefab;
    }
}
