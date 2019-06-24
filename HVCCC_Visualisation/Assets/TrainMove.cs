using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Schemas;


public class TrainMove : MonoBehaviour
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
        Debug.Log("foo bar nathan");
        XMLHelper xml_helper = new XMLHelper();

        dataRailNetworkRailPlannerTimeTables timetables = xml_helper.getTimetable();

        string[] downTimetable = timetables.downTimetable[0].junctionIds.Split(' ');
        string[] upTimetable = timetables.upTimetable[0].junctionIds.Split(' ');

        LinkedList<PathCreator> pathObjects = new LinkedList<PathCreator>();

        for (int i = 0; i < xml_helper.getPaths().paths.Length; i++)
        {
            dataRailNetworkRailPlannerAllPathsPaths current = xml_helper.getPaths().paths[i];
            bool isUp = false;
            string[] currentTimetable;

            //Checks for direction up or down the Valley
            if (current.timetableId.Equals("downTimetable"))
            {
                isUp = false;
                currentTimetable = downTimetable;
            }
            else
            {
                isUp = true;
                currentTimetable = upTimetable;
            }

            //Checks for what days the path runs on
            string[] daysTravelsOnPath = current.daysOfWeek.Split(' ');
            string[] timeAtJunction = current.timeAtJunction.Split(' ');

            //Checks the number of days and creates a PathCreator object for each day.
            for (int k=0; k < daysTravelsOnPath.Length; k++)
            {
                PathCreator pathCreator = new PathCreator(isUp, (int) Convert.ToInt32(daysTravelsOnPath[k]));

                //Checks for timeAtJunction, if they are -1000, the train does not go there so the visit to that junction (in setDestination) is not added
                for (int j = 0; j < timeAtJunction.Length; j++)
                {
                    //string dest = "";
                    if (downTimetable[j].Equals("-1000"))
                    {
                        break;
                    }
                    else
                    {
                        pathCreator.setDestination(currentTimetable[j], timeAtJunction[j]);
                    }
                }
                Debug.Log(pathCreator.ToString());
                pathObjects.AddFirst(pathCreator);
            }

        }


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

        /*
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


public class PathCreator
{
    public bool isUp;
    private LinkedList<Destination> destinations = new LinkedList<Destination>();
    public int dayOfWeek;

    public PathCreator(bool isUp, int day)
    {
        this.isUp = isUp;
        dayOfWeek = day;
    }

    public void setIsUp(bool input)
    {
        isUp = input;
    }

    public void setDestination(string destination, string time)
    {
        Destination d = new Destination();
        d.addDestination(destination);
        d.addTime(time);
        destinations.AddFirst(d);
    }

    public LinkedList<Destination> getDesintations()
    {
        return destinations;
    }

    public void setDay(int day)
    {
        dayOfWeek = day;
    }

    public override string ToString()
    {
        return "New Path " + isUp + " " + dayOfWeek + " " + destinations.ToString();
    }
}


public class Destination
{
    public float time;
    public string junction;

    public Destination()
    {

    }

    public void addDestination(string dest)
    {
        this.junction = dest;
    }

    public void addTime(string time)
    {
        this.time = (float)Convert.ToDouble(time);
    }

    public override string ToString()
    {
        return "(" + time + " " + junction + ")";
    }
}
