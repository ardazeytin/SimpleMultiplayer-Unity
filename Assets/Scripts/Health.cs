using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Health : Photon.MonoBehaviour {

    public const int maxHealth = 100;
    public bool destroyOnDeath;

    public GameObject graveStone;

    //[SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;

    Text informationText; //Win or Lose text variable

    public RectTransform healthBar;

    //private NetworkStartPosition[] spawnPoints; //array of spawn points

    void Start()
    {
        //Set all health to max
        currentHealth = maxHealth;

        //Add player to server(from lobby)
        if (!photonView.isMine)
        {
            DeathMatchManager.AddPlayer(this);
        }
    }

    void Update()
    {
        if (!photonView.isMine)
        {
            return;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data

            stream.SendNext(currentHealth);
        }
        else
        {
            //Network player, receive data
            currentHealth = (int)stream.ReceiveNext();
            

        }
    }

    //Damage system
    public void TakeDamage(int amount)
    {
        if (amount == 10 && photonView.isMine)
        {
            //gameObject.SetActive(false);
            PhotonNetwork.Instantiate("gravestone", gameObject.transform.position, Quaternion.identity, 0);
            //PhotonNetwork.Destroy(graveStone);
            //PhotonNetwork.Destroy(gameObject);
            Invoke("BackToLobby", 4f);
            
        }
        
        //if (photonView.isMine || currentHealth <= 0)
        //{
        //    return;
        //}

        //currentHealth -= amount;
        //print(currentHealth);
        //if (photonView.isMine)
        //{
        //    photonView.RPC("sendHealthToServer", PhotonTargets.AllBuffered, currentHealth);
        //}
        //if (currentHealth <= 0)
        //{
        //    currentHealth = 0;
        //    RpcDied();
        //    if (DeathMatchManager.RemovePlayerAndCheckWinner(this))
        //    {
        //        Health player = DeathMatchManager.GetWinner();
        //        player.Won();
        //        Invoke("BackToLobby", 3f);
        //    }

        //    return;
        //}
        
    }

    //[PunRPC]
    //void sendHealthToServer(int newHealth)
    //{
    //    currentHealth = newHealth;
    //}

    
    void Died()
    {
        //GetComponent<PlayerColor>().HidePlayer(); //disable player color
        
        //if (photonView.isMine)
        //{
        //    //Write Game Over to loser screen
            

        //    //disable player functions(These dirty codes need refactoring)
        //    //GetComponent<PlayerController_Net>().enabled = false;
        //    //GetComponent<Bullet>().enabled = false;
        //}
    }

    //[PunRPC]
    ////Server to client invoke 
    //public void Won()
    //{
    //    //Server sends string to winner
    //    if (photonView.isMine)
    //    {
    //        informationText = FindObjectOfType<Text>();
    //        informationText.text = "You Win";
    //    }
    //}

    
    void BackToLobby()
    {
        PhotonNetwork.Disconnect();
    }

    void OnChangeHealth(int currentHealth)
    {
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }



    //Improved Spawn player codes
    //[Command]
    //void CmdRespawnSvr()
    //{
    //    var spawn = NetworkManager.singleton.GetStartPosition();
    //    var newPlayer = (GameObject)Instantiate(NetworkManager.singleton.playerPrefab, spawn.position, spawn.rotation);
    //    NetworkServer.Destroy(gameObject);
    //    NetworkServer.ReplacePlayerForConnection(connectionToClient, newPlayer, playerControllerId);
    //}
}
