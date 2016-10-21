using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    // Use this for initialization
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    Color[] colorList = new Color[] { Color.red, Color.yellow, Color.blue, Color.cyan, Color.magenta, Color.green };

    [SyncVar]
    Color color = Color.gray;


	
	// Update is called once per frame
    [ClientCallback]
	void Update ()
    {
        GetComponent<MeshRenderer>().material.color = color;

        if (!isLocalPlayer)
        {
            return;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }
	}

   
    [Command]
    void CmdFire()
    {
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation); // create
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 9; //velocity
        NetworkServer.Spawn(bullet);
        Destroy(bullet, 2.0f);
    }

    public void OnGUI()
    {
        if (isLocalPlayer)
        {
            if (GUILayout.Button("Change Color"))
            {
                CmdChangeColor();
            }
        }
    }

    [Command]
    void CmdChangeColor()
    {
        color = colorList[Random.Range(0, colorList.GetLength(0))];
    }

    
}
