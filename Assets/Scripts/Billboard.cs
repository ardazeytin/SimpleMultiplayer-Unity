﻿using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

    // Player healt bars always should look to gameplay camera
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
