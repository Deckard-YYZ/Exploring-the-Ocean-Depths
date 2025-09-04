using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TerrainExperiment : MonoBehaviour
{
    //[MenuItem("TerrainStuff/Do Something")]
    static void thingy()
    {
        string[] lines = File.ReadAllLines("Assets/Scripts/ireland.asc");
        
        int nCols = Int32.Parse(lines[0].Split(' ')[lines[0].Split(' ').Length-1]);
        int nRows = Int32.Parse(lines[1].Split(' ')[lines[0].Split(' ').Length-1]);
        
        float[,] heightValues = new float[nRows, nCols];
        float cell;
        string[] line;
        float minVal = 99999;
        float maxVal = -99999;
        
        for (int i = 6; i<lines.Length; i++)
        {
            line = lines[i].Split(" ");
            for (int j = 0; j<line.Length-1; j++)
            {
                cell = float.Parse(line[j]);
                if (cell != -9999)
                {
                    if (cell < minVal) minVal = cell;
                    if (cell > maxVal) maxVal = cell;
                }
            }
        }

        float maxValHeight = 1 - (maxVal / minVal);
        
        Debug.Log(minVal);
        Debug.Log(maxVal);
        
        for (int i = 6; i<lines.Length; i++)
        {
            line = lines[i].Split(" ");
            for (int j = 0; j<line.Length-1; j++)
            {
                cell = float.Parse(line[j]);
                if (cell == -9999)
                {
                    heightValues[i - 6, j] = 0;
                }
                else
                {
                    heightValues[i - 6, j] = (1 - (float.Parse(line[j]) / minVal))/maxValHeight;
                }
                
            }
        }

        Terrain sel;
        TerrainData tData;
        sel = Selection.transforms[0].GetComponent(typeof(Terrain)) as Terrain;
        tData = sel.terrainData;
        tData.SetHeights(0,0,heightValues);
    }
}
