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


        //Currently looks at the geojson file directly to get the track coordinates
        //May need to get from xml file.

        //Returns json from the file.
        string getJsonPath = "hvccc_rail.geojson";
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
         
        //pathCoords = null;
        List<string> allCoords = new List<string>();    //All co-ordinates in values

        Debug.Log(getGeoJson["features"].AsArray.Count);
        Debug.Log(getGeoJson["features"][1]["geometry"]["coordinates"][0].AsArray.Count);
        
        //pathCoords = null;
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
        /*
        //Gets a list of all subjunctions that appear more than once
        HashSet<string> distinctSubjunctions = new HashSet<string>(allCoords);   //distinct subJunctions.
        HashSet<string> duplicateSubjunctions = new HashSet<string>();           //All duplicate sub junctions

        foreach(string s in distinctSubjunctions)
        {
            //Counts number of times string appears in allcoords. If found more than once, adds to duplicates list.
            int numEntries = 0;
            int coordIndex = 0;
            while(coordIndex < allCoords.Count && numEntries < 2 )
            {
                if(allCoords[coordIndex].Equals(s))
                {
                    numEntries++;
                }
                
                coordIndex++;
            }
            if(numEntries > 1)
            {
                //Add to List if duplicate is found
                duplicateSubjunctions.Add(s);
            }
        }
        
        Debug.Log("all coords");
        Debug.Log(allCoords.Count);
        Debug.Log("distinct junctions hashset");
        Debug.Log(distinctSubjunctions.Count);
        Debug.Log("dupe junctions hashset");
        Debug.Log(duplicateSubjunctions.Count);
        
        
        
         */
        
        /*
        
        
        //Creates hash set based on geojson
        Debug.Log("all coords");
        Debug.Log(allCoords.Count);
        List<string>  distinctSubjunctions = new HashSet<string>(allCoords).ToList();
        Debug.Log("distinct junctions hashset");
        Debug.Log(distinctSubjunctions.Count);
        List<string> duplicatedValues = new List<string>();
        //Contains all values that have more than one entry
        for(int n1 = 0; n1 < distinctSubjunctions.Count; n1++)
        {
            for(int n2 = 0; n2 < )
            {

            }
            if(allCoords.FindAll(distinctSubjunctions[n1]).Count < 1)
            {
                duplicatedValues.Add(distinctSubjunctions[n1]);
            }
            n++;
        }
        
        Debug.Log("duplicated values");
        Debug.Log(duplicatedValues.Count);
        */

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
            instance.GetComponent<Section>().fromJunction = GameObject.Find(getGeoJson["features"][i]["fromjunc"].Value);
            instance.GetComponent<Section>().toJunction = GameObject.Find(getGeoJson["features"][i]["tojunc"].Value);

            /*
            foreach(string s in allSectionsPaths[i])    //for each string in section coords checks if string has appeared before. If found Connects the section to the existing section at coord
            {
                for(int x1 = 0; x1 < i; x1++)
                {
                    if(allSectionsPaths[x1].Contains(s))
                    {
                        instance.GetComponent<Section>().nextSection.Add(existingSections[x1]);
                        instance.GetComponent<Section>().nextSectionCoord.Add(s);
                        existingSectionsHasNext.Add(instance);
                    }
                }
            }
            
             */
            
            /*
            for(int x2 = 0; x2 < allSectionsPaths.Count; x2++)
            {
                foreach(string s in allSectionsPaths[x2])
                {
                    if(coordsList.Contains(s))
                    {
                        //Connect the section
                        //Finds which section path in allSectionsPaths has the section connected to it
                        for(int x1 = 0; x1 < allSectionsPaths.Count; x1++)
                        {
                            List<string> thisSectionPath = allSectionsPaths[x1];
                            if(thisSectionPath.Contains(s))
                            {
                                instance.GetComponent<Section>().nextSection.Add(existingSections[x1]);
                                instance.GetComponent<Section>().nextSectionCoord.Add(s);
                            }
                        }
                        coordsList.Add(s);
                    }
                }
            }
            
            
             */
            
            existingSections.Add(instance);
            
            /*
            if (i < subJunctionNames.Count)
            {
                instance.name = subJunctionNames[i];
            }
            */
            _spawnedObjects.Add(instance);
        }
        //Makes connections bi-directional
        foreach(GameObject thisGameObject in existingSectionsHasNext)
        {
            if(thisGameObject.GetComponent<Section>().nextSection != null)
            {
                int sectionNo = 0;
                foreach(GameObject sectionObject in thisGameObject.GetComponent<Section>().nextSection)
                {
                    sectionObject.GetComponent<Section>().nextSection.Add(thisGameObject);
                    sectionObject.GetComponent<Section>().nextSectionCoord.Add(thisGameObject.GetComponent<Section>().nextSectionCoord[sectionNo]);
                    sectionNo++;
                }
            }
        }
        zoom = _map.AbsoluteZoom;

        //thisMBLJS.sectionObjects = _spawnedObjects;
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

        //Debug.Log(Event.current.type);
    }

}