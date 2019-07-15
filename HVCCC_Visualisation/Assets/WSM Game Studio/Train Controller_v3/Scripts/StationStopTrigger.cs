using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WSMGameStudio.RailroadSystem
{
    public class StationStopTrigger : MonoBehaviour
    {
        public StopMode stopMode;
        public StationDoorDirection stationDoorDirection;
        public StationBehaviour stationBehaviour;
        public float stopTimeout = 10f;
        [Range(0, 100)]
        public int randomStopProbability = 50;
        public bool turnOffEngines = false;

        private bool _alreadyStopped = false;

        private void OnTriggerEnter(Collider other)
        {
            TrainStationController trainStationController = other.GetComponent<TrainStationController>();

            if (trainStationController != null)
            {
                switch (stopMode)
                {
                    case StopMode.Always:
                        trainStationController.StopAtStation(stationBehaviour, stopTimeout, turnOffEngines);
                        break;
                    case StopMode.Once:
                        if (!_alreadyStopped)
                        {
                            trainStationController.StopAtStation(stationBehaviour, stopTimeout, turnOffEngines);
                            _alreadyStopped = true;
                        }
                        break;
                    case StopMode.Random:
                        if (Extension.RandomEvent(randomStopProbability))
                            trainStationController.StopAtStation(stationBehaviour, stopTimeout, turnOffEngines);
                        break;
                }

                ITrainDoorsController trainDoorsController = other.GetComponent<ITrainDoorsController>();

                if (trainDoorsController != null)
                    trainDoorsController.StationDoorDirection = stationDoorDirection;
            }
        }
    }
}
