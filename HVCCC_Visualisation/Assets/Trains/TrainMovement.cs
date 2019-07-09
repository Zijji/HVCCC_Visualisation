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
    
    // Update is called once per frame
    void Update()
    {

    }

    void Start()
    {
        print("Train created");

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
}
