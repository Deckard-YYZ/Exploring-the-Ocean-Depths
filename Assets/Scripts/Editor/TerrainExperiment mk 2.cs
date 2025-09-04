using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TerrainExperimentMk2 : MonoBehaviour
{
    [MenuItem("TerrainStuff/Do Something Mk2")]
    static void thingy()
    {
        //THESE VALUES MUST BE MANUALLY CHANGED FOR THE DATA USED
        int xChunks = 11;
        int yChunks = 6;
        float minVal = -10776;
        float maxVal = 8354;
        //DO NOT FORGET TO CHANGE THESE
        //SELECT TERRAIN OBJECTS IN ORDER OF INCREASING X THEN INCREASING Z (or Y, whichever)
        string currX;
        string currY;
        for (int chunk = 0; chunk < xChunks * yChunks; chunk++)
        {
            currX = (chunk % xChunks).ToString();
            currY = (chunk / xChunks).ToString();
            string[] lines = File.ReadAllLines("Assets/Scripts/"+currX+"-"+currY+".csv"); //don't forget to set max/min

            int nCols = lines[0].Split(",").Length;
            int nRows = lines.Length;

            float[,] heightValues = new float[nRows, nCols];
            float cell;
            string[] line;

            Debug.Log("Did you remember to set the max/min values? :>");

            float maxValHeight = maxVal - minVal;

            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i].Split(",");
                for (int j = 0; j < line.Length - 1; j++)
                {
                    cell = float.Parse(line[j]);
                    if (cell == -99999)
                    {
                        Debug.Log("Bad/Empty Data Found");
                        heightValues[i, j] = 0;
                    }
                    else
                    {
                        if (j != 0)
                        {
                            if (float.Parse(line[j]) == 0) //Math.Abs(float.Parse(line[j - 1]) - float.Parse(line[j])) >= 1000
                            {
                                Debug.Log("Large difference detected at line[" + j + "]" +
                                          " = " + line[j] + " and line[" + (j - 1) + "] = " + line[j - 1] + ", i = " +
                                          i);
                                heightValues[i, j] = (float.Parse(line[j-1]) - minVal) / maxValHeight;
                                line[j] = line[j-1];
                            }
                            else
                            {
                                heightValues[i, j] = (float.Parse(line[j]) - minVal) / maxValHeight;
                            }
                        }
                        else
                        {
                            heightValues[i, j] = (float.Parse(line[j]) - minVal) / maxValHeight;
                        }
                    }

                }
            }

            Terrain sel;
            TerrainData tData;
            sel = Selection.transforms[chunk].GetComponent(typeof(Terrain)) as Terrain;
            tData = sel.terrainData;
            tData.SetHeights(0, 0, heightValues);
        }
    }
}
