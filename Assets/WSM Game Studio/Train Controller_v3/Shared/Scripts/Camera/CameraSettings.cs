using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WSMGameStudio.Cameras
{
    [Serializable]
    public class CameraSettings
    {
        public float cameraSpeed = 0;
        public float xOffset = 0;
        public float yOffset = 0;
        public float zOffset = 0;
        public bool rotateAround = false;
    } 
}
