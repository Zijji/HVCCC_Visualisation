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
    
    List<Path> paths = new List<Path>(); // Will contain all the trains that will need to be spawned at anytime.

    // Start is called before the first frame update
    void Start()
    {
        // Get input from XML
        XMLHelper xml_helper = new XMLHelper();

        // Getting the timetables from the XML
        dataRailNetworkRailPlannerTimeTables timetables = xml_helper.getTimetable();
        string[] downTimetable = timetables.downTimetable[0].junctionIds.Split(' ');
        string[] upTimetable = timetables.upTimetable[0].junctionIds.Split(' ');

        /* Retrieve all the paths from XML */
        int j;
        // for (int i = 0; i < xml_helper.getPaths().paths.Length; i++)
        for (int i = 0; i < 3; i++) 
        {
            dataRailNetworkRailPlannerAllPathsPaths current = xml_helper.getPaths().paths[i];

            var timetable = current.timetableId.Equals("downTimetable") ? downTimetable : upTimetable;
            var days = current.daysOfWeek.Split(' '); // Days travelled 1 day = 1440 minutes
            var minutes = current.timeAtJunction.Split(' '); // Times at the junctions minutes after midnight

            foreach (string d in days)
            {
                Path path = new Path();
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
        /* Create the trains */
        
        /* Check if it is time to dispatch the train */
        
        
        // First need to setup a global time in the visualization.
    }
}