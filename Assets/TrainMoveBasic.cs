using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;

/* Scripts that make trains move */
public class TrainMoveBasic : MonoBehaviour
{
    //public GameObject stationDestionation;
    public float speed = 0.25f;
    public GameObject junctionDestination = null; //set this variable to be the next destination.
    public Vector3 junctionPrevTransform;
    private float waitTime = 1.0f;

    [SerializeField]
    AbstractMap _map;

    [SerializeField]
    float _spawnScale = 2.0f;

    private int pathCurrentDest = 0;        //Index of current destination in path

    // Update is called once per frame
    void FixedUpdate()
    {
        if (waitTime > 0.0f)
        {
            waitTime -= Time.deltaTime;
        }
        else
        {
            if (junctionDestination == null)
            {
                junctionDestination = GameObject.Find("jn_Waratah");
                transform.position = junctionDestination.transform.position;
                junctionPrevTransform = junctionDestination.transform.position;
            }
            BasicTrainMovement();
            if (GetNearDest())
            {
                if (junctionDestination.name.Equals("jn_Waratah"))
                {
                    junctionDestination = GameObject.Find("jn_Islington");
                    junctionPrevTransform = junctionDestination.transform.position;
                }
                else
                {
                    junctionDestination = GameObject.Find("jn_Waratah");
                    junctionPrevTransform = junctionDestination.transform.position;
                }

            }
        }
        
    }

    private void Update()
    {
        if (junctionDestination != null)
        {
            if (junctionPrevTransform != junctionDestination.transform.position)
            {
                transform.position += (junctionDestination.transform.position - junctionPrevTransform);
            }
            junctionPrevTransform = junctionDestination.transform.position;

        }
        //transform.localPosition = _map.GeoToWorldPosition(location, true);
        //transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);

    }

    void Start()
    {
        //BasicTrainMovement();
        /*
        junctionDestination = GameObject.Find("jn_Waratah");
        transform.position = junctionDestination.transform.position;
        junctionDestination = GameObject.Find("jn_Islington");
         * */
    }

    private void BasicTrainMovement()
    {
        if (junctionDestination != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, junctionDestination.transform.position, speed * Time.deltaTime);
        }
    }

    //Returns true if distance to destination is <0.1f, false otherwise
    private bool GetNearDest()
    {
        if (junctionDestination != null)
        {
            var heading = junctionDestination.transform.position - transform.position;
            if (heading.magnitude < 0.1f)
            {
                return true;
            }
        }
        return false;
    }
}
