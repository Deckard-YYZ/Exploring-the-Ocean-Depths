using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using File = System.IO.File;

public class GradientGenerator : MonoBehaviour
{

    public Gradient gradientMap;

    public string savingPath = "/Gradient Maps/";
    // Start is called before the first frame update
    public float width = 256f;
    public float height = 64f;

    private Texture2D gradientTexture;
    private Texture2D tempTexture;


    Texture2D GenerateGradientTexture(Gradient grad)
    {
        if (tempTexture == null)
        {
            tempTexture = new Texture2D((int)width, (int)height);
            
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = grad.Evaluate(0 + (x / width));
                tempTexture.SetPixel(x,y,color);
            }
        }

        tempTexture.wrapMode = TextureWrapMode.Clamp;
        tempTexture.Apply();
        return tempTexture;
    }


    public void BakeGradientTexture(string name)
    {
        gradientTexture = GenerateGradientTexture(gradientMap);
        byte[] _bytes = gradientTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + savingPath + name + ".png", _bytes);
    }
   
}