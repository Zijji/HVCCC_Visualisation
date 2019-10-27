using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using UnityEngine;
using Schemas;

/* Scripts that make trains move */
public class TrainMovement : MonoBehaviour
{
    //public GameObject stationDestionation;
    public float speed = 0.25f;
    public GameObject junctionDestination = null; //set this variable to be the next destination.
    public GameObject junctionPrev = null; //previous junction.
    public ConsistPath TrainPath = null;
    List<string> allSubJunctions;
    private GameObject getTimeObj;
    

    private int pathCurrentDest = 0;        //Index of current destination in path

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
                TimeTrainMovement(); 
            }
        }
        /**if (!(junctionDestination.Equals(TrainPath.GetJunction(TrainPath.Length() - 1))))
        {
            GetPathDest();
            TimeTrainMovement();
        } */
    }
    void Start()
    {
        allSubJunctions = MapboxCreateSubJunctions.allSubJunctions;
        
        //Finds time object
        getTimeObj = GameObject.Find("TimeObject");

        if (getTimeObj == null)
        {
            Debug.Log("Error: No time object found. Create an object called 'TimeObject' with the script 'TimeController.cs'");
        }

        pathCurrentDest = 1;
        GetPathDest();
        TimeTrainMovement();
    }

    //Movement taking into consideration time.
    private void TimeTrainMovement()
    {
        transform.LookAt(junctionDestination.transform.position, Vector3.up);
        float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
        float destTime = (TrainPath.GetArrivalTime(pathCurrentDest));
        float prevTime = (TrainPath.GetArrivalTime(pathCurrentDest - 1));

        transform.position = new Vector3(
            junctionDestination.transform.position.x -
            (junctionDestination.transform.position.x - junctionPrev.transform.position.x) *
            ((trainTime - prevTime) / (prevTime - destTime)),
            junctionDestination.transform.position.y -
            (junctionDestination.transform.position.y - junctionPrev.transform.position.y) *
            ((trainTime - prevTime) / (prevTime - destTime)),
            junctionDestination.transform.position.z -
            (junctionDestination.transform.position.z - junctionPrev.transform.position.z) *
            ((trainTime - prevTime) / (prevTime - destTime))
        );
    }

    
    public double checkDistance(float longitude, float latitude, float otherLongitude, float otherLatitude)
    {
        var d1 = latitude * (Math.PI / 180.0);
        var num1 = longitude * (Math.PI / 180.0);
        var d2 = otherLatitude * (Math.PI / 180.0);
        var num2 = otherLongitude * (Math.PI / 180.0) - num1;
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

        return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
    }

    //Gets destination from path. True if there is still more paths, false if there are no more.
    private bool GetPathDest()
    {
        int path = TrainPath.Length();
        
        //if (pathCurrentDest < path)
        //{
            float lat_current = (float) MapboxCreateJunctions._locations[pathCurrentDest].x;
            float lon_current = (float) MapboxCreateJunctions._locations[pathCurrentDest].y;
            
            float lat_prev = (float) MapboxCreateJunctions._locations[pathCurrentDest-1].x;
            float lon_prev = (float) MapboxCreateJunctions._locations[pathCurrentDest-1].y;

           // int indexLat = Math.Abs(MapboxCreateSubJunctions.allSubJunctionsLat.BinarySearch(lat_prev.ToString()));
           // int indexLon = Math.Abs(MapboxCreateSubJunctions.allSubJunctionsLat.BinarySearch(lat_prev.ToString()));
            
            //Debug.Log("index lat "+ indexLat + " index long " + indexLon);
            
            int index = Math.Abs(allSubJunctions.BinarySearch(lat_prev + "," + lon_prev));
            
            double distanceCurrentPrev = checkDistance(lon_current, lat_current, lon_prev, lat_prev);

            Debug.Log( "Found the index at " +  index + " which is " + " of length " + distanceCurrentPrev);
            
            String atIndex = allSubJunctions[index];
            float lat_atIndex = float.Parse(atIndex.Split(',')[0], CultureInfo.InvariantCulture);;
            float lon_atIndex = float.Parse(atIndex.Split(',')[1], CultureInfo.InvariantCulture);;
            
            String atIndexPrev = allSubJunctions[index - 1];
            float lat_atIndexPrev = float.Parse(atIndexPrev.Split(',')[0], CultureInfo.InvariantCulture);;
            float lon_atIndexPrev = float.Parse(atIndexPrev.Split(',')[1], CultureInfo.InvariantCulture);;
            
            Debug.Log("Checking the distance between " + atIndex + " to " + atIndexPrev);
            
            Debug.Log(("Check distance 1 " + checkDistance(lon_current, lat_current, lon_atIndex, lat_atIndex))  + " distnace =  "  + distanceCurrentPrev);
            Debug.Log(("Check distance 2 " + checkDistance(lon_current, lat_current, lon_atIndexPrev, lat_atIndexPrev))  + " distnace2 =  "  + distanceCurrentPrev);
            
            if ((checkDistance(lon_current, lat_current, lon_atIndex, lat_atIndex)) <= distanceCurrentPrev)
            {
                //Go to index
               junctionDestination = GameObject.Find("subJunction_" + (index+1));
               Debug.Log(" #1 Found the subjunction  at " + (index+1) + " called : " + junctionDestination);
               path++;

            } else if ((checkDistance(lon_current, lat_current, lon_atIndexPrev, lat_atIndexPrev)) < distanceCurrentPrev)
            {
                //Go to indexPrev
               junctionDestination = GameObject.Find("subJunction_" + (index-1));
               Debug.Log(" #2 Found the subjunction at " + (index-1) + " called : " + junctionDestination);
            }
            //No subjunctions can help
            else
            {
                Debug.Log("Routing in straight line to " + TrainPath.GetJunction(pathCurrentDest));
                junctionDestination = GameObject.Find(TrainPath.GetJunction(pathCurrentDest));
            }
            
            junctionPrev = GameObject.Find(TrainPath.GetJunction(pathCurrentDest - 1));

            if ((checkDistance(lon_current, lat_current, (float) MapboxCreateJunctions._locations[pathCurrentDest-1].x, (float) MapboxCreateJunctions._locations[pathCurrentDest-1].y)) < 50)
            {
                Debug.Log("Incremented the path " + pathCurrentDest + " trainlength = " + TrainPath.Length());
                
                
            }
            Debug.Log(" Path " + path + " pathCurrentDest " + pathCurrentDest);
            pathCurrentDest++;
            return true;
        //}
        //return false;
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
}