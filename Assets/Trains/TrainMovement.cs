using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Schemas;

/* Scripts that make trains move */
public class TrainMovement : MonoBehaviour
{
    //public GameObject stationDestionation;
    public float speed = 1.0f;
    public GameObject junctionDestination = null; //set this variable to be the next destination.
    public GameObject junctionPrev = null; //previous junction.
    public ConsistPath TrainPath = null;

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
        if (junctionDestination != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, junctionDestination.transform.position, speed * Time.deltaTime);
            transform.LookAt(junctionDestination.transform.position, Vector3.up);
        }

    }
    //Movement taking into consideration time.
    private void TimeTrainMovement()
    {
        transform.LookAt(junctionDestination.transform.position, Vector3.up);
        float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
        float destTime = (TrainPath.GetArrivalTime(pathCurrentDest));
        float prevTime = (TrainPath.GetArrivalTime(pathCurrentDest - 1));
        float distance = Vector3.Distance(junctionDestination.transform.position, junctionPrev.transform.position);

        //transform.position = junctionPrev.transform.position;


        /*
        transform.position = new Vector3(junctionPrev.transform.position.x + (junctionDestination.transform.position.x - junctionPrev.transform.position.x) *0,
                                        junctionPrev.transform.position.y + (junctionDestination.transform.position.y - junctionPrev.transform.position.y) *0,
                                        junctionPrev.transform.position.z + (junctionDestination.transform.position.z - junctionPrev.transform.position.z) *0
                                            );
         */
        /*
        transform.position = new Vector3(junctionPrev.transform.position.x + (junctionDestination.transform.position.x - junctionPrev.transform.position.x) * ((trainTime - prevTime) / (prevTime - destTime)),
                                        junctionPrev.transform.position.y + (junctionDestination.transform.position.y - junctionPrev.transform.position.y) * ((trainTime - prevTime) / (prevTime - destTime)),
                                        junctionPrev.transform.position.z + (junctionDestination.transform.position.z - junctionPrev.transform.position.z) * ((trainTime - prevTime) / (prevTime - destTime))
                                            );
         */
        transform.position = new Vector3(junctionDestination.transform.position.x - (junctionDestination.transform.position.x - junctionPrev.transform.position.x) * ((trainTime - prevTime) / (prevTime - destTime)),
                                        junctionDestination.transform.position.y - (junctionDestination.transform.position.y - junctionPrev.transform.position.y) * ((trainTime - prevTime) / (prevTime - destTime)),
                                        junctionDestination.transform.position.z - (junctionDestination.transform.position.z - junctionPrev.transform.position.z) * ((trainTime - prevTime) / (prevTime - destTime))
                                            );
        //junctionPrev.transform.position * distance*(destTime - trainTime)/(prevTime - trainTime);


        /*
        float distance = Vector3.Distance(junctionDestination.transform.position, transform.position);
        float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
        float dTime = (TrainPath.GetArrivalTime(pathCurrentDest-1) - trainTime);
        */
        //Debug.Log(distance);
        //Debug.Log(trainTime);
        //Debug.Log(dTime);
        //Places train where it should be according to time.


        //speed = (distance / dTime)*getTimeObj.GetComponent<TimeController>().GetSpeed();///Time.deltaTime;
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
            junctionPrev = GameObject.Find(TrainPath.GetJunction(pathCurrentDest - 1));
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