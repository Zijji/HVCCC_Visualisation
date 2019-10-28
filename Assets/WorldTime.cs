﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WorldTime : MonoBehaviour
{
    public XMLHelper finalTimeObject = new XMLHelper();
    public Slider worldTime;
    private float currentValue;
    private float time = 1.28f;
    public GameObject universalTime;
    bool sliderSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(finalTimeObject.GetLastTime());
        worldTime.onValueChanged.AddListener(ListenerMethod);
        worldTime.maxValue = finalTimeObject.GetLastTime();
        worldTime.minValue = universalTime.GetComponent<TimeController>().GetTime();
        worldTime.value = currentValue;

    }

    public void SliderSelected()
    {
        sliderSelected = true;
    }

    public void SliderDeselect()
    {
        sliderSelected = false;
    }

    // Update is called once per frame
    void Update()
    {
        time += (Time.deltaTime) * 0.0002f;
        if (!sliderSelected)
        {
            worldTime.value = time;
        }
        time = worldTime.value;
    }

    // Stops the value for the slider to go backwards
    public void ListenerMethod(float value)
    {
        if (value < currentValue)
            worldTime.value = currentValue;
        else
            currentValue = value;
    }
}