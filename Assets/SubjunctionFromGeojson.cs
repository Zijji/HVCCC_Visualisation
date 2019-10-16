using UnityEngine;
using System.IO;
using SimpleJSON;

public class SubjunctionFromGeojson
{
    public string name;
    //public array features;
    
    //Getting Subjunctions from Geojson. May be removed later
    public static SubjunctionFromGeojson Create()
    {
        string geJsonPath = "hunter_valley_tracks.geojson";
        string getJsonString = "";
        StreamReader sr = new StreamReader(geJsonPath); 
        getJsonString += sr.ReadToEnd();
        sr.Close();

        return JsonUtility.FromJson<SubjunctionFromGeojson>(getJsonString);
    }
       
}