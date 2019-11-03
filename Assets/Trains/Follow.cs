using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    //place this script on the player gameobject

    public GameObject Leader; // in the inspector drag the gameobject the will be following the player to this field
    //public int followDistance;
    public float followDistance;
    private List<Vector3> storedPositions;
    private List<Quaternion> rotations;


    void Awake()
    {
        storedPositions = new List<Vector3>(); //create a blank list

        if (!Leader)
        {
            Debug.Log("The FollowingMe gameobject was not set");
        }

        if (followDistance == 0)
        {
            Debug.Log("Please set distance higher then 0");
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (storedPositions.Count == 0)
        {
            Debug.Log("blank list");
            storedPositions.Add(Leader.transform.position); //store the players currect position
            //rotations.Add(Leader.transform.rotation);
            return;
        }
        else if (storedPositions[storedPositions.Count - 1] != Leader.transform.position)
        {
            //Debug.Log("Add to list");
            storedPositions.Add(Leader.transform.position); //store the position every frame
            //rotations.Add(Leader.transform.rotation);
        }

        //if (storedPositions.Count > followDistance)
        if (Vector3.Distance(Leader.transform.position, transform.position) > followDistance)
        {
            transform.position = storedPositions[0]; //move
            //transform.rotation = rotations[0];
            storedPositions.RemoveAt(0); //delete the position that player just move to
            //rotations.RemoveAt(0);
        }
        else
        {
            transform.position = storedPositions[0];
        }
    }
}
