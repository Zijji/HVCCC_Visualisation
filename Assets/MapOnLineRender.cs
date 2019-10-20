using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOnLineRender : MonoBehaviour
{
    public Transform sampleTrain;
    
    private LineRenderer line;
    private Junction junction1;
    private Junction junction2;
    // Start is called before the first frame update
    void Start()
    {
        sampleTrain.position = new Vector3();
        
        line.SetPosition(0, sampleTrain.position);

        line.enabled = false;

    }

    private void placeOnTrack(Vector3 position)
    {
        return;
    }
}
