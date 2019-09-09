/*
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
}