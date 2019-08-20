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

        Quaternion rotateVector = Quaternion.Euler(0,thisTransform.eulerAngles.y,0);
        thisTransform.position = thisTransform.position + (rotateVector * new Vector3(moveX * moveSpeed, 0, moveZ * moveSpeed));
    }
}
