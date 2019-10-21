using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapboxLinkJunctionsToSections : MonoBehaviour
{
    private bool junctionsCreated = false;
    private bool sectionsCreated = false;
    private MapboxCreateJunctions junctionScript; 
    private MapboxCreateSections sectionScript; 
    public string[] sectionStrings;
    public string[] junctionStrings;

    public GameObject junctionParent;
    public GameObject sectionParent;


    // Start is called before the first frame update
    void Start()
    {
        junctionScript = GetComponent<MapboxCreateJunctions>();
        sectionScript = GetComponent<MapboxCreateSections>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SectionsCreate()
    {
        sectionsCreated = true;
        if(junctionsCreated == true)
        {
            LinkJunctionsToSections();
        }
    }
    public void JunctionsCreate()
    {
        junctionsCreated = true;
        if(sectionsCreated == true)
        {
            LinkJunctionsToSections();
        }
    }

    private void LinkJunctionsToSections()
    {
        //Iterates through junction strings and attaches them to the closest section string

        //Section[] getSections = sectionParent.GetComponentsInChildren<Section>();
        

        junctionScript = GetComponent<MapboxCreateJunctions>();
        sectionScript = GetComponent<MapboxCreateSections>();

        Section[] getSections = sectionParent.GetComponentsInChildren<Section>();


        for(int i1 = 0; i1 < junctionStrings.Length; i1++)
        {
            string getJunCoord = junctionStrings[i1];
            string[] splitJunCoords =  getJunCoord.Split(char.Parse(","));
            float junX = float.Parse(splitJunCoords[0]);
            float junY = float.Parse(splitJunCoords[1]);
            //junctionStrings[i1] = ;
            
            Vector2 junctionVector = new Vector2(junX,junY);
            
            //Gets first section vector
            int selectedSection = 0;
            string getSecCoord = getSections[selectedSection].pathCoords[0];
            string[] splitCoords =  getSecCoord.Split(char.Parse(","));
            float secX = float.Parse(splitCoords[0]);
            float secY = float.Parse(splitCoords[1]);
            Vector2 selectedSectionCoord = new Vector2(secX, secY);
            
            for(int i2 = 0; i2 < sectionStrings.Length; i2++)
            {
                getSecCoord = getSections[i2].pathCoords[0];
                splitCoords =  getSecCoord.Split(char.Parse(","));
                secX = float.Parse(splitCoords[0]);
                secY = float.Parse(splitCoords[1]);
                Vector2 sectionVector = new Vector2(secX,secY);

                if(Vector2.Distance(sectionVector, junctionVector) < Vector2.Distance(selectedSectionCoord, junctionVector))
                {
                    selectedSection = i2;
                    selectedSectionCoord = sectionVector;
                }

            }
            //places junction at section
            junctionStrings[i1] = getSections[selectedSection].pathCoords[0];
            //junctionStrings[i1] = "-32.9213408, 151.7530507";//getSections[selectedSection].pathCoords[0];

        }

        //Transfers junction strings back to junction object.
        //junctionScript.setLocationStrings(junctionStrings);
        
        
        /*
        float junX = 
        float junY = 
         */
        
        //TODO: Special case for junctions that are too far away from any section - makes it their own section

        /*
        for(int i1 = 0; i1 < junctionStrings.Length; i1++)
        {
            
            junctionStrings[i1] =
            for(int i2 = 0; i2 < sectionStrings.Length; i2++)
            {
                ;
            }
        }
        
         */
        
            //
        //Goes from the first junction and navigates through the section network.
        //Culls any sections that are out of the network.




        //Iterates through Junction Parent, links junction to the closest one
        /*
        Transform[] getJunctionTransforms = junctionParent.GetComponentsInChildren<Transform>();
        List<GameObject> getJunctions = new List<GameObject>();
        foreach (Transform junction in getJunctionTransforms)
        { 
            getJunctions.Add(junction.gameObject);
        }

        foreach(GameObject junction in getJunctions)
        {
            //Finds nearest junction
            junction.
        }
         */
        
        //Just links them to the closest one.
    }
}
