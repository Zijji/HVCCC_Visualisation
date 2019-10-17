/*
Sections connect two junctions to each other

 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
    public string[] pathCoords;     //Stores all co-ordinate pairs in the pathCoords object

    //nextSection
    public List<GameObject> nextSection; //Which section does this go to next?
    public List<string> nextSectionCoord; //Which coord in the next section does this section connect to?

    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
