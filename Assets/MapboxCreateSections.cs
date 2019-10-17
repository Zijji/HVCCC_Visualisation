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

public class MapboxCreateSections : MonoBehaviour
{

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


    public GameObject _subJunctionParent;       //Change to section parent later

    List<GameObject> _spawnedObjects;

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
/*
"features": [
{ "type": "Feature", "properties": { "osm_id": "2654260", "code": 6102, "fclass": "light_rail", "name": "Inner West Light Rail" }, "geometry": { "type": "MultiLineString", "coordinates": [ [ [ 151.1964198, -33.868837 ], [ 151.1962464, -33.8687557 ] ] ] } },

 */
 /*
        string example = GJ["features"][0]["properties"]["name"].Value;
        Debug.Log("value:"+example);
        string geometry = GJ["features"][0]["geometry"]["coordinates"][0][0][0].Value;
        Debug.Log("geometry:"+geometry);
        Debug.Log("isNull?:"+ geometry==null);
        var arrayCount = GJ["features"][0]["geometry"]["coordinates"][0][0][2].Value;
        Debug.Log("end of array:"+arrayCount);
        Debug.Log("isEmpty?:"+ arrayCount.Equals(""));
  */
        
        //var thisCoord = GJ["features"][0]["geometry"]["coordinates"][0][0][0].Value;
        // 1: spawn all subjunctions   2:link subjunctions  3: get closest junctions to subjunctions  4: 

        


        var getGeoJson = JSON.Parse(getJsonString);
        /*
        Debug.Log("array length:" + );
        Debug.Log("end of array0:" + getGeoJson["features"][0]["geometry"]["coordinates"][0][0][0].Value.Equals(""));
        Debug.Log("end of array1:" + getGeoJson["features"][0]["geometry"]["coordinates"][0][0][1].Value.Equals(""));
        Debug.Log("end of array2:" + getGeoJson["features"][0]["geometry"]["coordinates"][0][0][2].Value.Equals(""));
         */
         
        Debug.Log(getGeoJson["features"].AsArray.Count);
        Debug.Log(getGeoJson["features"][1]["geometry"]["coordinates"][0].AsArray.Count);
        
        //pathCoords = null;
        List<string> allSections = new List<string>();      //allSections records the first co-ords of the section so it can be translated to vector3 position in location_strings  
        
        
              //gets path for one section
        //Test code for creating a single section
        for (int i1 = 0; i1 < getGeoJson["features"].AsArray.Count; i1++)
        {
            allSections.Add(getGeoJson["features"][i1]["geometry"]["coordinates"][0][0][1].Value + ", " + getGeoJson["features"][i1]["geometry"]["coordinates"][0][0][0].Value);
            for(int i2 = 0; i2 < getGeoJson["features"][i1]["geometry"]["coordinates"][0].AsArray.Count; i2++)
            {
                List<string> sectionPath = new List<string>();
                sectionPath.Add(getGeoJson["features"][i1]["geometry"]["coordinates"][0][i2][1].Value + ", " + getGeoJson["features"][i1]["geometry"]["coordinates"][0][i2][0].Value);
            }

        }
        

        /*
        //Removes duplicate subjunction coords
        var distinctSubjunctions = new HashSet<string>(allSections);
        
        allSections = new List<string>(distinctSubjunctions);
        
         */
        
        /*
        for(i = 0; !getGeoJson["features"][i].Equals(""); i++)
        {
            for(i2 = 0; !getGeoJson["features"][i]["geometry"]["coordinates"][0][i2].Equals(""); i2++)
            {
                allSections.Add(getGeoJson["features"][i]["geometry"]["coordinates"][0][i2][0].Value + "," + getGeoJson["features"][i]["geometry"]["coordinates"][0][i2][1].Value);
            }
        }
        */

        /*
        List<List<List<float>>> allTracks = xml_helper.getAllTrackCoords();
        List<string> allSections = new List<string>();
        
        foreach (List<List<float>> tracks in allTracks)
        {
            foreach (List<float> subTracks in tracks)
            {
                allSections.Add(Convert.ToString(subTracks[1]) + "," + Convert.ToString(subTracks[0]));
            }
        }
         */

        /*
        List<string> subJunctionNames = new List<string>(); 

        for (int i =0; i < allSections.Count; i++)
        {
            subJunctionNames.Add("subJunction_" + i);
        }
        
         */
        


        //Inserts the junctions to the start of the location strings array. 
        //string[] subJunctionLocationsArr = allSections.ToArray();

        string[] subJunctionLocationsArr = allSections.ToArray();


        var allSubLocations = new string[_locationStrings.Length + subJunctionLocationsArr.Length];

        subJunctionLocationsArr.CopyTo(allSubLocations, 0);
        _locationStrings.CopyTo(allSubLocations, subJunctionLocationsArr.Length);

        _locationStrings = allSubLocations;

        _locations = new Vector2d[_locationStrings.Length];
        _spawnedObjects = new List<GameObject>();
        
        for (int i = 0; i < _locationStrings.Length; i++)
        {
            var locationString = _locationStrings[i];
            _locations[i] = Conversions.StringToLatLon(locationString);
            var instance = Instantiate(_markerPrefab);
            instance.transform.parent = _subJunctionParent.transform;
            instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
            instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            instance.GetComponent<Section>().pathCoords = allSections.ToArray();
            /*
            if (i < subJunctionNames.Count)
            {
                instance.name = subJunctionNames[i];
            }
            */
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

        Debug.Log(Event.current.type);
    }

}