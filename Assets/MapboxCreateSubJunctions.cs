using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using SimpleJSON;
using Console = System.Console;


public class MapboxCreateSubJunctions : MonoBehaviour
{
    //List<SubJunctionsTrack> allSubJunctions = new List<SubJunctionsTrack>();

    [SerializeField]
    AbstractMap _map;

    [SerializeField]
    [Geocode]
    string[] _locationStrings;
    Vector2d[] _locations;

    [SerializeField]
    float _spawnScale = 100f;

    [SerializeField]
    GameObject _markerPrefab;

    public static HashSet<string> subjunctionSet;

    public GameObject _subJunctionParent;

    List<GameObject> _spawnedObjects;

    public static List<string> allSubJunctions; 
    //public static List<string> allSubJunctionsLat;
    //public static List<string> allSubJunctionsLong;

    public float zoom;
    XMLHelper xml_helper = new XMLHelper();     //Not currently used, looks at hunter_vally_tracks.geojson instead



    void Start()
    {

        //Currently looks at the geojson file directly to get the track coordinates
        //May need to get from xml file.

        //Returns json from the file.
        string getJsonPath = "hunter_valley_tracks.geojson";
        string getJsonString = "";
        StreamReader sr = new StreamReader(getJsonPath); 
        getJsonString += sr.ReadToEnd();
        sr.Close();
        
        allSubJunctions = new List<string>();
        
        var getGeoJson = JSON.Parse(getJsonString);

        for(int i1 = 0; i1 < getGeoJson["features"].AsArray.Count; i1++)
        {
            for (int i2 = 0; i2 < getGeoJson["features"][i1]["geometry"]["coordinates"][0].AsArray.Count; i2++)
            {
                //Checks within 20km of Hexham for debugging purposes 
                /*
                if (
                    (checkDistance(float.Parse(allSubJunctions[i2].Split(',')[0]), float.Parse(allSubJunctions[i2].Split(',')[1]),
                         float.Parse("-32.836132"), float.Parse("151.686721")) < 20000))
                { */
                    allSubJunctions.Add(getGeoJson["features"][i1]["geometry"]["coordinates"][0][i2][1].Value + ", " +
                                        getGeoJson["features"][i1]["geometry"]["coordinates"][0][i2][0].Value);
                    //allSubJunctionsLat.Add(getGeoJson["features"][i1]["geometry"]["coordinates"][0][i2][1].Value );
                    //allSubJunctionsLong.Add(  getGeoJson["features"][i1]["geometry"]["coordinates"][0][i2][0].Value);
                //}
            }
        }
        //Removes duplicate subjunction coords
        var distinctSubjunctions = new HashSet<string>(allSubJunctions);
        
        allSubJunctions = new List<string>(distinctSubjunctions);
        
        //Would have to sort if we were using BinarySearch
        //allSubJunctions.Sort();
        
        //Removes subjunctions that are within 50m of each other
        for (int i =0; i < allSubJunctions.Count - 1; i++)
        {
            if ((checkDistance(float.Parse(allSubJunctions[i].Split(',')[0]), float.Parse(allSubJunctions[i].Split(',')[1]),
                    float.Parse(allSubJunctions[i + 1].Split(',')[0]), float.Parse(allSubJunctions[i + 1].Split(',')[1])))
                < 50)
            {
                allSubJunctions.RemoveAt(i);
            } 
        }
        
        List<string> subJunctionNames = new List<string>();

        for (int i =0; i < allSubJunctions.Count; i++)
        {
            subJunctionNames.Add("subJunction_" + i);
        }

        //Puts into set which makes it faster to search
        subjunctionSet = new HashSet<string>(allSubJunctions);

        //Inserts the junctions to the start of the location strings array. 
        string[] subJunctionLocationsArr = allSubJunctions.ToArray();
        var allSubLocations = new string[_locationStrings.Length + subJunctionLocationsArr.Length];

        subJunctionLocationsArr.CopyTo(allSubLocations, 0);
        _locationStrings.CopyTo(allSubLocations, subJunctionLocationsArr.Length);

        _locationStrings = allSubLocations;

        _locations = new Vector2d[_locationStrings.Length];
        _spawnedObjects = new List<GameObject>();
       
        //Disabled placement code for locations for testing
        for (int i = 0; i < _locationStrings.Length; i++)
        {
            var locationString = _locationStrings[i];
            _locations[i] = Conversions.StringToLatLon(locationString);
            var instance = Instantiate(_markerPrefab);
            instance.transform.parent = _subJunctionParent.transform;
            instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
            instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            if (i < subJunctionNames.Count)
            {
                instance.name = subJunctionNames[i];
            }
            _spawnedObjects.Add(instance);
        }
       
        zoom = _map.AbsoluteZoom;
    }

    /**
    void Update()
    {
        if (zoom != _map.AbsoluteZoom)
        {
            int count = _spawnedObjects.Count;
            for (int i = 0; i < count; i++)
            {
                var spawnedObject = _spawnedObjects[i];
                var location = _locations[i];
                spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
                spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            }

            zoom = _map.AbsoluteZoom;
        }
    }*/

    //https://stackoverflow.com/questions/6366408/calculating-distance-between-two-latitude-and-longitude-geocoordinates
    public double checkDistance(float longitude, float latitude, float otherLongitude, float otherLatitude)
    {
        var d1 = latitude * (Math.PI / 180.0);
        var num1 = longitude * (Math.PI / 180.0);
        var d2 = otherLatitude * (Math.PI / 180.0);
        var num2 = otherLongitude * (Math.PI / 180.0) - num1;
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

        return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
    }

  
    void OnGUI()
    {
        if (Event.current.type == EventType.ScrollWheel || Event.current.type == EventType.MouseDrag ||
            Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown)
        {
            int count = _spawnedObjects.Count;
            for (int i = 0; i < count; i++)
            {
                var spawnedObject = _spawnedObjects[i];
                var location = _locations[i];
                spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
                spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            }
        }
    }

}