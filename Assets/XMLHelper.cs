//imports for xmlparser to work
using System.Xml;
using System.Xml.Serialization;
using Schemas;
using System.Linq;
using System.IO;
using Schemas1;
using System;
using Tracks;
using System.Collections.Generic;


public class XMLHelper{
    // 
    public class Graph{


        //Value to set the raduis for the region of the junction to be assigned to points
        public double assigning_radius_scale_factor = 0.1f;

        //Stores the coordinates of junction gotten from csv
        public Dictionary<string,List<double>> junction_coords = new Dictionary<string,List<double>>();

        //Stores a list of points who have been assigned to a junctions
        public List<string> assigned_points = new List<string>();

        //Stores a list of points that are used in a trains path
        public List<string> used_junctions = new List<string>();

        //Stores what junction an assigned vertice corresponds to.
        public Dictionary<string, string> point_to_junction = new Dictionary<string,string>();

        //Stores what points are assigned to what junction. Used for beginning a path
        public Dictionary<string, List<string>> junction_to_point= new Dictionary<string,List<string>>();

        //Stores the connections of the graph: basic undirected graph data structure
        public Dictionary<string, List<string>> points =new Dictionary<string, List<string>>();

        // Class to store each [x,y] coordinate representing a location of a track as a graph object,
        public Graph(List<List<List<double>>> input,List<string> input_used_junctions){
            used_junctions = input_used_junctions;
            //1. going through each 
            //Console.WriteLine("Gets to graph constructure");
            foreach (List<List<double>> track in input){
                //2. going through each [x,y] pont in the track
                string prev_key = "";
                foreach (List<double> point in track){

                    //3. Logic for building each node of the graph. Note that it is a directed graph so we have to record the forward pass of the track, and the backward this is 
                    string next_key = (point[0].ToString()+":"+point[1].ToString());

                    //3.1 if current point is the first in the current track
                    if(prev_key==""){

                        //Console.WriteLine("First key in track " + next_key);
                        //3.1.1 if the key is the first key in the current track and is already registered inthe points dict: do nothing
                        if (points.ContainsKey(next_key)){
                              //prev_key=next_key
                                //Console.WriteLine("points already has " + next_key);
                            }

                        //3.1.2 If the key is the first key in the current track but is not registered in the points dict: add it to dict
                        else{
                            //Console.WriteLine("points doesnt have " + next_key+ "; adding it now");
                            points.Add(next_key, new List<string>());
                            //prev_key=next_key
                        }
                    }
                    //3.2 If not first point in track
                    else{

                        //Console.WriteLine("Not first point in track: Prev " + prev_key + " and next(Current) "+ next_key);
                        //3.2.1 if next_key (current point) is already registered in the points dict: do nothing
                        if(points.ContainsKey(next_key)){
                            //Console.WriteLine( next_key +" Key already in graph");
                            //points[prev_key].add(next_key)
                            //prev_key = next_key
                        }

                        //3.2.2 if next_key (current point) is not registered in the points dict: add next_key to points
                        else{
                            //Console.WriteLine( next_key +" Not in graph; adding it");
                            points.Add(next_key,new List<string>());
                            //points[prev_key].add(next_key)
                            //prev_key = next_key
                        }

                        //3.2.3 Connect the prev_key(prev node) to next_key(next node). Note nodes mean the [x,y] coords of rail locational data
                        //Add a connection from prev_key to next_key and then one from next_key to prev_key
                        points[prev_key].Add(next_key);
                        points[next_key].Add(prev_key);
                    }
                    prev_key = next_key;
                }
            }
            assignJunctions();
        }

        // public List<string> getUsedJunctions(){
        //     List<string> ret_list = new List<string>();
        //     for (int i =0;i<rail_log.railEvents[0].TrainPassedByJunction.Length;i++)
        //     {
        //         ret_list.Add(rail_log.railEvents[0].TrainPassedByJunction[i].junctionID);
        //     }
        //     ret_list = ret_list.Distinct().ToList();
        //     return ret_list;
        // }
        //Corresponds a point to a junction
        public void assignJunctions(){
            //1. read in the junction coords from the csv and populate the junction coords dictionary
            initJunctionCoords();
            Console.WriteLine("after initjunctions");

            //2. Assigning Points to the junctions
            List<string> assigned_junctions = new List<string>();
            List<string> unassigned_junctions = new List<string>();
            foreach(var junction in junction_coords){
                bool junctionAssignedAtLeastOne = false;
                
                //distance to closest junction
                double closest_junction_dist = distanceToClosestJunction(junction.Key);

                //creating a value in the junction_to_point dictionary
                junction_to_point.Add(junction.Key,new List<string>());

                foreach(var point in points){

                    // if less than the assigning_radius distance than assign current point to current junction
                    if (distanceBetweenTwoPoints(junction.Value,pointStringIdToDoubles(point.Key))<(closest_junction_dist*assigning_radius_scale_factor)){

                            //Case 1: point is already assigned a junction: raise error
                        if( assigned_points.Contains(point.Key)){
                                  Console.WriteLine("Error: Two junctions assigned to a single point");
                        }
                        //Case 2: assign to point 
                        else{
                            assigned_points.Add(point.Key);
                            point_to_junction.Add(point.Key,junction.Key);
                            junctionAssignedAtLeastOne=true;

                            junction_to_point[junction.Key].Add(point.Key);
                        }
                        
                    }
                }
                if (junctionAssignedAtLeastOne==false){
                    //Console.WriteLine("Junction is not assigned to atleast one point");
                    unassigned_junctions.Add(junction.Key);
                }
                else{
                    assigned_junctions.Add(junction.Key);
                }


            }
            Console.WriteLine("assigned Junctions");
            assigned_junctions.ForEach(Console.WriteLine);
            Console.WriteLine("unassigned Junctions");
            unassigned_junctions.ForEach(Console.WriteLine);

        }
        // Returns a list of the next available subjunction given current; prev is for direction and removes prev from potential next nodes
        public double distanceToClosestJunction(string junction_id){
            double min_dist = 1000000000;
            foreach(var junction in junction_coords){
                //if the currenct junction isnt the same as the junction id
                if (junction.Key.Equals(junction_id)==false){
                    double temp_dist = distanceBetweenTwoPoints(junction_coords[junction_id],junction.Value);

                    if(temp_dist<min_dist){
                        min_dist = temp_dist;
                    }
                }
            }
            return min_dist;

        }
        public List<string> getNext(string current, string prev){
            List<string> potential_next = points[current];
            if (string.IsNullOrEmpty(prev)){
                return potential_next;
            }
            potential_next.Remove(prev);
            return potential_next;
        }

        //Returns true if the given point is assigned to a junctions, or false if it is not. assigned points are stored in the assigned_points list
        public bool isPointAssigned(string point_string){
            return assigned_points.Contains(point_string);
        }

        //Reads the locations of all the junctions from the csv file and stores them in junction_coords dictionary
        public void initJunctionCoords(){

            string junction_coords_filename = "Junctions_Coordinates.csv";
            using(var reader = new StreamReader(@junction_coords_filename))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    // if ((values[0].Equals("_id")==false)&&(used_junctions.Contains(values[0]))){
                    if (values[0].Equals("_id")==false){
                      string junction_name = values[0];
                      // Console.WriteLine(junction_name);
                      // Console.WriteLine(values[2]);
                      // Console.WriteLine(values[3]);
                      string string_coords = values[3]+":"+values[2];
                      string_coords = string_coords.Replace(" ","");
                      string_coords = string_coords.Replace("\"","");
                      //Console.WriteLine(string_coords);
                      List<double> coords = pointStringIdToDoubles(string_coords);
                      // Console.WriteLine("junction_name is above");
                      junction_coords.Add(junction_name,coords);

                  }
            }
            // foreach(var item in junction_coords)
            // {
            //     Console.WriteLine(item.Key);
            //     string lat = junction_coords[item.Key][0].ToString();
            //     string lon = junction_coords[item.Key][1].ToString();
            //     Console.WriteLine(lat+","+lon);

            // }
        }

      }

// Below are helper functions for the graph class

        // Returns a string "151.7297:-32.87208"version of coordinates to a list of doubles [x,y]
        public List<double> pointStringIdToDoubles(String string_version){
            // Console.WriteLine(string_version);
            // String x_s = string_version.Split(':')[0];
            // String y_s = string_version.Split(':')[1];
            // Console.WriteLine(x_s);
            // Console.WriteLine(y_s);

            double x = double.Parse(string_version.Split(':')[0]);//long
            double y = double.Parse(string_version.Split(':')[1]);//lat
            List<double> ret_list = new List<double>();
            ret_list.Add(y);//lat is first
            ret_list.Add(x);//long is second
            return  ret_list;
        }

        //Return distance between two points given point 1 is a list of the form [x,y] and [x,y]
        public double distanceBetweenTwoPoints(List<double> p1, List<double> p2){
            // p1 and p2 are of the form [lat,long]
            //Using the haversine formula to calculate the distance between two points(google it )

            //1. coverting all the points into radians
            double lat1 = (double) p1[0]*(Math.PI/180);
            double lon1 = (double) p1[1]*(Math.PI/180);
            double lat2 = (double) p2[0]*(Math.PI/180);
            double lon2 = (double) p2[1]*(Math.PI/180);

            //2. haversine formula
            double dlon = lon2-lon1;
            double dlat = lat2-lat1;
            double a = Math.Pow(Math.Sin(dlat/2),2) +Math.Cos(lat1)*Math.Cos(lat2)*Math.Pow(Math.Sin(dlon/2),2);
            double c = 2*Math.Asin(Math.Sqrt(a));
            double r = 6371.0;

            // Console.WriteLine("Haversine");
            // Console.WriteLine("a " + a.ToString());
            // Console.WriteLine("c " + c.ToString());
            double final_dis = (double) (c*r);
            return final_dis;

        }
        //
        public double distanceBetweenTwoPoints(string p1_string, string p2_string){
            //Given an input of string representing two points, return a list of doubles of each
            List<double> p1 = pointStringIdToDoubles(p1_string);
            List<double> p2 = pointStringIdToDoubles(p2_string);
            Console.WriteLine("P1:"+p1_string);
            Console.WriteLine(p1[0].ToString());
            Console.WriteLine(p1[1].ToString());
            Console.WriteLine("P2:"+p2_string);
            Console.WriteLine(p2[0].ToString());
            Console.WriteLine(p2[1].ToString());


            return distanceBetweenTwoPoints(p1,p2);
        }


    }


    public Graph graph;
    public Dictionary<string,List<List<string>>> train_routes_dict = new Dictionary<string,List<List<string>>>();
    public data model_inputs_data_object;
    public railLog rail_log;
    public kml kml_tracks;
    public XMLHelper(){
        XmlSerializer ser = new XmlSerializer(typeof(data));
        using (XmlReader reader = XmlReader.Create("modelInputs.xml"))
        {
            model_inputs_data_object = (data) ser.Deserialize(reader);
        }

        XmlSerializer ser1 = new XmlSerializer(typeof(railLog));
        using (XmlReader reader = XmlReader.Create("RailEventsLog.xml"))
        {
            rail_log= (railLog) ser1.Deserialize(reader);
        }

        XmlSerializer ser2 = new XmlSerializer(typeof(kml));
        using (XmlReader reader = XmlReader.Create("hunter_valley_tracks.xml"))
        {
            kml_tracks = (kml) ser2.Deserialize(reader);

        }

        

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
    public railLogRailEventsTrainReachedJunction[] getReachedJunctionById(string train_id){
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
    public railLogRailEventsTrainPassedByJunction[] getPassedByJunctionById(string train_id){
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

    public List<string> getUsedJunctions(){
        List<string> ret_list = new List<string>();
        for (int i =0;i<rail_log.railEvents[0].TrainPassedByJunction.Length;i++)
        {
            ret_list.Add(rail_log.railEvents[0].TrainPassedByJunction[i].junctionID);
        }
        Console.WriteLine("\n\n\n\n\nUsed Junctions");
        ret_list = ret_list.Distinct().ToList();
        ret_list.ForEach(Console.WriteLine);
        Console.WriteLine("\n\n\n\n\n");
        return ret_list;
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


        railLogRailEventsTrainReachedJunction[] reached_junctions = getReachedJunctionById(train_id);
        Array.Sort(reached_junctions,(x,y)=>x.time.CompareTo(y.time));
        Array.Sort(reached_junctions,(x,y)=>x.time.CompareTo(y.time));
        railLogRailEventsTrainPassedByJunction[] passed_junctions = getPassedByJunctionById(train_id);
        Array.Sort(passed_junctions,(x,y)=>x.time.CompareTo(y.time));
 
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
    //-----------------------------------------------------------------------------
    //----- Functions below are used for RailLog
    //-----------------------------------------------------------------------------
    
    //----- Returns a list of coordinates for a track of the form [[x1,y1],[x2,y2],[x3,y3]]
    public List<List<List<double>>> getAllTrackCoords(){
        kmlDocumentFolderPlacemark[] placemarks = getAllTrackObjects();
        // Console.WriteLine(placemarks.Length);
        List<List<List<double>>> track_coords_list= new List<List<List<double>>>();
        //Console.WriteLine(placemarks[0].MultiGeometry[0].Linestring[0].coordinates.Split(','));
        
        // for(int i = 0;i<placemarks.Length;i++){
        for(int i = 0;i<placemarks.Length;i++){

            // Console.WriteLine(i);
            string[] temp_coords_vals = placemarks[i].MultiGeometry[0].LineString[0].coordinates.Split(' ');
            List<List<double>> temp_coords = new List<List<double>>();
            // Console.WriteLine(temp_coords_vals);
            for(int j = 0;j<temp_coords_vals.Length;j++){
                double temp_x = double.Parse(temp_coords_vals[j].Split(',')[0]);
                double temp_y = double.Parse(temp_coords_vals[j].Split(',')[1]);

                // double temp_x_2 = double.Parse(temp_coords_vals[j].Split(',')[0]);
                // double temp_y_2 = double.Parse(temp_coords_vals[j].Split(',')[1]);

                // Console.WriteLine(temp_coords_vals[j]);
                // Console.WriteLine(temp_x.ToString());
                // Console.WriteLine(temp_x_2.ToString());
                // Console.WriteLine(temp_y.ToString());
                // Console.WriteLine(temp_y_2.ToString());
                List<double> temp_coord = new List<double>();
                temp_coord.Add(temp_x);
                temp_coord.Add(temp_y);
                temp_coords.Add(temp_coord);
                // track_coord_list[i].Add(temp_coord);

            }
            track_coords_list.Add(temp_coords);
        }
        return track_coords_list;
        // Console.WriteLine("Testing the first track" + kml_tracks.Items[0].Folder[0].Placemark[0].MultiGeometry[0].LineString[0].coordinates);

        // Console.WriteLine("Testing the first track" + kml_tracks.Items[0].Folder[0].Placemark[10].MultiGeometry[0].LineString[0].coordinates);
        
    }
    //----- Returns a list of the tracks in their raw form as Placemarks
    public kmlDocumentFolderPlacemark[] getAllTrackObjects(){
        
        return kml_tracks.Items[0].Folder[0].Placemark;
        // Console.WriteLine("Testing the first track" + kml_tracks.Items[0].Folder[0].Placemark[10].MultiGeometry[0].LineString[0].coordinates);
    }
    //-----------------------------------------------------------------------------
    //----- Functinos below are used for generating the routes
    //-----------------------------------------------------------------------------
    public string getAllRoutePaths(){
        graph = new Graph(getAllTrackCoords(),getUsedJunctions());



        int testing_option = 4;
        if(testing_option==4){
            getRoutePathForTrain("train0");
            foreach(var in_route in train_routes_dict["train0"] ){
                in_route.ForEach(Console.WriteLine);
                
            }

            return "";
        }

        if(testing_option==1){
          string test_prev = "151.728:-32.8715";
          string test_current = "151.7288:-32.87183";
          //next should be 
          string test_next = "151.7297:-32.87208";
          Console.WriteLine("prev:"+test_prev+";current"+test_current);
          List<string> test_get_next_output= graph.getNext(test_current,test_prev);
          //List<string> test_get_next_output= g.getNext(test_current,"");
          test_get_next_output.ForEach(Console.WriteLine);
          double dist = graph.distanceBetweenTwoPoints(test_prev,test_current);
          Console.WriteLine("dist: "+dist.ToString());


          //List<string> test_get_next_output= g.getNext(test_current,"");

        }

        if(testing_option==2){
                //Testing the haversine
            List<double>  p1 = new List<double>(new double[]{-32.8715f,151.728f});
            List<double>  p2 = new List<double>(new double[]{-32.87183f,151.7288f});
            List<double>  p3 = new List<double>(new double[]{-32.87208f,151.7297f});
            Console.WriteLine("testing distance with haversine formulat");
            double dist = graph.distanceBetweenTwoPoints(p1,p2);
            Console.WriteLine("dist: "+dist.ToString());
        }

        if(testing_option==3)
        {
            Dictionary<string,List<string>> train0_route = getTimeOrderedRouteById("train0");
            Console.WriteLine("train0 route");
            for(int i=0;i<train0_route["junctions"].Count;i++){
                Console.WriteLine(train0_route["junctions"][i]);
            }
        }
        return "hello";
    }

    // public List<double> getRoutePathForTrain(string train_id){
    public void getRoutePathForTrain(string train_id){
        // getting a list of junctions representing the route of a train
        Console.WriteLine("\n\n\n");
        List<string> route = getTimeOrderedRouteById(train_id)["junctions"];
        //Console.WriteLine("Routing for "+train_id);
        //route.ForEach(Console.WriteLine);
        // List<string> test_route = new List<string>{ "jn_NCIG","jn_K9"};
        List<string> test_route = new List<string>{ "jn_K17","jn_NCIG"};
        // route=test_route;

        Console.WriteLine("test junctions");
        foreach(string test_junction in test_route){
            Console.WriteLine(test_junction);
            graph.junction_to_point[test_junction].ForEach(Console.WriteLine);
        }
        //getting all the starting points to begin looking for routes
        route.ForEach(Console.WriteLine);
        List<String> starting_points = graph.junction_to_point[route[0]];
        Console.WriteLine("Starting Points for " +train_id);


        starting_points.ForEach(Console.WriteLine);

        //creating an entry for this traind in the train_routes_dict
        train_routes_dict.Add(train_id,new List<List<string>>());

        // foreach(string sp in starting_points){
        //     Console.WriteLine("\n\n\n\nNew Starting Point");
        //     findPath(new List<string>(){sp}, route.ToList(),"",train_id,new List<string>(){sp});
        // }
        findPath(new List<string>(){starting_points[0]}, route.ToList(),"",train_id,new List<string>(){starting_points[0]});


    }

    public void findPath(List<string> path,List<string> route, string prev_junction, string train_id,List<string> seen_till_next_junction){
        Console.WriteLine("\n\n\nFINDPATH");

        string prev_point = "";
        string curr_point = path[path.Count-1];

        if (path.Count<2){
            prev_point = "";
        }
        else{
            prev_point = path[(path.Count-2)];
            //prev_point = path[1];
        }


        List<String> next_points = graph.getNext(curr_point,prev_point);

        //First junction in route and point in route
        Console.WriteLine("Points");
        Console.WriteLine("Curr:"+curr_point+";Prev:"+prev_point);
        Console.WriteLine("Path");
        path.ForEach(Console.WriteLine);
        Console.WriteLine("Next Points");
        next_points.ForEach(Console.WriteLine);
        Console.WriteLine("Junction");
        Console.WriteLine("Prev"+prev_junction);
        Console.WriteLine("Route");
        route.ForEach(Console.WriteLine);
        Console.WriteLine("\n");


        //SUCCESS-Current path is valid
        if(route.Count<1){
            Console.WriteLine("SUCCESS");
            //1. End of track, no more values in next:Finish
            if(next_points.Count<1){
                Console.WriteLine("1. End of a track or current point not associated with prev junction");
                path.Add(curr_point);
                train_routes_dict[train_id].Add(path);
                return;
            }
            //2. Curr is not associated with a junction:Finish
            else if(graph.isPointAssigned(curr_point)==false){
                Console.WriteLine("2. Curr is not associated with a junction");
                train_routes_dict[train_id].Add(path);
                return;
            }
            //3. Curr is associated with a junction
            else if(graph.isPointAssigned(curr_point)){
                Console.WriteLine("3. Curr is assocaited with a junction");

                //3.1 Curr is assocaite with a junction that is prev_junction:Continue
                if(graph.point_to_junction[curr_point].Equals(prev_junction)){
                    Console.WriteLine("3.1 Curr is associated with prev junction");
                    //continue
                    }
                
                //3.2 Curr is assoaciate with a junction that is not prev_junction:FInish
                else{
                    Console.WriteLine("3.2 Curr is associated with a junction that isn't prev Junction");
                    train_routes_dict[train_id].Add(path);
                    return;
                }

            }
        }

        //Failure- Current Path is invalid
        //Failure if point already seen
        // else if(path.Contains(curr_point)){
        //     Console.WriteLine("FAILURE");
        //     Console.WriteLine("F0. point already in path");
        //     return;
        // }
        //1. If at end of track(next is 0) and routes>0:Finish
        else if((route.Count>0)&&(next_points.Count<1)){
            Console.WriteLine("FAILURE");
            Console.WriteLine("F1. End of track and route isn't empty");
            return;
        }

        //2. If associated with a junction
        else if(graph.isPointAssigned(curr_point)){
            Console.WriteLine("FAILURE");
            Console.WriteLine("F2. Associated with a junction");

            //2.1 If that junction is prev_junction:Continue
            if(graph.point_to_junction[curr_point].Equals(prev_junction)){
                Console.WriteLine("F2.1 If junction is prev junction");
            }


            //2.2 If that junction is next junction to visit(route[0])
            else if(graph.point_to_junction[curr_point].Equals(route[0])){
                Console.WriteLine("F2.2 If junction is next junction");
            }

            //2.3 If that junction is neither
            else{
                Console.WriteLine("F2.3 If junction is neither next or prev junction");
                Console.WriteLine(graph.point_to_junction[curr_point]);
                return;
            }
        }

        //Continue-Path is valid, but cannot be finished
        Console.WriteLine("CONTINUE");

        //1. If point assocaitead with junction
        if (graph.isPointAssigned(curr_point)){
            Console.WriteLine("C1. Point associated with junction");
            //1.1 if Point is prev point:continue
            if (graph.point_to_junction[curr_point].Equals(prev_junction)){
                Console.WriteLine("C1.1 Curr Point is prev junction");
            }
            //1.2 if point is next point: Do things
            else{
                Console.WriteLine("C1.2 Associated Junction is next");
                //1.2.1 make prev_junction this the next junction(route[0])
                Console.WriteLine("Gets to prev junction");
                prev_junction = route[0];
                //1.2.2 make remove the first junction from the route list(route[0])
                Console.WriteLine("Gets to remove from route junction");
                route.RemoveAt(0);
                Console.WriteLine("Complete Remove at");
                seen_till_next_junction = new List<string>();
            }
        }

        //2. If point is not assocaiated with a junction: Continue
        else{
            Console.WriteLine("2. Curr_point is not associated with a junction");
        }


        //Moving on to next points

        //2. Go to all the next points
        foreach(string next_point in next_points)
        {
            //Copy the lists so they are copies and not references
            //List<string> temp_path = path.GetClone();
            // List<string> temp_path = new List<string>();
            // foreach(string p_point in path){
            //     temp_path.Add(p_point);
            // }


            //Only go to the next point if it hasn't already been visited
            Console.WriteLine("before seen till loop");
            if(seen_till_next_junction.Contains(next_point)==false){

                Console.WriteLine("In stl");
            
                List<string> temp_path = path.ToList();
                List<string> temp_route = route.ToList();
                temp_path.Add(next_point);
                seen_till_next_junction.Add(next_point);
                // Console.WriteLine("Input Path");
                // path.ForEach(Console.WriteLine);
                // Console.WriteLine("tempPath");
                // temp_path.ForEach(Console.WriteLine);
                //Moving onto the next points !!!!
                findPath(temp_path,temp_route,prev_junction,train_id,seen_till_next_junction);
            }
            else{
                Console.WriteLine("Point already in route");
            }
            Console.WriteLine("After STL");
        }
    }
}
