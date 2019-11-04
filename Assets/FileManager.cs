using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FileManager : MonoBehaviour
{
    public string modelInputs, railEventLogs;
    public XMLHelper xml;
    public Text errorMsg;
    // Start is called before the first frame update
    void Start()
    {
        errorMsg.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartVisualisation()
    {
        Boolean test = xml.ParseXML();
        if(test == false)
        {
            errorMsg.text = "Invalid XML files";
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            SceneManager.LoadScene("ZoomableMap");
        }
        
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
