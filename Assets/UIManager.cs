﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject[] objects;
    public bool setToggle = true;
    public Slider time;
    public GameObject universalTime;
    public Text timeText;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        DisplayTime();
    }


    //controls the pausing of the scene
    public void pauseControl()
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = time.value;
        }
    }

    public void toggle()
    {
        setToggle = !setToggle;
        foreach (GameObject go in objects)
        {
            foreach (Transform child in go.transform)
            {
                child.GetChild(0).gameObject.SetActive(setToggle);
                child.GetChild(1).gameObject.SetActive(setToggle);
            }
        }
    }
    public void AccelerateTime()
    {
        Time.timeScale = (float)time.value;
    }
    public void DisplayTime()
    {
        //timeText.text = "" + universalTime.GetComponent<TimeController>().GetTime();
        float hours = universalTime.GetComponent<TimeController>().GetTime();
        float minutes = hours % 1;
        string hr;
        string min;
        int m;
        hours = hours - minutes;
        minutes = minutes * 60;
        m = (int)minutes;
        if(hours<10)
        {
            hr = "0"+hours.ToString();
        }
        else
        {
            hr = hours.ToString();
        }
        if (minutes < 10)
        {
            min = "0" + m.ToString();
        }
        else
        {
            min = m.ToString();
        }
        timeText.text = hr + ":" + min;
        //timeText.text = "" + universalTime.GetComponent<TimeController>().GetTime();
    }

}
