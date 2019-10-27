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
    public GameObject fromJunction;   //Assumes first pathCoord is the from junction
    public GameObject toJunction;   //Assumes last pathCoord is the to Junction

    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
