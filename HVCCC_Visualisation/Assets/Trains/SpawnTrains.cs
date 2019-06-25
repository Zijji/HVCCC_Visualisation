using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schemas;
using System;
using System.Text;

public class SpawnTrains : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get input from XML
        XMLHelper xml_helper = new XMLHelper();
        LinkedList<string> paths = new LinkedList<string>();
        
        // Getting the timetables from the XML
        dataRailNetworkRailPlannerTimeTables timetables = xml_helper.getTimetable();

        string[] downTimetable = timetables.downTimetable[0].junctionIds.Split(' ');
        string[] upTimetable = timetables.upTimetable[0].junctionIds.Split(' ');
        
        string[] timetable; // references if the direciton is up or down the valley
        string[] days; // Days travelled 1 day = 1440 minutes
        string[] minutes; // Times at the junctions minutes after midnight
        StringBuilder sb = new StringBuilder();

        // Retreive all the paths from XML
        for (int i = 0; i < xml_helper.getPaths().paths.Length; i++)
        {
            dataRailNetworkRailPlannerAllPathsPaths current = xml_helper.getPaths().paths[i];

            if (current.timetableId.Equals("downTimetable"))
            {
                timetable = downTimetable;
            }
            else
            {
                timetable = upTimetable;
            }

            days = current.daysOfWeek.Split(' ');
            minutes = current.timeAtJunction.Split(' ');

            foreach (string temp in minutes)
            {
                print(temp);
            }

            // Probably using StringBuilder to build before adding to the data structure called paths.

            foreach (string d in days)
            {
                sb.Clear(); // Clear the string builder for a new path

                foreach (string m in minutes)
                {
                    if (m.Equals("-1000"))
                    {
                        print("Case being ignored because j = 10000");
                        break; // Just ignore this junction it is not visited path (or the train) for this instance
                    }
                    else
                    {
                        /* Add a new destination for the path. */
                        sb.Append() // Currently appending nothing but should look into StringBuilder.AppendFormat()
//        https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder?view=netframework-4.8#methods
                    }
                }
                
                // Insert into data structure called paths
            }

            // Checks the number of days and creates a PathCreator object for each day.
//            for (int k = 0; k < daysTravelsOnPath.Length; k++)
//            {
//                PathCreator pathCreator = new PathCreator(isUp, (int) Convert.ToInt32(daysTravelsOnPath[k]));
//
//                //Checks for timeAtJunction, if they are -1000, the train does not go there so the visit to that junction (in setDestination) is not added
//                for (int j = 0; j < timeAtJunction.Length; j++)
//                {
//                    //string dest = "";
//                    if (downTimetable[j].Equals("-1000"))
//                    {
//                        break;
//                    }
//                    else
//                    {
//                        pathCreator.setDestination(currentTimetable[j], timeAtJunction[j]);
//                    }
//                }
//
//                print(pathCreator.ToString());
//                pathObjects.AddFirst(pathCreator);
//            }
        }
    }
}