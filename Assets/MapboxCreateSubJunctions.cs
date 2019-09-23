using System.Collections;
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


    void Start()
    {
        /*
        Coordinates c1 = new Coordinates(151.1910184f, -33.9324425f);
        Coordinates c2 = new Coordinates(151.1914601f, -33.9325201f);
        Coordinates c3 = new Coordinates(151.19183f, -33.9326104f);
        Coordinates c4 = new Coordinates(151.1920802f, -33.9327024f);
        Coordinates c5 = new Coordinates(151.192379f, -33.9328467f);


        List<Coordinates> allCoords = new List<Coordinates>();

        allCoords.Add(c1);
        allCoords.Add(c2);
        allCoords.Add(c3);
        allCoords.Add(c4);
        allCoords.Add(c5);
        */

        /*
        junctionLocations.Add("-33.9324425,151.1910184");
        junctionLocations.Add("-33.9325201,151.1914601");
        junctionLocations.Add("-33.9326104,151.19183");
        junctionLocations.Add("-33.9327024,151.1920802");
        junctionLocations.Add("-33.9328467,151.192379");

        for (int i = 1; i < allCoords.Count - 1; i++)
        {
            SubJunctionsTrack j = new SubJunctionsTrack();

            double xCoord;
            double.TryParse(allCoords[i].getLongitude().ToString(), out xCoord);
            j.xCoordinate = xCoord;

            double zCoord;
            double.TryParse(allCoords[i].getLatitude().ToString(), out zCoord);
            j.zCoordinate = zCoord;

            allSubJunctions.Add(j);
        }
         * */

        List<string> allSubJunctions = new List<string>();
        allSubJunctions.Add("-33.9324425,151.1910184");
        allSubJunctions.Add("-33.9325201,151.1914601");
        allSubJunctions.Add("-33.9326104,151.19183");
        allSubJunctions.Add("-33.9327024,151.1920802");
        allSubJunctions.Add("-33.9328467,151.192379");

        List<string> subJunctionNames = new List<string>();
        subJunctionNames.Add("junction1");
        subJunctionNames.Add("junction2");
        subJunctionNames.Add("junction3");
        subJunctionNames.Add("junction4");
        subJunctionNames.Add("junction5");


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

        Debug.Log(Event.current.type);
    }

}