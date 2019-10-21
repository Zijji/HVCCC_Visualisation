using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FileManager : MonoBehaviour
{
    public string modelInputs, railEventLogs;
    public XMLHelper xml;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartVisualisation()
    {
        xml.ParseXML();
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene("ZoomableMap");
    }

    //
    public void OpenMI()
    {
        modelInputs = EditorUtility.OpenFilePanel("Add The Model Inputs", "", "xml");
    }
    public void OpenREL()
    {
        railEventLogs = EditorUtility.OpenFilePanel("Add The Rail Events Log", "", "xml");
    }
    public string GetMI()
    {
        return modelInputs;
    }
    public string GetREL()
    {
        return railEventLogs;
    }

}
