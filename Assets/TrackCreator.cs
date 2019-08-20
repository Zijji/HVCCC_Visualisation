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
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SectionMoveStart(Vector3 newPos)
    {
        TrackStart.transform.position = newPos;
        return;
    }
    public void SectionMoveEnd(Vector3 newPos)
    {
        TrackEnd.transform.position = newPos;
        return;
    }
    public void SectionDraw()
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
            GameObject newTrack = Instantiate(TrackObject, TrackPosition, TrackStart.transform.rotation);
            newTrack.transform.parent = transform.parent;
            Collider TrackCollider = TrackObject.GetComponent<Collider>();
            TrackPosition += (TrackEnd.transform.position - TrackStart.transform.position).normalized * 0.02f;//, TrackCollider.bounds.size;
            //TrackPosition += Vector3.Project((TrackEnd.transform.position - TrackStart.transform.position).normalized, TrackCollider.bounds.size);
        }
    }
}
