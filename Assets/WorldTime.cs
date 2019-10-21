using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WorldTime : MonoBehaviour
{
    public GameObject finalTimeObject;
    public Slider worldTime;
    private float currentValue;
    private float time = 1.28f;
    public GameObject universalTime;
    bool sliderSelected = false;
    public XMLHelper xml;

    // Start is called before the first frame update
    void Start()
    {
        finalTimeObject = GameObject.Find("FileManager");
        xml = finalTimeObject.GetComponent<XMLHelper>();
        worldTime.onValueChanged.AddListener(ListenerMethod);
        worldTime.maxValue = xml.GetLastTime();
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
