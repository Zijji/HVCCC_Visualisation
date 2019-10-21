using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunctionDisplayName : MonoBehaviour
{
    [SerializeField]
    TextMesh _textMesh;
    public GameObject LoadPoint;

    // Start is called before the first frame update
    void Start()
    {

        _textMesh.text = gameObject.name;
        if(gameObject.name.Contains("Loader")!=true)
        {
            LoadPoint.SetActive(false);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
