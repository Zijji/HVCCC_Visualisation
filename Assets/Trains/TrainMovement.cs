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
    public GameObject superSectionPrefab = null;       //Prefabs for 'supersections', sections that combine many sections together in one big section.
    public GameObject sectionPrev = null; //Previous section. Used to prevent loops.
    public GameObject superSection = null;       //Assigned when a super section is created. After it's used, it's destroyed.
    public GameObject sectionCurrent = null; //Current section the train is travelling across.
    public int sectionPartCurrent = 0; //Current part of the section the train is travelling towards.
    public float sectionTimeTotal = 0.0f; //Total time to navigate a section
    public List<float> sectionTime = new List<float>(); //The time that the train should be at each coord in the section
    public string[] sectionCoords;

    public bool nomoretrack;
    public bool stopTrain = false;
    private GameObject getTimeObj;

    private int pathCurrentDest = 0;        //Index of current destination in path

    [SerializeField]
    AbstractMap _map;   //Stores map for converting between lat/long and unity transform

    public float zoom;

    

    [SerializeField]
    float _spawnScale = 1.0f;

    // Update is called once per frame
    void LateUpdate()
    {
        nomoretrack = NoMorePath();
        if(!NoMorePath())
        {
            if(pathCurrentDest == 0)
            {
                GetPathDest();
            }
            TimeTrainMovement();
            float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
            //if (GetNearDest()
            //Now checks if time > departure time
            if( TrainPath != null)
            {
                if (trainTime > TrainPath.GetDepartureTime(pathCurrentDest))
                {
                    if (NoMorePath())
                    {
                        junctionDestination = null;
                    }
                    else
                    {
                        GetPathDest();
                    }

                }
            }
            
        }
        else
        {
            transform.position = new Vector3(junctionDestination.transform.position.x,
                                        junctionDestination.transform.position.y,
                                        junctionDestination.transform.position.z
                                            );
            //Destroy(gameObject);
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
        pathCurrentDest = 0;
    }
    //Movement taking into consideration time.
    private void TimeTrainMovement()
    {
        /*
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
        */
        //Moves towards next portion of the section.
        if(sectionCurrent == null)
        {
            //transform.LookAt(junctionDestination.transform.position, Vector3.up);
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
            /*
            float[] coordPosFloat = Array.ConvertAll((sectionCurrent.GetComponent<Section>().pathCoords[sectionPartCurrent+1]).Split(','), float.Parse);
            Debug.Log("coordPosFloat");
            Debug.Log(coordPosFloat[1]);
            Debug.Log(coordPosFloat[0]);
            
             */
            Vector2d _location = Conversions.StringToLatLon(sectionCoords[sectionPartCurrent]);
            Vector3 getCoordPos = _map.GeoToWorldPosition(_location, true);
            /*
            Debug.Log("CoordsNext");
            Debug.Log(getCoordPos);
            
             */
            //coordPosFloat = Array.ConvertAll((sectionCurrent.GetComponent<Section>().pathCoords[sectionPartCurrent]).Split(','), float.Parse);
            //Vector3 getCoordPosPrev = _map.GeoToWorldPosition(new Vector2d(coordPosFloat[1], coordPosFloat[0]), true);

            _location = Conversions.StringToLatLon(sectionCoords[sectionPartCurrent-1]);
            Vector3 getCoordPosPrev = _map.GeoToWorldPosition(_location, true);

            /*
            Debug.Log("CoordsPrev");
            Debug.Log(getCoordPosPrev);
            
             */
            transform.position = Vector3.Lerp(getCoordPosPrev, getCoordPos, ((trainTime - sectionTime[sectionPartCurrent-1]) / (sectionTime[sectionPartCurrent] - sectionTime[sectionPartCurrent-1])));
            //First part of the section has the train look at the point directly; from then on, uses slerp to rotate.
            if(sectionPartCurrent <= 1)
            {
                transform.LookAt(getCoordPos, Vector3.up);
            }
            else
            {
                Vector3 posVector = (getCoordPos - transform.position);
                if(posVector != Vector3.zero)
                {
                    Quaternion rotationQuaternion = Quaternion.LookRotation(posVector);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotationQuaternion, ((trainTime - sectionTime[sectionPartCurrent - 1]) / (sectionTime[sectionPartCurrent] - sectionTime[sectionPartCurrent - 1])));
                }
                
                
            }
            
            //Debug.Log(((trainTime - sectionTime[sectionPartCurrent-1]) / (sectionTime[sectionPartCurrent-1] - sectionTime[sectionPartCurrent] )));
            /*
            transform.position = new Vector3(getCoordPos.x - (getCoordPos.x - getCoordPosPrev.x) * ((trainTime- sectionTime[sectionPartCurrent]) / (sectionTime[sectionPartCurrent] - sectionTime[sectionPartCurrent+1] )),
                                        getCoordPos.y - (getCoordPos.y - getCoordPosPrev.y) * ((trainTime - sectionTime[sectionPartCurrent]) / (sectionTime[sectionPartCurrent] - sectionTime[sectionPartCurrent+1] )),
                                        getCoordPos.z - (getCoordPos.z - getCoordPosPrev.z) * ((trainTime - sectionTime[sectionPartCurrent]) / (sectionTime[sectionPartCurrent] - sectionTime[sectionPartCurrent+1] ))
                                        );
             */
            
            if(trainTime > sectionTime[sectionPartCurrent] )
            {
                if(sectionPartCurrent < sectionCoords.Length-1)
                {
                    sectionPartCurrent++;
                }
                //Going to the next junction is handled in the update() event
            }
            
            //TODO: When train gets to path coord, moves on to next
        }
        
        
    }
    //Gets destination from path. True if there is still more paths, false if there are no more.
    private bool GetPathDest()
    {
                
        if (pathCurrentDest < TrainPath.Length())
        {
            pathCurrentDest++;
            junctionDestination = GameObject.Find(TrainPath.GetJunction(pathCurrentDest));
        
            //Checks if the junction has sections connected to it. If not, disables train movement.
            if(junctionDestination.GetComponent<Junction>().junctionNeighbour.Count <= 0 )
            {
                junctionDestination = GameObject.Find(TrainPath.GetJunction(pathCurrentDest-1));
                junctionPrev = GameObject.Find(TrainPath.GetJunction(pathCurrentDest-2));
                TrainPath = null;
                stopTrain = true;
                return false;
            }
            junctionPrev = GameObject.Find(TrainPath.GetJunction(pathCurrentDest - 1));
            //Finds the next section from the current junction.
            //This part should be expanded to do a breadth-search for junctions that could be in the network. 
            //Also should consider looping sections.
            if(superSection != null)
            {
                Destroy(superSection);
                superSection = null;
            }
            bool sectionFound = false;
            for(int i = 0; i < junctionPrev.GetComponent<Junction>().junctionNeighbour.Count; i++)
            {
                if(junctionDestination == junctionPrev.GetComponent<Junction>().junctionNeighbour[i]
                && junctionPrev.GetComponent<Junction>().junctionNeighbourSections[i] != sectionCurrent)
                {
                    sectionPrev = sectionCurrent;
                    sectionCurrent = junctionPrev.GetComponent<Junction>().junctionNeighbourSections[i];
                    sectionFound = true;
                    i = junctionPrev.GetComponent<Junction>().junctionNeighbour.Count;
                }
            }
            if(sectionFound == false )
            {
                sectionCurrent = null;
                
            }
            //Determines route of junction based on breadth-first search, if the section is not a direct neighbour.
            if(sectionCurrent == null)
            {
                //List<List<GameObject>> bfsSectionPathList = new List<List<GameObject>>();   //List of all section paths
                List<GameObject> junctionsSearched = new List<GameObject>();    //all junctions searched in the bfs
                List<GameObject> junctionsFound = new List<GameObject>();    //junctions found but not searched
                junctionsFound.Add(junctionPrev);
                int currentJunctionIndex = 0;
                GameObject currentJunction = junctionsFound[currentJunctionIndex];
                junctionsSearched.Add(currentJunction);
                
                bool found = false;
                while(!found)
                {
                    //Adds nearby junctions to searched junctions until the end junction is found.
                    for(int i = 0; i < currentJunction.GetComponent<Junction>().junctionNeighbour.Count; i++)
                    {
                        if(junctionDestination == currentJunction.GetComponent<Junction>().junctionNeighbour[i])
                        {
                            //Avoids adding a junction if it uses a section that was previously used.
                            if(!junctionsSearched.Contains(currentJunction.GetComponent<Junction>().junctionNeighbour[i]))
                            {
                                junctionsSearched.Add(currentJunction.GetComponent<Junction>().junctionNeighbour[i]);
                            }
                            found = true;
                        }
                        else
                        {
                            if(!junctionsFound.Contains(currentJunction.GetComponent<Junction>().junctionNeighbour[i])
                            && sectionPrev != currentJunction.GetComponent<Junction>().junctionNeighbourSections[i])
                            {
                                junctionsFound.Add(currentJunction.GetComponent<Junction>().junctionNeighbour[i]);
                            }
                        }
                    }
                    if(!found)
                    {
                        currentJunctionIndex = 0;
                        Debug.Log("COUNT " + junctionsFound.Count);
                        while(junctionsSearched.Contains(junctionsFound[currentJunctionIndex]))
                        {
                            Debug.Log("Current index = "+currentJunctionIndex);
                            currentJunctionIndex++;
                            
                        }
                        currentJunction = junctionsFound[currentJunctionIndex];
                        junctionsSearched.Add(currentJunction);
                        
                    }
                }
               
                //All searched junctions (including start and end junctions) are stored in junctionsSearched
                //Does a DFS to find all the sections to that junction.
                List<GameObject> dfsJunctionPath = new List<GameObject>();       //List of the 'true' path containing all the sections up to the junction
                dfsJunctionPath.Add(junctionPrev);
                bool jpfound = false;
                int junctionsFoundIndex = 0;
                int loop = 0;
                while(!jpfound && loop < 100)
                {
                    bool addedJunction = false;
                    for(int i = 0; i < dfsJunctionPath[junctionsFoundIndex].GetComponent<Junction>().junctionNeighbour.Count; i++)
                    {
                        if(junctionsSearched.Contains(dfsJunctionPath[junctionsFoundIndex].GetComponent<Junction>().junctionNeighbour[i]) 
                        && !dfsJunctionPath.Contains(dfsJunctionPath[junctionsFoundIndex].GetComponent<Junction>().junctionNeighbour[i])
                        && dfsJunctionPath[junctionsFoundIndex].GetComponent<Junction>().junctionNeighbourSections[i] != sectionPrev
                        )
                        {
                            dfsJunctionPath.Add(dfsJunctionPath[junctionsFoundIndex].GetComponent<Junction>().junctionNeighbour[i]);  //Finds junction path
                            i = dfsJunctionPath[junctionsFoundIndex].GetComponent<Junction>().junctionNeighbour.Count;   //Ends loop
                            junctionsFoundIndex++;
                            addedJunction = true;
                        }
                    }
                    if(addedJunction == false)
                    {
                        junctionsSearched.Remove(dfsJunctionPath[junctionsFoundIndex]);
                        dfsJunctionPath.Remove(dfsJunctionPath[junctionsFoundIndex]);
                        junctionsFoundIndex--;
                    }
                    if(dfsJunctionPath.Contains(junctionDestination))
                    {
                        jpfound = true;
                    }
                    //Debug.Log("Path List:");
                    //for(int i = 0; i < dfsJunctionPath.Count; i++)
                    //{
                    //    Debug.Log(dfsJunctionPath[i].name);
                    //}
                }
                if(loop >= 100)     //Haven't tested this with a larger network. Will remove when first route is implemented
                {
                    Debug.Log("Error! Endless while loop found.");
                    Debug.Log("Path List:");
                    for(int i = 0; i < dfsJunctionPath.Count; i++)
                    {
                        Debug.Log(dfsJunctionPath[i].name);
                    }
                    Debug.Log("Path Count:");
                    Debug.Log(dfsJunctionPath.Count);
                    //Debug.Break();
                }
                //Iterates through sections in junction path list to create one 'super' section that implements all sections.
                List<String> supsecPathCoord = new List<String>();
                for(int i = 0; i < dfsJunctionPath.Count-1; i++)
                {
                    GameObject foundSection = null;
                    for(int i1 = 0; i1 < dfsJunctionPath.Count; i1++)
                    {
                        if(dfsJunctionPath[i].GetComponent<Junction>().junctionNeighbour[i1] == dfsJunctionPath[i+1])
                        {
                            foundSection = dfsJunctionPath[i].GetComponent<Junction>().junctionNeighbourSections[i1];
                            
                            i1 = dfsJunctionPath.Count;
                        }
                    }
                    if(dfsJunctionPath[i] == foundSection.GetComponent<Section>().toJunction)
                    {
                        //Special case for if this is the first section - adds the start.
                        if(i == 0)
                        {
                            supsecPathCoord.Add(foundSection.GetComponent<Section>().pathCoords[foundSection.GetComponent<Section>().pathCoords.Length-1]);
                        }
                        //Reverses input of foundSection.
                        for(int i1 = foundSection.GetComponent<Section>().pathCoords.Length-2; i1 >= 0; i1--)  //Excludes the 'start' of the section.
                        {
                            supsecPathCoord.Add(foundSection.GetComponent<Section>().pathCoords[i1]);
                        }
                    }
                    else
                    {
                        //Special case for if this is the first section - adds the start.
                        if(i == 0)
                        {
                            supsecPathCoord.Add(foundSection.GetComponent<Section>().pathCoords[0]);
                        }
                        for(int i1 = 1; i1 < foundSection.GetComponent<Section>().pathCoords.Length; i1++)  //Excludes the 'start' of the section.
                        {
                            supsecPathCoord.Add(foundSection.GetComponent<Section>().pathCoords[i1]);
                        }
                    }
                    //Assigns the last section as section prev
                    if(foundSection.GetComponent<Section>().toJunction == junctionDestination
                    || foundSection.GetComponent<Section>().fromJunction == junctionDestination)
                    {
                        sectionPrev = foundSection;

                    }
                    
                }
                superSection = Instantiate(superSectionPrefab, transform.position, Quaternion.identity);
                superSection.GetComponent<Section>().fromJunction = junctionPrev;
                superSection.name = this.name + "Section";
                superSection.GetComponent<Section>().toJunction = junctionDestination;
                superSection.GetComponent<Section>().pathCoords = supsecPathCoord.ToArray();
                superSection.transform.parent = this.transform;     //While this means the section will 'move' with the train, the section's pathcoords remain independant of movement

                
                /*
                for(int i = 0; i < superSection.GetComponent<Section>().pathCoords.Length; i++)
                {
                    Debug.Log(superSection.GetComponent<Section>().pathCoords[i]);
                }
                Debug.Break();
                
                 */
                sectionCurrent = superSection;
                /*
                //Start at junction junctionPrev
                bfsJunctionPath.Add(junctionPrev);

                bool foundRoute = false;
                while(!foundRoute)
                {
                    //Navigates from start junction to dead ends. Removes dead ends from junctionsSearched until a route to the end junction is found.
                    for(int i = 0; i < junctionsSearched.Count; i++)
                    {
                        if(junctionsSearched.contains(bfsSectionPath[bfsSectionPath.Count - 1].junctionNeighbour[0]))
                        {
                            
                        }
                        bfsSectionPath.Add();
                        Debug.Log(junctionsFound[i].name);
                    }
                }
                 */
                
                


                //Creates a 'supersection' that combines the sections between start and end junctions. (Make sure sections are 'linked' right, so it doesn't accidentally follow backwards or something)

                //Sets this supersection as the section to follow



                /*
                for(int i = 0; i < junctionsSearched.Count; i++)
                {

                    junctionNeighbour++;
                }
                 */
                
                /*
                //Enclose this in some kinda for loop
                //List<GameObject> bfsSectionPath = new List<GameObject>();
                bool found = false;
                while(!found)
                {
                    for(int i = 0; i < junctionPrev.GetComponent<Junction>().junctionNeighbour[i])
                    {
                        bfsSectionPathList[i][0].Add(junctionPrev.GetComponent<Junction>().junctionNeighbourSections[i]);
                    }
                }
                 */
                
                

                /*
                List<List<GameObject>> bfsJunctionPathList = new List<List<GameObject>>();   //List of all section paths
                List<GameObject> bfsSectionPath = new List<GameObject>();                  //List of one particular section path
                for(int i = 0; i < junctionPrev.GetComponent<Junction>().junctionNeighbour.Count; i++)
                {
                    bfsSectionPath.Add(junctionPrev.GetComponent<Junction>().junctionNeighbourSections[i]);
                }
                 */
                
                /*
                List<GameObject> bfsSectionPath = new List<GameObject>();                  //List of one particular section path

                //Creates section paths based on neighbours
                for(int i = 0; i < junctionPrev.GetComponent<Junction>().junctionNeighbour.Count; i++)
                {
                    bfsSectionPath.Add(junctionPrev.GetComponent<Junction>().junctionNeighbourSections[i]);
                }
                 */
                
                

                //Doesn't save the route - this will be recalculated every time a junction is reached
            }
            sectionCoords = sectionCurrent.GetComponent<Section>().pathCoords;

            //reverses direction if the to- and from- junctions don't match the section
            
            if(!(junctionDestination == sectionCurrent.GetComponent<Section>().toJunction))
            {
                System.Array.Reverse(sectionCoords);
            }
            
            if(sectionCurrent != null)
            {
                //Calculates time to navigate each portion of the section.
                
                //Clears list for new section
                sectionTime = new List<float>();
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
            

            sectionPartCurrent = 1; //Has to start at ==1 otherwise it starts moving towards the junction instead of the first point of the junction.

            
            return true;
        }
        return false;
        //TrainPath.GetTime(pathCurrentDest);
    }
    //Are there no more paths? true = no more paths; false = more paths
    private bool NoMorePath()
    {
        if(stopTrain == true)
        {
            return true;
        }
        if (pathCurrentDest < TrainPath.Length()-1)
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