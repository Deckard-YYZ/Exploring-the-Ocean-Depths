using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GradientGenerator))]
public class GradientGeneratorEditor : Editor
{
    // Start is called before the first frame update
    public string mapName = "Default";
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.BeginHorizontal();
        GUILayout.Label("File name:");
        GradientGenerator gradientGenerator = (GradientGenerator)target;
        mapName = GUILayout.TextField(mapName);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Generate PNG Gradient Texture"))
        {
            gradientGenerator.BakeGradientTexture(mapName);
            AssetDatabase.Refresh();
        }
    }
}