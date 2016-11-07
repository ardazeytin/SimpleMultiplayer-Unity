using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Health : Photon.MonoBehaviour {

    public const int maxHealth = 100;
    public bool destroyOnDeath;

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

    //Damage system
    public void TakeDamage(int amount)
    {
        if (photonView.isMine || currentHealth <= 0)
        {
            return;
        }

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            RpcDied();
            if (DeathMatchManager.RemovePlayerAndCheckWinner(this))
            {
                Health player = DeathMatchManager.GetWinner();
                player.Won();
                Invoke("BackToLobby", 3f);
            }

            return;
        }
    }

    [PunRPC]
    void RpcDied()
    {
        //GetComponent<PlayerColor>().HidePlayer(); //disable player color

        if (photonView.isMine)
        {
            //Write Game Over to loser screen
            informationText = FindObjectOfType<Text>();
            informationText.text = "Game Over";

            //disable player functions(These dirty codes need refactoring)
            //GetComponent<PlayerController_Net>().enabled = false;
            //GetComponent<Bullet>().enabled = false;
        }
    }

    [PunRPC]
    //Server to client invoke 
    public void Won()
    {
        //Server sends string to winner
        if (photonView.isMine)
        {
            informationText = FindObjectOfType<Text>();
            informationText.text = "You Win";
        }
    }

    //Return to lobby after end of match
    //void BackToLobby()
    //{
    //    FindObjectOfType<NetworkLobbyManager>().ServerReturnToLobby();
    //}

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
