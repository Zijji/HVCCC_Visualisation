using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WSMGameStudio.Cameras
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public CameraSettings cameraSettings;

        private Vector3 newPos;

        // Use this for initialization
        void Start()
        {
            //target = GameObject.FindGameObjectWithTag(Tags.Player).transform;
        }

        // Update is called once per frame
        void Update()
        {
            ApplyOffsetToPosition(cameraSettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offSet"></param>
        private void ApplyOffsetToPosition(CameraSettings offSet)
        {
            newPos = transform.position;
            newPos.x = target.position.x + offSet.xOffset;
            newPos.z = target.position.z + offSet.zOffset;
            newPos.y = target.position.y + offSet.yOffset;

            transform.position = newPos;

            if (!cameraSettings.rotateAround)
                transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime);

            transform.LookAt(target.transform);
        }
    }
}

