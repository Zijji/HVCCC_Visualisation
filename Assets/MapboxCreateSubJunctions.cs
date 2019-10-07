using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;

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


    public GameObject _subJunctionParent;

    List<GameObject> _spawnedObjects;

    public float zoom;
    XMLHelper xml_helper = new XMLHelper();


    void Start()
    {

        List<List<List<float>>> allTracks = xml_helper.getAllTrackCoords();
        List<string> allSubJunctions = new List<string>();
        
        foreach (List<List<float>> tracks in allTracks)
        {
            foreach (List<float> subTracks in tracks)
            {
                allSubJunctions.Add(Convert.ToString(subTracks[1]) + "," + Convert.ToString(subTracks[0]));
            }
        }

        List<string> subJunctionNames = new List<string>();

        for (int i =0; i < allSubJunctions.Count; i++)
        {
            subJunctionNames.Add("subJunction_" + i);
        }


        //Inserts the junctions to the start of the location strings array. 
        string[] subJunctionLocationsArr = allSubJunctions.ToArray();
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