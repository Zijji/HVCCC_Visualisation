using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;

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
    
    
    public GameObject _junctionParent;
    
    List<GameObject> _spawnedObjects;

    public float zoom;
    
    MapboxCreateSections thisMBCS;

    void Start()
    {
        thisMBCS = GetComponent<MapboxCreateSections>();
        //EXCEL PART, THE CSV IS IN THE RESOURCES FOLDER
        TextAsset junctionData = Resources.Load<TextAsset>("Junctions_Coordinates");
        string[] data = junctionData.text.Split(new char[] { '\n' });
        List<string> junctionLocations = new List<string>();
        List<string> junctionNames = new List<string>();
        for (int i = 1; i < data.Length - 1; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            AllJunctions j = new AllJunctions();

            //gets the data for each row and adds to class AllJunctions
            j.id = row[0];

            j.signalName = row[1];
            //Debug.Log("he");
            string x = row[2];
            //            Debug.Log(x);
            x = x.Substring(1);
            double xCoord;
            double.TryParse(x, out xCoord);
            j.xCoordinate = xCoord;

            string z = row[3];
            z = z.Replace("\"", "");
            double zCoord;
            double.TryParse(z, out zCoord);
            j.zCoordinate = zCoord;

            junctions.Add(j);
            junctionLocations.Add(x + "," + z);
            junctionNames.Add(row[0]);
        }
        /*
        AllJunctions j1 = new AllJunctions();
        j1.id = "id1";
        j1.signalName = "signal1";
        j1.xCoordinate = -33.9324425;
        j1.zCoordinate = 151.1910184;
        junctions.Add(j1);
         *
        junctionLocations.Add("-33.9324425,151.1910184");
        junctionLocations.Add("-33.9325201,151.1914601");
        junctionLocations.Add("-33.9326104,151.19183");
        junctionLocations.Add("-33.9327024,151.1920802");
        junctionLocations.Add("-33.9328467,151.192379");
        junctionNames.Add("junction1");
        junctionNames.Add("junction2");
        junctionNames.Add("junction3");
        junctionNames.Add("junction4");
        junctionNames.Add("junction5");
         */



        //Inserts the junctions to the start of the location strings array. 
        string[] junctionLocationsArr = junctionLocations.ToArray();
        var allLocations = new string[_locationStrings.Length + junctionLocationsArr.Length];

        junctionLocationsArr.CopyTo(allLocations, 0);
        _locationStrings.CopyTo(allLocations, junctionLocationsArr.Length);
        /*
        _locationStrings.CopyTo(allLocations, 0);
        junctionLocationsArr.CopyTo(allLocations, _locationStrings.Length);
        */
        _locationStrings = allLocations;
        

        _locations = new Vector2d[_locationStrings.Length];
        _spawnedObjects = new List<GameObject>();
        for (int i = 0; i < _locationStrings.Length; i++)
        {
            var locationString = _locationStrings[i];
            _locations[i] = Conversions.StringToLatLon(locationString);
            var instance = Instantiate(_markerPrefab);
            instance.transform.parent = _junctionParent.transform;
            instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
            instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            if (i < junctionNames.Count)
            {
                instance.name = junctionNames[i];
            }

            _spawnedObjects.Add(instance);
        }
    
        zoom = _map.AbsoluteZoom;
        thisMBCS.AssignSections();
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
        
//        Debug.Log(Event.current.type);
    }

    public void setLocationStrings(string[] newStrings)
    {
        _locationStrings = newStrings;
        _locations = new Vector2d[_locationStrings.Length];
        
    }
    public string[] getLocationStrings()
    {
        return _locationStrings;
    }

}