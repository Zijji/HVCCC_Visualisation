using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunctionDisplayName : MonoBehaviour
{
    [SerializeField]
    TextMesh _textMesh; 

    // Start is called before the first frame update
    void Start()
    {

        _textMesh.text = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
