using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject[] objects;
    public bool setToggle = true;
    public Slider time;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }


    //controls the pausing of the scene
    public void pauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }

    public void toggle()
    {
        setToggle = !setToggle;
        foreach (GameObject go in objects)
        {
            go.SetActive(setToggle);
        }
    }
    public void AccelerateTime()
    {
        Time.timeScale = (float)time.value;
    }

}
