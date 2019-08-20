using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schemas;

public class CreateSections : MonoBehaviour
{
    public GameObject section;
    // Start is called before the first frame update
    void Start()
    {
        //Getting the xml helper 
        XMLHelper xml_helper = new XMLHelper();

        //Getting all the section Data
        dataRailNetworkSectionsSection[] section_parser_objects = xml_helper.getSections();

        //Building all the sections
        for (int i = 0; i < section_parser_objects.Length; i++)
        {
            //Creating the Secion object, Havn't really figured out where to put it yet.These values are just place holders. All will be built to origin
            double section_pos_x = 0.00;
            double section_pos_z = 0.00;
            double section_pos_y = 0.00;
            //float section_pos_y = Terrain.activeTerrain.SampleHeight(new Vector3((float)section_pos_x, 0, (float)section_pos_z));

            GameObject section_object = Instantiate(section, new Vector3((float)section_pos_x, (float)section_pos_y, (float)section_pos_z), transform.rotation);
            //givens the id and signal to the junction
            section_object.GetComponent<Section>().section_id=section_parser_objects[i].id; //set the variable you want to initialize

        }



    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
}
