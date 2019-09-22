using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Schemas;

/* Scripts that make trains move */
public class TrainMovement : MonoBehaviour
{
    //public GameObject stationDestionation;
    public float speed = 0.25f;
    public GameObject junctionDestination = null; //set this variable to be the next destination.
    public ConsistPath TrainPath = null;
    public Vector3 junctionPrevTransform;
    public GameObject leadTrain;
    private GameObject getTimeObj;

    private int pathCurrentDest = 0;        //Index of current destination in path
    
    // Update is called once per frame
    void FixedUpdate()
    {
            BasicTrainMovement();
            MapScale();
            float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
            //if (GetNearDest()
            //Now checks if time > departure time
            if (trainTime > TrainPath.GetDepartureTime(pathCurrentDest-1))
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

    private void BasicTrainMovement()
    {
        if(junctionDestination != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, junctionDestination.transform.position, speed * Time.deltaTime);
            transform.LookAt(junctionDestination.transform.position, Vector3.up);
        }
        
    }
    //Movement taking into consideration time.
    private void TimeTrainMovement()
    {
        float distance = Vector3.Distance(junctionDestination.transform.position, transform.position);
        float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
        float dTime = (TrainPath.GetArrivalTime(pathCurrentDest-1) - trainTime);
        //Debug.Log(distance);
        //Debug.Log(trainTime);
        //Debug.Log(dTime);
        
        
        speed = (distance / dTime)*getTimeObj.GetComponent<TimeController>().GetSpeed();///Time.deltaTime;
        //Debug.Log(speed);
        //TrainPath.GetJunction();
        //transform.position = Vector3.MoveTowards(transform.position, junctionDestination.transform.position, speed * Time.deltaTime);
    }
    //Gets destination from path. True if there is still more paths, false if there are no more.
    private bool GetPathDest()  
    {
        if (pathCurrentDest < TrainPath.Length())
        {
            junctionDestination = GameObject.Find(TrainPath.GetJunction(pathCurrentDest));
            junctionPrevTransform = junctionDestination.transform.position;
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

    private void MapScale()
    {
        
        if (junctionDestination != null)
        {
            if (junctionPrevTransform != junctionDestination.transform.position)
            {
                transform.position += (junctionDestination.transform.position - junctionPrevTransform);
            }
            junctionPrevTransform = junctionDestination.transform.position;
            TimeTrainMovement();
        }
        
    }
}
