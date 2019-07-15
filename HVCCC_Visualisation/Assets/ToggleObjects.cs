using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjects : MonoBehaviour
{
    public GameObject[] objects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //creates a GUI button that works but not with the current camera/cursor because they are locked, need to discuss
    void OnGUI()
    {
        foreach(GameObject go in objects)
        {
            bool active = GUILayout.Toggle(go.activeSelf, go.name); 
            if(active != go.activeSelf)
            {
                go.SetActive(active);
            }
        }
    }
}
