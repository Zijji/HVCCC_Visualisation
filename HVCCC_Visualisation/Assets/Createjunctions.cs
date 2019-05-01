using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Createjunctions : MonoBehaviour
{
    public GameObject junction;
    public GameObject map;      //This is the map where the junctions will be plotted.
    public double junction_lat;  //junction latitude
    public double junction_lon;  //junction longitude

    public GameObject topleft;  //Assumes topleft is representation of -31.725714, 147.696161 
    public double topleft_lat = -31.725714;
    public double topleft_lon = 147.696161;

    public GameObject bottomright; //Assumes bottom right is -33.854337, 152.634515
    public double bottomright_lat = -33.854337;
    public double bottomright_lon = 152.634515;


    //Topleft and Bottomright will need to represent the correct lattitudes and longitudes later.
    // Start is called before the first frame update
    void Start()
    {
        //Maps junction lattitude and longitude to x and z values
        double lat_distance = bottomright_lat - topleft_lat;
        double jn_lat_distance = junction_lat - topleft_lat;
        double topleft_bottomright_length_x = bottomright.transform.position.x - topleft.transform.position.x;
        double jn_pos_x = topleft.transform.position.x + topleft_bottomright_length_x * (jn_lat_distance / lat_distance);

        double lon_distance = bottomright_lon - topleft_lon;
        double jn_lon_distance = junction_lon - topleft_lon;
        double topleft_bottomright_length_z = bottomright.transform.position.z - topleft.transform.position.z;
        double jn_pos_z = topleft.transform.position.z + topleft_bottomright_length_z * (jn_lon_distance / lon_distance);




        //topleft.position.x
        //bottomright.position.x


        //Creates the junctions
        Instantiate(junction, new Vector3( (float) jn_pos_x,topleft.transform.position.y, (float) jn_pos_z), transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
