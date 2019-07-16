using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public float time = 764;
    public float speed = 1;     //speed of time. 1 = 1x speed; -1 = rewind


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += (Time.deltaTime)*speed;
    }

    //Gets time
    public float GetTime()
    {
        return time;
    }

    //Gets speed
    public float GetSpeed()
    {
        return speed;
    }
}
