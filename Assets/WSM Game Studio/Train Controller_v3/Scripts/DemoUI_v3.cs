using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WSMGameStudio.RailroadSystem
{
    public class DemoUI_v3 : MonoBehaviour
    {
        public TrainController_v3 train;
        public Slider maxSpeedSlider;
        public Slider accelerationSlider;
        public Slider brakeSlider;

        private void Update()
        {
            if (maxSpeedSlider != null)
                train.maxSpeedKph = maxSpeedSlider.value;

            if (accelerationSlider != null)
                train.acceleration = accelerationSlider.value;

            if (brakeSlider != null)
                train.brake = brakeSlider.value;
        }

        public void ToggleLights()
        {
            train.ToggleLights();
        }

        public void Honk()
        {
            train.Honk();
        }
    }
}
