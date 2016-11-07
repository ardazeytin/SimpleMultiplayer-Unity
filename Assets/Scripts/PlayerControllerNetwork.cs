using UnityEngine;
using System.Collections;

public class PlayerControllerNetwork : Photon.MonoBehaviour {

    public float speed = 5f;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    bool firstTake = false;

    void OnEnable()
    {
        firstTake = true;
    }

    void Awake()
    {
        gameObject.name = gameObject.name + photonView.viewID;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data

            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            print("SendPos: " + transform.position);
        }
        else
        {
            //Network player, receive data

            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
            print("RecievePos: " + transform.position);

            // avoids lerping the character from "center" to the "current" position when this client joins
            if (firstTake)
            {
                firstTake = false;
                transform.position = correctPlayerPos;
                transform.rotation = correctPlayerRot;
            }

        }
    }

    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

    void Update()
    {
        if (photonView.isMine)
        {
            if (Input.GetKey(KeyCode.W))
                GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + Vector3.forward * speed * Time.deltaTime);

            if (Input.GetKey(KeyCode.S))
                GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position - Vector3.forward * speed * Time.deltaTime);

            if (Input.GetKey(KeyCode.D))
                GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + Vector3.right * speed * Time.deltaTime);

            if (Input.GetKey(KeyCode.A))
                GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position - Vector3.right * speed * Time.deltaTime);

            Fire();
            InputColorChange();
        }

        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
        }
    }

    
    void Fire()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            SpawnBullet();
        }   
    }

    [PunRPC]
    void SpawnBullet()
    {
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation); // create
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 9; //velocity
        //NetworkServer.Spawn(bullet);
        Destroy(bullet, 2.0f);
        if (photonView.isMine)
        {
            photonView.RPC("SpawnBullet", PhotonTargets.All, bullet);
        }
    }


    private void InputColorChange()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
    }

    [PunRPC]
    void ChangeColorTo(Vector3 color)
    {
        GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);

        if (photonView.isMine)
        {  
            //GetComponent<NetworkView>().RPC("ChangeColorTo", RPCMode.OthersBuffered, color); <<--- old 
            photonView.RPC("ChangeColorTo", PhotonTargets.OthersBuffered, color); // <<--- new
        }
    }
}
