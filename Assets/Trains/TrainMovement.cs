using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using Schemas;

/* Scripts that make trains move */
public class TrainMovement : MonoBehaviour
{
    //public GameObject stationDestionation;
    public float speed = 0.25f;
    public GameObject junctionDestination = null; //set this variable to be the next destination.
    public GameObject junctionPrev = null; //previous junction.
    public ConsistPath TrainPath = null;
    public GameObject sectionCurrent = null; //Current section the train is travelling across.
    public int sectionPartCurrent = 0; //Current part of the section the train is travelling towards.
    public GameObject sectionPrev = null; //Previous section used to prevent the train immmediately travelling across a section it has already travelled across. Currently unused
    public float sectionTimeTotal = 0.0f; //Total time to navigate a section
    public List<float> sectionTime = new List<float>(); //The time that the train should be at each coord in the section

    private GameObject getTimeObj;

    private int pathCurrentDest = 0;        //Index of current destination in path

    [SerializeField]
    AbstractMap _map;   //Stores map for converting between lat/long and unity transform

    public float zoom;

    
    [SerializeField]
    float _spawnScale = 1.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        TimeTrainMovement();
        float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
        //if (GetNearDest()
        //Now checks if time > departure time
        if (trainTime > TrainPath.GetDepartureTime(pathCurrentDest - 1))
        {
            if (NoMorePath())
            {
                junctionDestination = null;
            }
            else
            {
                GetPathDest();
                TimeTrainMovement(); // Comment out causing: Index out of range error 0 might be too low
            }

        }


    }
    void Start()
    {
        _map = FindObjectOfType<AbstractMap>();
        //Finds time object
        getTimeObj = GameObject.Find("TimeObject");
        if (getTimeObj == null)
        {
            Debug.Log("Error: No time object found. Create an object called 'TimeObject' with the script 'TimeController.cs'");
        }

        zoom = _map.AbsoluteZoom;
        pathCurrentDest = 1;
        GetPathDest();
        TimeTrainMovement();
    }

    private void BasicTrainMovement()
    {
        if (junctionDestination != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, junctionDestination.transform.position, speed * Time.deltaTime);
            transform.LookAt(junctionDestination.transform.position, Vector3.up);
        }

    }
    //Movement taking into consideration time.
    private void TimeTrainMovement()
    {
        //Old way to do it - does not consider sections.
        transform.LookAt(junctionDestination.transform.position, Vector3.up);
        float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
        float destTime = (TrainPath.GetArrivalTime(pathCurrentDest));
        float prevTime = (TrainPath.GetArrivalTime(pathCurrentDest - 1));
        float distance = Vector3.Distance(junctionDestination.transform.position, junctionPrev.transform.position);
        //Goes straight to junction by default.
        transform.position = new Vector3(junctionDestination.transform.position.x - (junctionDestination.transform.position.x - junctionPrev.transform.position.x) * ((trainTime - prevTime) / (prevTime - destTime)),
                                    junctionDestination.transform.position.y - (junctionDestination.transform.position.y - junctionPrev.transform.position.y) * ((trainTime - prevTime) / (prevTime - destTime)),
                                    junctionDestination.transform.position.z - (junctionDestination.transform.position.z - junctionPrev.transform.position.z) * ((trainTime - prevTime) / (prevTime - destTime))
                                        );
        //Moves towards next portion of the section.
        /*
        if(sectionCurrent == null)
        {
            transform.LookAt(junctionDestination.transform.position, Vector3.up);
            float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
            float destTime = (TrainPath.GetArrivalTime(pathCurrentDest));
            float prevTime = (TrainPath.GetArrivalTime(pathCurrentDest - 1));
            float distance = Vector3.Distance(junctionDestination.transform.position, junctionPrev.transform.position);
            //Goes straight to junction by default.
            transform.position = new Vector3(junctionDestination.transform.position.x - (junctionDestination.transform.position.x - junctionPrev.transform.position.x) * ((trainTime - prevTime) / (prevTime - destTime)),
                                        junctionDestination.transform.position.y - (junctionDestination.transform.position.y - junctionPrev.transform.position.y) * ((trainTime - prevTime) / (prevTime - destTime)),
                                        junctionDestination.transform.position.z - (junctionDestination.transform.position.z - junctionPrev.transform.position.z) * ((trainTime - prevTime) / (prevTime - destTime))
                                            );
        }
        else
        {
            float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
            //Moves towards next coord in section
            //Calculates total distance from the start and end coords in the section
            float[] coordPosFloat = Array.ConvertAll((sectionCurrent.GetComponent<Section>().pathCoords[sectionPartCurrent]).Split(','), float.Parse);

            Vector3 getCoordPos = _map.GeoToWorldPosition(new Vector2(coordPosFloat[1], coordPosFloat[0]), true);
            Vector3 getCoordPosPrev = _map.GeoToWorldPosition(sectionCurrent.GetComponent<Section>().pathCoords[sectionPartCurrent-1], true);
            transform.position = new Vector3(getCoordPos.x - (getCoordPos.x - getCoordPosPrev.x) * ((trainTime- sectionTime[sectionPartCurrent-1]) / (sectionTime[sectionPartCurrent-1] - sectionTime[sectionPartCurrent] )),
                                        getCoordPos.y - (getCoordPos.y - getCoordPosPrev.y) * ((trainTime - sectionTime[sectionPartCurrent-1]) / (sectionTime[sectionPartCurrent-1] - sectionTime[sectionPartCurrent] )),
                                        getCoordPos.z - (getCoordPos.z - getCoordPosPrev.z) * ((trainTime - sectionTime[sectionPartCurrent-1]) / (sectionTime[sectionPartCurrent-1] - sectionTime[sectionPartCurrent] ))
                                            );
                                            
            if(trainTime > sectionTime[sectionPartCurrent] )
            {
                sectionPartCurrent++;
            }
            //TODO: When train gets to path coord, moves on to next
        }
        
        
         */
        
        
    }
    //Gets destination from path. True if there is still more paths, false if there are no more.
    private bool GetPathDest()
    {
        if (pathCurrentDest < TrainPath.Length())
        {
            junctionDestination = GameObject.Find(TrainPath.GetJunction(pathCurrentDest));
            junctionPrev = GameObject.Find(TrainPath.GetJunction(pathCurrentDest - 1));
            //Finds the next section from the current junction.
            //This part should be expanded to do a breadth-search for junctions that could be in the network. 
            //Also should consider looping sections.
            sectionCurrent = null;
            for(int i = 0; i < junctionPrev.GetComponent<Junction>().junctionNeighbour.Count; i++)
            {
                if(junctionDestination == junctionPrev.GetComponent<Junction>().junctionNeighbour[i])
                {
                    sectionCurrent = junctionPrev.GetComponent<Junction>().junctionNeighbourSections[i];
                }
                
            }
            if(sectionCurrent != null)
            {
                //Calculates time to navigate each portion of the section.
                
                //float sectionAccumulatedTime = getTimeObj.GetComponent<TimeController>().GetTime();   //Accumulated time used in for loop
                float sectionAccumulatedTime = TrainPath.GetDepartureTime(pathCurrentDest - 1);
                //Calculates total distance from the start and end coords in the section
                float[] sectionPartCoordsStart = Array.ConvertAll((sectionCurrent.GetComponent<Section>().pathCoords[0]).Split(','), float.Parse);
                float[] sectionPartCoordsEnd = Array.ConvertAll((sectionCurrent.GetComponent<Section>().pathCoords[sectionCurrent.GetComponent<Section>().pathCoords.Length-1]).Split(','), float.Parse);

                //calculates total distance first
                float totalDistance = 0;
                for(int i = 0; i < sectionCurrent.GetComponent<Section>().pathCoords.Length-1; i++)
                {
                    float[] sectionPartCoordsPrev = Array.ConvertAll((sectionCurrent.GetComponent<Section>().pathCoords[i]).Split(','), float.Parse);
                    float[] sectionPartCoordsNext = Array.ConvertAll((sectionCurrent.GetComponent<Section>().pathCoords[i+1]).Split(','), float.Parse);

                    totalDistance += Vector2.Distance(new Vector2(sectionPartCoordsPrev[0],sectionPartCoordsPrev[1]), new Vector2(sectionPartCoordsNext[0],sectionPartCoordsNext [1]));
                }
                //Calculates times the train should arrive at each part.
                sectionTimeTotal = TrainPath.GetArrivalTime(pathCurrentDest) - TrainPath.GetDepartureTime(pathCurrentDest - 1);
                sectionTime.Add(TrainPath.GetDepartureTime(pathCurrentDest - 1));        //Adds starting time.
                for(int i = 0; i < sectionCurrent.GetComponent<Section>().pathCoords.Length-1; i++)
                {
                    float[] sectionPartCoordsPrev = Array.ConvertAll((sectionCurrent.GetComponent<Section>().pathCoords[i]).Split(','), float.Parse);
                    float[] sectionPartCoordsNext = Array.ConvertAll((sectionCurrent.GetComponent<Section>().pathCoords[i+1]).Split(','), float.Parse);

                    sectionAccumulatedTime += sectionTimeTotal*(Vector2.Distance(new Vector2(sectionPartCoordsPrev[0],sectionPartCoordsPrev[1]), new Vector2(sectionPartCoordsNext[0],sectionPartCoordsNext [1]))/totalDistance);
                    //Debug.Log((Vector2.Distance(new Vector2(sectionPartCoordsPrev[0],sectionPartCoordsPrev[1]), new Vector2(sectionPartCoordsNext[0],sectionPartCoordsNext [1]))/totalDistance).ToString()+"%");
                    //sectionTime.Add(sectionAccumulatedTime);
                    sectionTime.Add(sectionAccumulatedTime);
                    //This is What time the train should appear at pathCoord +1. i.e. sectionTime[x] in train corresponds to pathCoord[x+1] in section.
                }
                
            }
            

            sectionPartCurrent = 0;

            pathCurrentDest++;
            return true;
        }
        return false;
        //TrainPath.GetTime(pathCurrentDest);
    }
    //Are there no more paths? true = no more paths; false = more paths
    private bool NoMorePath()
    {
        if (pathCurrentDest < TrainPath.Length())
        {
            return false;
        }
        return true;
        //TrainPath.GetTime(pathCurrentDest);
    }
    //Returns true if distance to destination is <0.1f, false otherwise
    private bool GetNearDest()
    {
        if (junctionDestination != null)
        {
            var heading = junctionDestination.transform.position - transform.position;
            if (heading.magnitude < 0.1f)
            {
                return true;
            }
        }
        return false;
    }

    /*
    
    void Update()
    {
        if (zoom != _map.AbsoluteZoom)
        {
            transform.localPosition = _map.GeoToWorldPosition(location, true);
            transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            zoom = _map.AbsoluteZoom;
        }
    }
    
    void OnGUI()
    {
        if (Event.current.type == EventType.ScrollWheel || Event.current.type == EventType.MouseDrag || 
            Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown)
        {
            transform.localPosition = _map.GeoToWorldPosition(location, true);
            transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
        }
    
//        Debug.Log(Event.current.type);
    }
    */
}