using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schemas;

public class Section : MonoBehaviour
{

    //all the raw data from the xml
    public string section_id;
    public string length;
    public string network_path;
    public string start_junction_id;
    public string end_junction_id;
    public string acceleration_time;
    public string track_ids;
    public string run_time_up;
    public string run_time_down;

    public dataRailNetworkTracksTrack[] section_tracks;

    // Start is called before the first frame update
    void Start()
    {
        XMLHelper xml_helper = new XMLHelper();


        //using the xml helper to get the section from the xml
        dataRailNetworkSectionsSection section_parse_object = xml_helper.getSectionById(this.section_id);

        //Setting the varaibles of the object to the values found in the xml
        this.section_id = section_parse_object.id;
        this.length = section_parse_object.length;
        this.network_path = section_parse_object.networkPath;
        this.start_junction_id = section_parse_object.startJunctionId;
        this.end_junction_id=section_parse_object.endJunctionId;
        this.acceleration_time = section_parse_object.accelerationTime;
        //this.track_ids = section_parse_object.trackIds.Split(" ");
        this.run_time_up = section_parse_object.runTimeUp;
        this.run_time_down = section_parse_object.runTimeDown;

        //getting the tracks
        string[] track_ids_array = this.track_ids.Split(' ');
        //initializing the section_tracks array as length of track ids
        this.section_tracks = new dataRailNetworkTracksTrack[track_ids_array.Length];//dataRailNetworkSectionsSection[track_ids_array.Length];
        for ( int i = 0 ; i < track_ids_array.Length;i++){
            this.section_tracks[i] = xml_helper.getTrackById(track_ids_array[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
