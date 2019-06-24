//imports for xmlparser to work
using System.Xml;
using System.Xml.Serialization;
using Schemas;
using System;
//


public class XMLHelper{
    public data data_object;

    public XMLHelper(){
        XmlSerializer ser = new XmlSerializer(typeof(data));
        using (XmlReader reader = XmlReader.Create("modelInputs.xml"))
        {
            data_object = (data) ser.Deserialize(reader);

        }

    }

    public dataRailNetworkJunctionsJunction[] getJunctions(){
        //Return the all the jucntions as alist of junction objects
        return data_object.railNetwork[0].junctions[0].junction;
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
        return data_object.railNetwork[0].tracks[0].track;
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
        return data_object.railNetwork[0].sections[0].section;
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
        return data_object.railNetwork[0].railPlanner[0].timeTables[0];
    }

    public dataRailNetworkRailPlannerAllPaths getPaths()
    {
        //return data_object.railNetwork[0].railPlanner[0].timeTables;
        return data_object.railNetwork[0].railPlanner[0].allPaths[0];
    }

} 
