using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSMGameStudio.RailroadSystem;

public interface ITrainDoorsController
{
    StationDoorDirection StationDoorDirection { get; set; }

    void OpenCabinDoorLeft();
    void OpenCabinDoorRight();
    void CloseCabinDoorLeft();
    void CloseCabinDoorRight();
    void OpenPassengersDoors();
    void ClosePassengersDoors();
}
