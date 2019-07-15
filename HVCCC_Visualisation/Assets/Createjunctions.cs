using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schemas;
public class Createjunctions : MonoBehaviour
{
    public GameObject trainObject;
    public GameObject junction_parent;
    public GameObject junction;
    public GameObject loader;
    public GameObject section_object;
    public GameObject map;      //This is the map where the junctions will be plotted.
    public Terrain terrain;
    public double junction_lat;  //junction latitude
    public double junction_lon;  //junction longitude

    public GameObject topleft;  //Assumes topleft is representation of 
    public double topleft_lat = -29.7;
    public double topleft_lon = 148.7;

    public GameObject bottomright; //Assumes bottom right is 
    public double bottomright_lat = -34.3;
    public double bottomright_lon = 153.3;

    public XMLHelper xml_helper = new XMLHelper();
    List<AllJunctions> junctions = new List<AllJunctions>();


    //Topleft and Bottomright will need to represent the correct lattitudes and longitudes later.
    // Start is called before the first frame update
    void Start()
    {

        //EXCEL PART, THE CSV IS IN THE RESOURCES FOLDER
        TextAsset junctionData = Resources.Load<TextAsset>("Junctions_Coordinates");
        string[] data = junctionData.text.Split(new char[] { '\n' });
        Debug.Log(data.Length);
        for (int i = 1; i < data.Length-1; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            AllJunctions j = new AllJunctions();

            //gets the data for each row and adds to class AllJunctions
            j.id = row[0];

            j.signalName = row[1];
            //Debug.Log("he");
            string x = row[2];
            Debug.Log(x);
            x = x.Substring(1);
            double xCoord;
            double.TryParse(x, out xCoord);
            j.xCoordinate = xCoord;

            string z = row[3];
            z = z.Replace("\"", "");
            double zCoord;
            double.TryParse(z, out zCoord);
            j.zCoordinate = zCoord;
            
            junctions.Add(j);
        }

        Debug.Log(junctions[0]);
        // topleft.position.x;
        // bottomright.position.x;

        //Maps junction lattitude and longitude to x and z values
        double lat_distance = bottomright_lat - topleft_lat;
        double topleft_bottomright_length_z = bottomright.transform.position.z - topleft.transform.position.z;
        double lon_distance = bottomright_lon - topleft_lon;
        double topleft_bottomright_length_x = bottomright.transform.position.x - topleft.transform.position.x;

        for (int i = 0; i < junctions.Count; i++)
        {
            //Code below is for getting the junctions that junction i connects to
            //NOTE: THe code below will likely be ommited in the final product
            string connecting_junction_ids = "";
            dataRailNetworkSectionsSection[] sections = this.xml_helper.getSections();

            for (int j = 0; j < sections.Length; j++)
            {

                if (sections[j].startJunctionId.Equals(junctions[i].id))
                {

                    connecting_junction_ids += sections[j].endJunctionId + ';';
                }
                else if (sections[j].endJunctionId.Equals(junctions[i].id))
                {
                    connecting_junction_ids += sections[j].startJunctionId + ';';

                }
            }
            print(connecting_junction_ids);
            string[] connecting_junction_id_list = connecting_junction_ids.Split(';');

            double jn_lat_distance = junctions[i].xCoordinate - topleft_lat;
            double jn_pos_x = topleft.transform.position.x + topleft_bottomright_length_x * (jn_lat_distance / lat_distance);
            double jn_lon_distance = junctions[i].zCoordinate - topleft_lon;
            double jn_pos_z = topleft.transform.position.z + topleft_bottomright_length_z * (jn_lon_distance / lon_distance);
            float posy = Terrain.activeTerrain.SampleHeight(new Vector3((float)jn_pos_z, 0, (float)jn_pos_x));
            
            //Makes the loaders, might to find a better place to put, othjer wise they go invisible with junction markers.
            if(junctions[i].id.Contains("Loader") == true)
            {
                GameObject loaderobject = Instantiate(loader, new Vector3((float)jn_pos_z, posy, (float)jn_pos_x), transform.rotation);
            }


            //Creates the junctions
            GameObject junction_object = Instantiate(junction, new Vector3((float)jn_pos_z, posy, (float)jn_pos_x), transform.rotation);
            junction_object.transform.parent = junction_parent.transform;
            //givens the id and signal to the junction
            junction_object.name = junctions[i].id;
            junction_object.GetComponent<Junction>().junction_id = junctions[i].id; //set the variable you want to initialize
            junction_object.GetComponent<Junction>().printJunctionConnectionData(); //set the variable you want to initialize
            junction_object.GetComponent<Junction>().connecting_junctions = connecting_junction_id_list; //sets the connection junctions to a vraible inside junction

        }
        //Creates sections between junctions
        for (int i = 0; i < junctions.Count; i++)
        {
            GameObject thisJunction = GameObject.Find(junctions[i].id);
            GameObject sectionParent = GameObject.Find("SectionParent");
            
            if (thisJunction != null)
            {
                Debug.Log("Found thisJunction not null!");
                dataRailNetworkSectionsSection[] sections = this.xml_helper.getSections();

                for (int j = 0; j < sections.Length; j++)
                {
                    if (sections[j].startJunctionId.Equals(junctions[i].id))
                    {
                        //connecting_junction_ids += sections[j].endJunctionId + ';';
                        GameObject endJunction = GameObject.Find(sections[j].endJunctionId);
                        Debug.Log("Found endJunction");
                        if (endJunction != null)
                        {
                            Debug.Log("Found endJunction not null!");
                            
                            GameObject newSection = Instantiate(section_object, thisJunction.transform.position, thisJunction.transform.rotation);
                            newSection.transform.parent = sectionParent.transform;
                            newSection.GetComponent<TrackCreator>().SectionMoveStart(thisJunction.transform.position);
                            newSection.GetComponent<TrackCreator>().SectionMoveEnd(endJunction.transform.position);
                            newSection.GetComponent<TrackCreator>().SectionDraw();
                            
                        }

                        /*
                        else if (sections[j].endJunctionId.Equals(junctions[i].id))
                        {
                            connecting_junction_ids += sections[j].startJunctionId + ';';
                        }
                        */
                    }
                }
            }

            //thisJunction
        }

        /**
        int trainNo = 0;
        for (int i = 0; i < junctions.Count; i++)
        {
            if(Random.Range(0, 15) == 0)
            {
                //Creates Train for this junction randomly - 1 in 15 chance
                GameObject trainJunction = GameObject.Find(junctions[i].id);
                GameObject newTrainObject = Instantiate(trainObject, trainJunction.transform.position, trainJunction.transform.rotation);
                newTrainObject.GetComponent<TrainMove>().junctionDestination = GameObject.Find(junctions[i].id);
                newTrainObject.name = "Train" + trainNo;
                trainNo++;
                GameObject trainParent = GameObject.Find("TrainParent");
                newTrainObject.transform.parent = trainParent.transform;
            }
         }
         */

        // Just creating one train for testing
        Debug.Log("Nathan: creating 1 train for testing");
        GameObject trainJunction = GameObject.Find(junctions[0].id);
        GameObject newTrainObject = Instantiate(trainObject, trainJunction.transform.position, trainJunction.transform.rotation);
        newTrainObject.GetComponent<TrainMove>().junctionDestination = GameObject.Find(junctions[0].id);
        GameObject trainParent = GameObject.Find("TrainParent");
        newTrainObject.transform.parent = trainParent.transform;


        /*
        //Creates Train for this junction randomly - 1 in 15 chance
        GameObject trainJunction = GameObject.Find("thisJunction");
        GameObject newTrainObject = Instantiate(trainObject, trainJunction.transform.position, trainJunction.transform.rotation);
        newTrainObject.GetComponent<TrainMove>().junctionDestination = GameObject.Find("thisJunction");
        newTrainObject.name = "Train" + int.Parse(trainNo);
        */
        /*
        //Creates Train for one junction
        GameObject trainJunction = GameObject.Find("jn_Tahmoor_LoopPoints");
        GameObject newTrainObject = Instantiate(trainObject, trainJunction.transform.position, trainJunction.transform.rotation);
        newTrainObject.GetComponent<TrainMove>().junctionDestination = GameObject.Find("jn_Tahmoor_LoopPoints");
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
