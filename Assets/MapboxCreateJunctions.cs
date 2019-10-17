using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;

//Creates the junctions using the mapbox scaling
public class MapboxCreateJunctions : MonoBehaviour
{
    public XMLHelper xml_helper = new XMLHelper();
    List<AllJunctions> junctions = new List<AllJunctions>();

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
    
    //Holds all junctions in one gameobject
    public GameObject _junctionParent;
    
    List<GameObject> _spawnedObjects;

    public float zoom;
    

    void Start()
    {
	
        //EXCEL PART, THE CSV IS IN THE RESOURCES FOLDER
        TextAsset junctionData = Resources.Load<TextAsset>("Junctions_Coordinates");
        string[] data = junctionData.text.Split(new char[] { '\n' });
        List<string> junctionLocations = new List<string>();
        List<string> junctionNames = new List<string>();
        
        //Iterates through the data of the junctions
        for (int i = 1; i < data.Length - 1; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            AllJunctions j = new AllJunctions();

            //gets the data for each row and adds to class AllJunctions
            j.id = row[0];

            //Gets name
            j.signalName = row[1];
            
            //Latitude
            string x = row[2];
            x = x.Substring(1);
            double xCoord;
            double.TryParse(x, out xCoord);
            j.xCoordinate = xCoord;

            //Longitude
            string z = row[3];
            z = z.Replace("\"", "");
            double zCoord;
            double.TryParse(z, out zCoord);
            j.zCoordinate = zCoord;

            //Adds to a list
            junctions.Add(j);
            junctionLocations.Add(x + "," + z);
            junctionNames.Add(row[0]);
        }

        //Inserts the junctions to the start of the location strings array. 
        string[] junctionLocationsArr = junctionLocations.ToArray();
        var allLocations = new string[_locationStrings.Length + junctionLocationsArr.Length];

        //Begins mapbox conversion
        junctionLocationsArr.CopyTo(allLocations, 0);
        _locationStrings.CopyTo(allLocations, junctionLocationsArr.Length);
        _locationStrings = allLocations;
        
        //Uses gameobjects
        _locations = new Vector2d[_locationStrings.Length];
        _spawnedObjects = new List<GameObject>();

        //Iterates through the locations and makes the new mapbox friendly gameobjects
        for (int i = 0; i < _locationStrings.Length; i++)
        {
            var locationString = _locationStrings[i];
            _locations[i] = Conversions.StringToLatLon(locationString);
            var instance = Instantiate(_markerPrefab);
            instance.transform.parent = _junctionParent.transform;
            instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);

            //Checks position based on scale (zoom)
            instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            if (i < junctionNames.Count)
            {
                instance.name = junctionNames[i];
            }
            _spawnedObjects.Add(instance);
        }
        zoom = _map.AbsoluteZoom;
    }

    //Called on every frame, checks for zoom
    void Update()
    {
        if (zoom != _map.AbsoluteZoom)
        {
            int count = _spawnedObjects.Count;
            //refreshes the junction positions based on the map zoom
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
    
    //When the user interacts with the program 
    void OnGUI()
    {
        //On scroll, mouse drag, WASD
        if (Event.current.type == EventType.ScrollWheel || Event.current.type == EventType.MouseDrag || 
            Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown || Event.current.Equals(Event.KeyboardEvent("w"))
            || Event.current.Equals(Event.KeyboardEvent("a")) || Event.current.Equals(Event.KeyboardEvent("s")) || Event.current.Equals(Event.KeyboardEvent("d")))
        {
            int count = _spawnedObjects.Count;
            //Updates coords
            for (int i = 0; i < count; i++)
            {
                var spawnedObject = _spawnedObjects[i];
                var location = _locations[i];
                spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
                spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            }
        }
        else
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