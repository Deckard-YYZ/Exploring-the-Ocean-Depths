using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseEffect : MonoBehaviour
{

    public Material mat;
    // Start is called before the first frame update
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
