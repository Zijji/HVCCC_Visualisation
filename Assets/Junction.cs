using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schemas;

public class Junction : MonoBehaviour
{
    public List<GameObject> junctionNeighbour = new List<GameObject>();      //Array of all junctions that are neighbour to this junction
    public List<GameObject> junctionNeighbourSections = new List<GameObject>();  //Sections that connect the associated junction. e.g. this junction connects to junctionNeighbour[x] via section junctionNeighbourSections[x]

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

}
