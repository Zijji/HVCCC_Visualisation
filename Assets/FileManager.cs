using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FileManager : MonoBehaviour
{
    static public string modelInputs, railEventLogs;

    public void OpenExplorer()
    {
        modelInputs = EditorUtility.OpenFilePanel("Add The Model Inputs", "", "xml");
        railEventLogs = EditorUtility.OpenFilePanel("Add The Rail Events Log", "", "xml");
    }
}
