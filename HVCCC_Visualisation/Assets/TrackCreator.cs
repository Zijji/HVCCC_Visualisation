using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCreator : MonoBehaviour
{
    public GameObject TrackStart;
    public GameObject TrackEnd;
    public GameObject TrackObject;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 TrackPosition = TrackStart.transform.position;

        /*
        boolean quit = false;
        while(!quit)
        {
            
        }
        */
        //for(int i = 0; i < 50; i++)
        //while(Vector3.Distance(TrackStart.transform.position, TrackPosition) < Vector3.Distance(TrackEnd.transform.position, TrackStart.transform.position))
        //for (int i = 0; i < 50; i++)
        while (Vector3.Distance(TrackStart.transform.position, TrackPosition) < Vector3.Distance(TrackEnd.transform.position, TrackStart.transform.position))
        {
            Instantiate(TrackObject, TrackPosition, TrackStart.transform.rotation);
            Collider TrackCollider = TrackObject.GetComponent<Collider>();
            TrackPosition += (TrackEnd.transform.position - TrackStart.transform.position).normalized * 0.02f;//, TrackCollider.bounds.size;
            //TrackPosition += Vector3.Project((TrackEnd.transform.position - TrackStart.transform.position).normalized, TrackCollider.bounds.size);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
