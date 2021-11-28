using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private Vector3 _startScale;

    private void Start()
    {
        _startScale = transform.localScale;
        
        Debug.Log("AAA-3 _startScale : " + _startScale);
    }
}
