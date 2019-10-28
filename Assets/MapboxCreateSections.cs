using System.Collections;
using System;
using System.IO;
using System.Linq;
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

    public string[] dpue;



    void Start()
    {

        
        
    }


    public void AssignSections()
    {
        //Looks at the geojson file directly to get the track coordinates

        //Returns json from the file.
        string getJsonPath = "hvccc_rail.geojson";
        string getJsonString = "";
        StreamReader sr = new StreamReader(getJsonPath); 
        getJsonString += sr.ReadToEnd();
        sr.Close();

        var getGeoJson = JSON.Parse(getJsonString);
        List<string> allCoords = new List<string>();    //All co-ordinates in values

        Debug.Log(getGeoJson["features"].AsArray.Count);
        Debug.Log(getGeoJson["features"][1]["geometry"]["coordinates"][0].AsArray.Count);
        
        List<string> allSections = new List<string>();      //allSections records the first co-ords of the section so it can be translated to vector3 position in location_strings  

        
        List<List<string>> allSectionsPaths = new List<List<string>>(); //Stores all sections paths in one big list.
        
        
        //gets path for one section
        //Test code for creating a single section
        for (int i1 = 0; i1 < getGeoJson["features"].AsArray.Count; i1++)
        {
            allSections.Add(getGeoJson["features"][i1]["geometry"]["coordinates"][0][0][1].Value + ", " + getGeoJson["features"][i1]["geometry"]["coordinates"][0][0][0].Value);
            List<string> sectionPath = new List<string>();
            for(int i2 = 0; i2 < getGeoJson["features"][i1]["geometry"]["coordinates"][0].AsArray.Count; i2++)
            {
                sectionPath.Add(getGeoJson["features"][i1]["geometry"]["coordinates"][0][i2][1].Value + ", " + getGeoJson["features"][i1]["geometry"]["coordinates"][0][i2][0].Value);
                //distinctSubjunctions.Add(getGeoJson["features"][i1]["geometry"]["coordinates"][0][i2][1].Value + ", " + getGeoJson["features"][i1]["geometry"]["coordinates"][0][i2][0].Value);
                allCoords.Add(getGeoJson["features"][i1]["geometry"]["coordinates"][0][i2][1].Value + ", " + getGeoJson["features"][i1]["geometry"]["coordinates"][0][i2][0].Value);
            }
            allSectionsPaths.Add(sectionPath);
        }

        string[] subJunctionLocationsArr = allSections.ToArray();


        var allSubLocations = new string[_locationStrings.Length + subJunctionLocationsArr.Length];

        subJunctionLocationsArr.CopyTo(allSubLocations, 0);
        _locationStrings.CopyTo(allSubLocations, subJunctionLocationsArr.Length);

        _locationStrings = allSubLocations;

        _locations = new Vector2d[_locationStrings.Length];
        _spawnedObjects = new List<GameObject>();

        //List that matches game object to allSections
        List<GameObject> existingSections = new List<GameObject>();
        //Contains sections that have added next sections
        HashSet<GameObject> existingSectionsHasNext = new HashSet<GameObject>();

        
        for (int i = 0; i < _locationStrings.Length; i++)
        {
            var locationString = _locationStrings[i];
            _locations[i] = Conversions.StringToLatLon(locationString);
            var instance = Instantiate(_markerPrefab);
            instance.transform.parent = _subJunctionParent.transform;
            instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
            instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            instance.name = "Section"+i.ToString();
            instance.GetComponent<Section>().pathCoords = allSectionsPaths[i].ToArray();
            GameObject fromJunc = GameObject.Find(getGeoJson["features"][i]["properties"]["fromjunc"].Value);
            GameObject toJunc = GameObject.Find(getGeoJson["features"][i]["properties"]["tojunc"].Value);
            //Adds the from- and to- junctions to the section
            instance.GetComponent<Section>().fromJunction = fromJunc;
            fromJunc.GetComponent<Junction>().junctionNeighbour.Add(toJunc);
            fromJunc.GetComponent<Junction>().junctionNeighbourSections.Add(instance);
            
            instance.GetComponent<Section>().toJunction = toJunc;
            toJunc.GetComponent<Junction>().junctionNeighbour.Add(fromJunc);
            toJunc.GetComponent<Junction>().junctionNeighbourSections.Add(instance);
            
            existingSections.Add(instance);
            
            _spawnedObjects.Add(instance);
        }
        
        zoom = _map.AbsoluteZoom;
        
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