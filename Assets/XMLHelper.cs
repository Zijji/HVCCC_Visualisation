//imports for xmlparser to work
using System.Xml;
using System.Xml.Serialization;
using Schemas;
using Schemas1;
using System;
using System.Collections.Generic;
using System.Collections;
//


public class XMLHelper
{
    public data model_inputs_data_object;
    public railLog rail_log;
    public bool file = false;
    public XMLHelper(){
        XmlSerializer ser = new XmlSerializer(typeof(data));
        if (FileManager.modelInputs != null)
        {
            using (XmlReader reader = XmlReader.Create(FileManager.modelInputs))
            {
                model_inputs_data_object = (data)ser.Deserialize(reader);
            }

            XmlSerializer ser1 = new XmlSerializer(typeof(railLog));
            using (XmlReader reader = XmlReader.Create(FileManager.railEventLogs))
            {
                rail_log = (railLog)ser1.Deserialize(reader);
            }
            file = true;
        }
        //else
        //{
        //    using (XmlReader reader = XmlReader.Create("modelInputs.xml"))
        //    {
        //        model_inputs_data_object = (data)ser.Deserialize(reader);
        //    }

        //    XmlSerializer ser1 = new XmlSerializer(typeof(railLog));
        //    using (XmlReader reader = XmlReader.Create("RailEventsLog.xml"))
        //    {
        //        rail_log = (railLog)ser1.Deserialize(reader);
        //    }
        //}
    }

    public dataRailNetworkJunctionsJunction[] getJunctions(){
        //Return the all the junctions as a list of junction objects
        return model_inputs_data_object.railNetwork[0].junctions[0].junction;
    }

    public dataRailNetworkJunctionsJunction getJunctionById(string junction_id){
        // given a jucntion id returns a junction object .
        //  If the jucntion id given does not correspond to a junction in the data throw a key not found exception
        for(int i = 0; i < getJunctions().Length; i++){
            if(getJunctions()[i].id.Equals(junction_id) ){
                return getJunctions()[i];
            }
        }
        // If the function has arrived here it means that no name matching the given id was founds. Produce a KeyNotFoundException
        throw new Exception(("The given junction id : " + junction_id + " does not correspond to any in the data file. Check that the junction_id is correct and that it there exists a junction with that as it's id"));
    }


    public dataRailNetworkTracksTrack[] getTracks(){
        // Return all the tracks as a list of Track objects
        return model_inputs_data_object.railNetwork[0].tracks[0].track;
    }

    public dataRailNetworkTracksTrack getTrackById(string track_id){
        // given a track id returns a junction object .
        //  If the jucntion id given does not correspond to a junction in the data throw a key not found exception
        for(int i = 0; i < getTracks().Length; i++){
            if(getTracks()[i].id.Equals(track_id) ){
                return getTracks()[i];
            }
        }
        // If the function has arrived here it means that no name matching the given id was founds. Produce a KeyNotFoundException
        throw new Exception(("The given track id : " + track_id + " does not correspond to any in the data file. Check that the track is is correct and that it there exists a track with that as it's id"));
    }

    public dataRailNetworkSectionsSection[] getSections(){
        // Return all the tracks as a list of Track objects
        return model_inputs_data_object.railNetwork[0].sections[0].section;
    }

    public dataRailNetworkSectionsSection getSectionById(string section_id){
        // given a track id returns a junction object .
        //  If the jucntion id given does not correspond to a junction in the data throw a key not found exception
        for(int i = 0; i < getSections().Length; i++){
            if(getSections()[i].id.Equals(section_id) ){
                return getSections()[i];
            }
        }
        // If the function has arrived here it means that no name matching the given id was founds. Produce a KeyNotFoundException
        throw new Exception(("The given track id : " + section_id + " does not correspond to any in the data file. Check that the track is is correct and that it there exists a track with that as it's id"));
    }

    public dataRailNetworkRailPlannerTimeTables getTimetable()
    {
        //return data_object.railNetwork[0].railPlanner[0].timeTables;
        return model_inputs_data_object.railNetwork[0].railPlanner[0].timeTables[0];
    }

    public dataRailNetworkRailPlannerAllPaths getPaths()
    {
        //return data_object.railNetwork[0].railPlanner[0].timeTables;
        return model_inputs_data_object.railNetwork[0].railPlanner[0].allPaths[0];
    }



//-----------------------------------------------------------------------------
//----- Functions below are used for RailLog
//-----------------------------------------------------------------------------
    public string[] getAllTrainIds()
    {
        //
        // Gets all the trains including the trains that do not exit the departure terminal
        //
        railLogRailEventsTrainCreated[]  trains_created = rail_log.railEvents[0].TrainCreated;
        string[] train_ids = new string[trains_created.Length];
        for (int i =0;i<trains_created.Length;i++ ){
            train_ids[i] = trains_created[i].trainID;
        }
        return train_ids;

    }


    public string[] getWorkingTrainIds()
    {
        //
        // Gets all the trains that leave the departure terminal
        //
        if (!file)
            return null;
        railLogRailEventsTrainCreated[]  trains_created = rail_log.railEvents[0].TrainCreated;
        List<string> train_ids = new List<string>();
        for (int i =0;i<trains_created.Length;i++ ){
            if(getReachedJunctionById(trains_created[i].trainID).Length>0){
                train_ids.Add(trains_created[i].trainID);
            }
        }
        return train_ids.ToArray();

    }

    public railLogRailEventsTrainCreated getTrainCreationById(string train_id){
        //
        // Gets a train creation event given it's id
        //

        railLogRailEventsTrainCreated[]  trains_created = rail_log.railEvents[0].TrainCreated;
        for (int i =0;i<trains_created.Length;i++ ){
            if (trains_created[i].trainID==train_id){
                    return trains_created[i];
                }
        }
        
        throw new Exception(("The given train id: " + train_id + " does not correspond to any in the data file. Check that the train_id is correct and that it there exists a train_id with that as it's id"));



    }
    public railLogRailEventsTrainReachedJunction[] getReachedJunctionById(String train_id){
        //
        // Get all the Reached junction events for a train
        //


        List<railLogRailEventsTrainReachedJunction> reached_junctions = new List<railLogRailEventsTrainReachedJunction>();
        for (int i =0;i<rail_log.railEvents[0].TrainReachedJunction.Length;i++)
        {
            if (rail_log.railEvents[0].TrainReachedJunction[i].trainID.Equals(train_id)){
                reached_junctions.Add(rail_log.railEvents[0].TrainReachedJunction[i]);
            }
            
        }
        return reached_junctions.ToArray();
    }
    public railLogRailEventsTrainPassedByJunction[] getPassedByJunctionById(String train_id){
        //
        // Get all the Passed junction events for a train
        //


        List<railLogRailEventsTrainPassedByJunction> passed_junctions = new List<railLogRailEventsTrainPassedByJunction>();
        for (int i =0;i<rail_log.railEvents[0].TrainPassedByJunction.Length;i++)
        {
            if (rail_log.railEvents[0].TrainPassedByJunction[i].trainID.Equals(train_id)){
                passed_junctions.Add(rail_log.railEvents[0].TrainPassedByJunction[i]);
            }
            
        }
        return passed_junctions.ToArray();
    }
    public Dictionary<string,List<string>> getTimeOrderedRouteById(string train_id){
        //
        // Gets all the information for a train route
        //Returns a dictionary of the form {"junctions":<list of junction names>, "reached":<list of times a train reach the junction> ,"passed":<list of times a train passed the junction}
        // A single index,i, corresponds to the all the information of trains and a junction: "junctions"][i] = junction name, "reached"][i]= time reached "junction"][i], "passed"][i]=time passed "junction"][i]
        //


        List<string> junction_list = new List<string>();
        List<string> reached_list = new List<string>();
        List<string> passed_list = new List<string>();
        // The first value(in each of the list,i.e., junction_list[0],reached_list[0],passed_list[0]) corresponds to the starting junctions of the list
        railLogRailEventsTrainCreated train_created = getTrainCreationById(train_id);
        junction_list.Add(train_created.junctionID);
        reached_list.Add(train_created.time);
        //reached_list.Add("0");

        railLogRailEventsTrainReachedJunction[] reached_junctions = getReachedJunctionById(train_id);
        //Array.Sort(reached_junctions,(x,y)=>x.time.CompareTo(y.time));
        railLogRailEventsTrainPassedByJunction[] passed_junctions = getPassedByJunctionById(train_id);
        //Array.Sort(passed_junctions,(x,y)=>x.time.CompareTo(y.time));
 
        for(int i = 0;i<reached_junctions.Length;i++){
            // Console.WriteLine("P "+passed_junctions[i].junctionID);
            // Console.WriteLine("P "+passed_junctions[i].time);

            // Console.WriteLine("R "+reached_junctions[i].junctionID);
            // Console.WriteLine("R "+reached_junctions[i].time);
            passed_list.Add(passed_junctions[i].time);
            reached_list.Add(reached_junctions[i].time);
            junction_list.Add(reached_junctions[i].junctionID);
        }

        //As when the train reachs it final junction it stops, the final passed value is irrelevent
        passed_list.Add("1000");
        Dictionary<string, List<string>> return_dict = new Dictionary<string, List<string>>(3);
        return_dict.Add("junctions",junction_list);
        return_dict.Add("reached",reached_list);
        return_dict.Add("passed",passed_list);

        return return_dict;
    }

} 
