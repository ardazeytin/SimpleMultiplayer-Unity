﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController_Net : NetworkBehaviour
{

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    //Player control input
    [ClientCallback]
	void Update ()
    {
        
        if (!isLocalPlayer)
        {
            return;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            CmdFire();
        }
	}

    //Spawn bullet on server when player press fire (client to server)   
    [Command]
    void CmdFire()
    {
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation); // create
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 9; //velocity
        NetworkServer.Spawn(bullet);
        Destroy(bullet, 2.0f);
    }
    
}
