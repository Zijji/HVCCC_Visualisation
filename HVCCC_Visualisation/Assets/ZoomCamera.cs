using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    public float minfov = 10.0f;
    public float maxfov = 60.0f;
    public float zoomspeed = 50.0f;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        cam.fieldOfView += -Input.GetAxis("Mouse ScrollWheel")*zoomspeed;
        if(cam.fieldOfView < minfov)
        {
            cam.fieldOfView = minfov;
        }
        if (cam.fieldOfView > maxfov)
        {
            cam.fieldOfView = maxfov;
        }
    }
}
