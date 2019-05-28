using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schemas;

public class Junction : MonoBehaviour
{
    public string junction_id;
    public string[] connecting_junctions;
    //The xml helper that is used to get info from the xml
    public XMLHelper xml_helper;
    //The list of junction connections
    public dataRailNetworkJunctionsJunctionJunctionConnection[] junction_connections;
    // Start is called before the first frame update
    void Start()
    {
        xml_helper = new XMLHelper();

        //using the xml helper to get the junctionconnections
        this.junction_connections = xml_helper.getJunctionById(this.junction_id).junctionConnection;
        // print("id " + this.junction_id);
        this.printJunctionConnectionData();
        // print("id " + this.junction_id +  " with junction " + this.junction_connections[0].fromTrackId);

    }

    // Update is called once per frame
    void Update()
    {

    }

    //functions below prints all connection info. Also shows how to access junction connection info
    public void printJunctionConnectionData()
    {
        print("Junction Connection Data for " + this.junction_id);
        //if there are no junciton connections 
        if (this.junction_connections == null)
        {
            print("No junction Connections");
        }
        else
        {
            for (int i = 0; i < this.junction_connections.Length; i++)
            {
                string fromTrackId = this.junction_connections[i].fromTrackId;
                string toTrackId = this.junction_connections[i].toTrackId;
                string length = this.junction_connections[i].length;
                string safeToSit = this.junction_connections[i].safeToSit;
                string internalNodes = this.junction_connections[i].internalNodes;

                print("fromTrackId:" + fromTrackId + ";toTrackId:" + toTrackId + ";length:" + length + ";safeToSit:" + safeToSit + ";internalNodes" + internalNodes);
            }

        }
    }
}
