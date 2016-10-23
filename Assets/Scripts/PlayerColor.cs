using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerColor : NetworkBehaviour {

    [SyncVar]
    public Color color; //The color to change players

    MeshRenderer[] rends; // Array to store the mesh renderers of the player

	// Use this for initialization
	void Start ()
    {
        rends = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material.color = color;
        }	
	}

    public void HidePlayer()
    {
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material.color = Color.clear;
        }
    }

	
	// Update is called once per frame
	void Update () {
	
	}
}
