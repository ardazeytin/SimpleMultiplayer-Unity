using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{

	public const int maxHealth = 100;
    public bool destroyOnDeath;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;
    Text informationText; //Win or Lose text variable

    public RectTransform healthBar;

    private NetworkStartPosition[] spawnPoints; //array of spawn points

    void Start()
    {
        //Set all health to max
        currentHealth = maxHealth;

        //Add player to server(from lobby)
        if (isServer)
        {
            DeathMatchManager.AddPlayer(this);
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
    }

    //Damage system
    public void TakeDamage(int amount)
    {
        if (!isServer || currentHealth <= 0)
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
                player.RpcWon();
                Invoke("BackToLobby", 3f);
            }

            return;
        }   
    }

    [ClientRpc]
    void RpcDied()
    {
        GetComponent<PlayerColor>().HidePlayer(); //disable player color

        if (isLocalPlayer)
        {
            //Write Game Over to loser screen
            informationText = FindObjectOfType<Text>();
            informationText.text = "Game Over";

            //disable player functions(These dirty codes need refactoring)
            GetComponent<PlayerController_Net>().enabled = false;
            GetComponent<Bullet>().enabled = false;
        }
    }

    [ClientRpc] 
    //Server to client invoke 
    public void RpcWon()
    {
        //Server sends string to winner
        if (isLocalPlayer)
        {
            informationText = FindObjectOfType<Text>();
            informationText.text = "You Win";
        }
    }

    //Return to lobby after end of match
    void BackToLobby()
    {
        FindObjectOfType<NetworkLobbyManager>().ServerReturnToLobby();
    }

    void OnChangeHealth(int currentHealth)
    {
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    

    //Improved Spawn player codes
    [Command]
    void CmdRespawnSvr()
    {
        var spawn = NetworkManager.singleton.GetStartPosition();
        var newPlayer = (GameObject)Instantiate(NetworkManager.singleton.playerPrefab, spawn.position, spawn.rotation);
        NetworkServer.Destroy(gameObject);
        NetworkServer.ReplacePlayerForConnection(connectionToClient, newPlayer, playerControllerId);
    }

    //Improved codes
    //Color selecting moved to new UI Lobby system. These codes only works in match. Useless for now. 

    //public void OnGUI()
    //{
    //    if (isLocalPlayer)
    //    {
    //        if (GUILayout.Button("Change Color"))
    //        {
    //            CmdChangeColor();
    //        }
    //    }
    //}

    //[Command]
    //public void CmdChangeColor()
    //{
    //    color = colorList[Random.Range(0, colorList.GetLength(0))];
    //}

    //[ClientRpc] //Opposite of Command. This works server to client.
    //void RpcRespawn()
    //{
    //    if (!isLocalPlayer)
    //    {
    //        //set spawn point to the origin
    //        Vector3 spawnPoint = Vector3.zero;

    //        //pick one randon spawn point
    //        if (spawnPoints != null && spawnPoints.Length > 0)
    //        {
    //            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
    //        }

    //        transform.position = spawnPoint;
    //    }
    //}

}
