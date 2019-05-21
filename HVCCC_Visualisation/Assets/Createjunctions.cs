using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Createjunctions : MonoBehaviour
{
    public GameObject junction_parent;
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

    List<AllJunctions> junctions = new List<AllJunctions>();


    //Topleft and Bottomright will need to represent the correct lattitudes and longitudes later.
    // Start is called before the first frame update
    void Start()
    {
        
        //EXCEL PART, THE CSV IS IN THE RESOURCES FOLDER
        TextAsset junctionData = Resources.Load<TextAsset>("Junctions_Coordinates");
        string[] data = junctionData.text.Split(new char[] { '\n' });
        //Debug.Log(data.Length);
        for(int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            AllJunctions j = new AllJunctions();

            //gets the data for each row and adds to class AllJunctions
            j.id = row[0];

            j.signalName = row[1];

            string x = row[2];
            x = x.Substring(1);
            double xCoord;
            double.TryParse(x, out xCoord);
            j.xCoordinate = xCoord;

            string z = row[3];
            z = z.Replace("\"", "");
            double zCoord;
            double.TryParse(z, out zCoord);
            j.zCoordinate = zCoord;

            //DOES ARE ALL THE BITS OF DATA IN EXCEL, THE IMPORTANT ONES AT LEAST
            //Debug.Log(row[0] + ", " + row[1] + ", " + xCoord + ", " + zCoord);

            junctions.Add(j);
        }

        Debug.Log(junctions[0]);
        //topleft.position.x
        //bottomright.position.x

        //Maps junction lattitude and longitude to x and z values
        double lat_distance = bottomright_lat - topleft_lat;
        double topleft_bottomright_length_x = bottomright.transform.position.x - topleft.transform.position.x;
        double lon_distance = bottomright_lon - topleft_lon;
        double topleft_bottomright_length_z = bottomright.transform.position.z - topleft.transform.position.z;
        /*
        double jn_lat_distance = junction_lat - topleft_lat;
        double jn_pos_x = topleft.transform.position.x + topleft_bottomright_length_x * (jn_lat_distance / lat_distance);
        double jn_lon_distance = junction_lon - topleft_lon;
        double jn_pos_z = topleft.transform.position.z + topleft_bottomright_length_z * (jn_lon_distance / lon_distance);
        //Creates the junctions
        Instantiate(junction, new Vector3((float)jn_pos_x, topleft.transform.position.y, (float)jn_pos_z), transform.rotation);
        */


        for (int i = 0; i < junctions.Count; i++)
        {
            double jn_lat_distance = junctions[i].xCoordinate - topleft_lat;
            double jn_pos_x = topleft.transform.position.x + topleft_bottomright_length_x * (jn_lat_distance / lat_distance);
            double jn_lon_distance = junctions[i].zCoordinate - topleft_lon;
            double jn_pos_z = topleft.transform.position.z + topleft_bottomright_length_z * (jn_lon_distance / lon_distance);
            //Creates the junctions
            GameObject junction_object = Instantiate(junction, new Vector3((float)jn_pos_x, topleft.transform.position.y, (float)jn_pos_z), transform.rotation);
            junction_object.transform.parent = junction_parent.transform;
            //givens the id and signal to the junction
            junction_object.name = junctions[i].id;
            junction_object.GetComponent<Junction>().junction_id=junctions[i].id; //set the variable you want to initialize
            junction_object.GetComponent<Junction>().printJunctionConnectionData(); //set the variable you want to initialize

        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
