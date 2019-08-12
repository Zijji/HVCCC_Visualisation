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
    public Path TrainPath = null;

    private GameObject getTimeObj;

    private int pathCurrentDest = 0;        //Index of current destination in path
    
    // Update is called once per frame
    void FixedUpdate()
    {
            BasicTrainMovement();
            if (GetNearDest())
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
        

    }

    void Start()
    {
        //Finds time object
        getTimeObj = GameObject.Find("TimeObject");
        if (getTimeObj == null)
        {
            Debug.Log("Error: No time object found. Create an object called 'TimeObject' with the script 'TimeController.cs'");
        }

        GetPathDest();
        TimeTrainMovement();
        //print("Train created");
        //print(TrainPath);
        
        // Example of trains moving around junctions that are next to it.
        /**transform.position = Vector3.MoveTowards(transform.position, junctionDestination.transform.position, speed * Time.deltaTime);
        //Vector3 relativePos = junctionDestination.transform.position - transform.position;
        //Quaternion trainDirection = Quaternion.LookRotation(relativePos, Vector3.up);
        //transform.rotation = trainDirection;
        var heading = junctionDestination.transform.position - transform.position;
        if (heading.magnitude < 0.1f)
        {
            //Gets random connected junction from junction
            string[] newJunction = junctionDestination.GetComponent<Junction>().connecting_junctions;
            junctionDestination = GameObject.Find(newJunction[Random.Range(0, newJunction.Length - 1)]);
           // public string[] connecting_junctions;
           //junctionDestination.GetComponent<TrainMove>().junctionDestination;
           //Gets random station to move towards
        }

        /* // Basic Example
        junctionDestination.GetComponent<Junction>();
        GameObject.Find("Hand");
        transform.position = Vector3.MoveTowards(transform.position, currentDestination.transform.position, speed * Time.deltaTime);
        var heading = currentDestination.transform.position - transform.position;
        if (heading.magnitude < 0.1f)
        {
            if (currentDestination != warabrookStation)
            {
                currentDestination = warabrookStation;
            }
            else
            {
                currentDestination = hexhamStation;
            }
        }
        */
    }

    private void BasicTrainMovement()
    {
        if(junctionDestination != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, junctionDestination.transform.position, speed * Time.deltaTime);
        }
        
    }
    //Movement taking into consideration time.
    private void TimeTrainMovement()
    {
        float distance = Vector3.Distance(junctionDestination.transform.position, transform.position);
        float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
        float dTime = TrainPath.GetTime(pathCurrentDest) - trainTime;
        
        speed = (distance / dTime);///Time.deltaTime;
        //TrainPath.GetJunction();
        //transform.position = Vector3.MoveTowards(transform.position, junctionDestination.transform.position, speed * Time.deltaTime);
    }
    //Gets destination from path. True if there is still more paths, false if there are no more.
    private bool GetPathDest()  
    {
        if (pathCurrentDest < TrainPath.Length())
        {
            junctionDestination = GameObject.Find(TrainPath.GetJunction(pathCurrentDest));
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
}
