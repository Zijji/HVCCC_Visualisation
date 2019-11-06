using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;



public class TrainSegmentedSpawner : MonoBehaviour
{
    public float timeSeg = 0.005f;
    public int numSegments = 3;
    private int totalSegments = 0;
    private float startTime;
    private GameObject getTimeObj;
    public GameObject trainSegment;

    private TrainMovement thisTrainMovement;



    //Spawns a bunch of trains
    public List<GameObject> trainSegments = null;
    // Start is called before the first frame update
    void Start()
    {
        thisTrainMovement = this.GetComponent<TrainMovement>();
        getTimeObj = GameObject.Find("TimeObject");
        startTime = getTimeObj.GetComponent<TimeController>().GetTime();
    }

    // Update is called once per frame
    void Update()
    {
        if( getTimeObj.GetComponent<TimeController>().GetTime() > startTime + timeSeg && totalSegments < numSegments)
        {
            GameObject thisJunction = GameObject.Find(thisTrainMovement.TrainPath.GetJunction(0));
            GameObject newTrain = Instantiate(trainSegment, thisJunction.transform.position, thisJunction.transform.rotation);
            newTrain.name = "ConsistSegment";
            //Creates new train segment
            ConsistPath retrieveTrainPath = thisTrainMovement.TrainPath;
            newTrain.GetComponent<TrainMovement>().TrainPath = new ConsistPath();
            for(int i = 0; i < retrieveTrainPath.Length(); i++)
            {
                newTrain.GetComponent<TrainMovement>().TrainPath.AddDestination(retrieveTrainPath.GetJunction(i), retrieveTrainPath.GetArrivalTime(i) + timeSeg*totalSegments, retrieveTrainPath.GetDepartureTime(i) + timeSeg*totalSegments);
            }
            Debug.Log(newTrain.GetComponent<TrainMovement>().TrainPath.ToString());
            startTime = getTimeObj.GetComponent<TimeController>().GetTime();
            totalSegments++;
            /*
            
             */
        }
    }
}
