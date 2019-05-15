using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    private Transform thisTransform;

    // Start is called before the first frame update
    void Start()
    {
        thisTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = -Input.GetAxis("Horizontal");
        float moveZ = -Input.GetAxis("Vertical");

        thisTransform.position = new Vector3(thisTransform.position.x + moveX * moveSpeed, 0.0f, thisTransform.position.z + moveZ * moveSpeed);
    }
}
