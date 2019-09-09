<<<<<<< HEAD
﻿/*
 * Author: Nathan Ebba
 * Date: 28/06/2019
 * GitHub: ebbanat@gmail.com | c3236497@uon.edu.au
 */

using System.Collections.Generic;
using UnityEngine;
using Schemas;
using System;

public class SpawnTrains : MonoBehaviour
{
    public float prevTime;
    public GameObject Train;
    private GameObject getTimeObj;


    List<ConsistPath> paths = new List<ConsistPath>(); // Will contain all the trains that will need to be spawned at anytime.
    List<int> pathsChecked = new List<int>(); // Contains whether the paths have been spawned (0 = not spawned yet; 1 = spawned)
    //Made an int list in case we need to expand it out later.

    // Start is called before the first frame update
    void Start()
    {
        //Finds time object
        getTimeObj = GameObject.Find("TimeObject");
        if(getTimeObj == null)
        {
            Debug.Log("Error: No time object found. Create an object called 'TimeObject' with the script 'TimeController.cs'");
        }

        // Get input from XML
        XMLHelper xml_helper = new XMLHelper();

        // Getting the timetables from the XML
        dataRailNetworkRailPlannerTimeTables timetables = xml_helper.getTimetable();
        string[] downTimetable = timetables.downTimetable[0].junctionIds.Split(' ');
        string[] upTimetable = timetables.upTimetable[0].junctionIds.Split(' ');

        /* Retrieve all the paths from XML */
        int j;
        for (int i = 0; i < xml_helper.getPaths().paths.Length; i++)
        //for (int i = 0; i < 3; i++) 
        {
            dataRailNetworkRailPlannerAllPathsPaths current = xml_helper.getPaths().paths[i];

            var timetable = current.timetableId.Equals("downTimetable") ? downTimetable : upTimetable;
            var days = current.daysOfWeek.Split(' '); // Days travelled 1 day = 1440 minutes
            var minutes = current.timeAtJunction.Split(' '); // Times at the junctions minutes after midnight

            foreach (string d in days)
            {
                ConsistPath path = new ConsistPath();
                j = 0;
                foreach (string m in minutes)
                {
                    if (m.Equals("-1000"))
                    {
                        // print("ID: " + i + " case being ignored. " + i + d + m + j);
                    }
                    else
                    {
                        /* Add a new destination for the path. */
                        var time = Convert.ToInt32(d) * 1440 + Convert.ToInt32(m);
                        //print("ID: " + i + " New destination: (" + timetable[j] + " " + time + ")");
                        //paths.Add(new Destinations {junction = timetable[j], time = time});
                        path.AddDestination(timetable[j], time);
                    }
                    j++;
                }
                paths.Add(path);
                pathsChecked.Add(0);
            }
        }
        
        print("Input order: ");
        foreach (var p in paths)
        {
            print(p);
        }

        paths.Sort(); // Sorts the collection based on the CompareTo() -- which sorts it based the minimum time on the time.

        print("Sorted order: ");
        foreach (var p in paths)
        {
            print(p);
        }

        /*
         * Should make a dispatcher that creates trains at the correct time
         * Will probably be the function that runs at every frame or something more frequent.
         * because this one just runs at the start but this is where everything should get created.
         */
    }

    void Update()
    {
        float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
        //Uses in built time variable, may need to place elsewhere for rewind/fast forward capability
        if (trainTime >= Mathf.Ceil(prevTime))
        {
            prevTime = trainTime;
            Debug.Log(trainTime);
            //Debug.Log(paths[0]);
            //Debug.Log("POP:   " + paths[0].PopJunction());
            //Debug.Log(paths[0].GetTime(0));

            for (int p = 0; p < paths.Count; p++ )
            {
                if ((trainTime >= ((float)paths[p].GetTime(0))) && (pathsChecked[p] == 0))
                {
                    pathsChecked[p] = 1;
                    //String nameJunction = paths[0].PopJunction();
                    GameObject thisJunction = GameObject.Find(paths[p].GetJunction(0));
                    GameObject newTrain = Instantiate(Train, thisJunction.transform.position, thisJunction.transform.rotation);
                    newTrain.GetComponent<TrainMovement>().TrainPath = paths[p];
                    Debug.Log("true");
                }
            }
            /*
            if (trainTime >= ((float)paths[0].GetTime(0)))
            {
                String nameJunction = paths[0].PopJunction();
                GameObject thisJunction = GameObject.Find(paths[0].GetJunction(0));
                GameObject newTrain = Instantiate(Train, thisJunction.transform.position, thisJunction.transform.rotation);
                newTrain.GetComponent<TrainMovement>().TrainPath = paths[0];
                Debug.Log("true");
            }
            else
            {
                Debug.Log("false");
            }
            */
            /*
            if (trainTime >= ( (float)paths[0].GetTime(0)))
            {
                String nameJunction = paths[0].PopJunction();
                GameObject thisJunction = GameObject.Find(nameJunction);
                GameObject nextJunction = GameObject.Find(paths[0].GetJunction(0));
                GameObject newTrain = Instantiate(Train, thisJunction.transform.position, thisJunction.transform.rotation);
                newTrain.GetComponent<TrainMovement>().junctionDestination = nextJunction;
                Debug.Log("true");
            }
            else
            {
                Debug.Log("false");
            }
             * */
        }
        /* Create the trains */
        /*
        Debug.Log(paths[0].GetTime(0));
        if ((trainTime >= paths[0].GetTime(0)) && (trainMake = false))
        {
            trainMake = true;
            GameObject thisJunction = GameObject.Find(paths[0].GetJunction(0));
            GameObject nextJunction = GameObject.Find(paths[0].GetJunction(1));
            GameObject newTrain = Instantiate(Train, thisJunction.transform.position, thisJunction.transform.rotation);
            newTrain.GetComponent<TrainMovement>().junctionDestination = nextJunction;

            Debug.Log("true");
        }
        else
        {
            Debug.Log("false");
        }
         * */
        /*
        if (paths[0].GetTime(0))
        Debug.Log(paths[0].GetJunction(0));
        Debug.Log(paths[0].GetTime(0));
        */

        /* Check if it is time to dispatch the train */


        // First need to setup a global time in the visualization.


    }
=======
﻿/*
 * Author: Nathan Ebba
 * Date: 28/06/2019
 * GitHub: ebbanat@gmail.com | c3236497@uon.edu.au
 */

using System.Collections.Generic;
using UnityEngine;
using Schemas;
using System;

public class SpawnTrains : MonoBehaviour
{
    public float prevTime;
    public GameObject Train;
    private GameObject getTimeObj;
    private int trainNo = 0;


    List<ConsistPath> paths = new List<ConsistPath>(); // Will contain all the trains that will need to be spawned at anytime.
    List<int> pathsChecked = new List<int>(); // Contains whether the paths have been spawned (0 = not spawned yet; 1 = spawned)
    //Made an int list in case we need to expand it out later.

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("is using new Spawntrains");
        //Finds time object
        getTimeObj = GameObject.Find("TimeObject");
        if(getTimeObj == null)
        {
            Debug.Log("Error: No time object found. Create an object called 'TimeObject' with the script 'TimeController.cs'");
        }

        // Get input from XML
        XMLHelper xml_helper = new XMLHelper();
        string[] train_ids = xml_helper.getWorkingTrainIds();
        Debug.Log("working trains " + train_ids.Length);
        for (int i = 0; i < train_ids.Length; i++)
        {
            Dictionary<string, List<string>> xml_train_paths_dic = xml_helper.getTimeOrderedRouteById(train_ids[i]);
            ConsistPath path = new ConsistPath();

            for (int j = 0; j < xml_train_paths_dic["passed"].Count; j++)
            {


                path.AddDestination(xml_train_paths_dic["junctions"][j], float.Parse(xml_train_paths_dic["reached"][j]), float.Parse(xml_train_paths_dic["passed"][j]));

            

            }
            paths.Add(path);
            pathsChecked.Add(0);
        }
        
        print("Input order: ");
        foreach (var p in paths)
        {
            print(p);
        }

        paths.Sort(); // Sorts the collection based on the CompareTo() -- which sorts it based the minimum time on the time.

        print("Sorted order: ");
        foreach (var p in paths)
        {
            print(p);
        }

        /*
         * Should make a dispatcher that creates trains at the correct time
         * Will probably be the function that runs at every frame or something more frequent.
         * because this one just runs at the start but this is where everything should get created.
         */
    }

    void FixedUpdate()
    {
        float trainTime = getTimeObj.GetComponent<TimeController>().GetTime();
        //Uses in built time variable, may need to place elsewhere for rewind/fast forward capability
        if (trainTime >= prevTime)
        {
            prevTime = trainTime;

            for (int p = 0; p < paths.Count; p++ )
            {
                if ((trainTime >= paths[p].GetDepartureTime(0)) & (pathsChecked[p] == 0))
                {
                    pathsChecked[p] = 1;
                    GameObject thisJunction = GameObject.Find(paths[p].GetJunction(0));
                    GameObject newTrain = Instantiate(Train, thisJunction.transform.position, thisJunction.transform.rotation);
                    newTrain.name = "Train" + trainNo;
                    trainNo++;
                    newTrain.GetComponent<TrainMovement>().TrainPath = paths[p];
                }
            }
        }


    }
>>>>>>> adding-ui-to-raileventlogs
}