using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public const int maxHealth = 100;
    public bool destroyOnDeath;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;
    Text informationText;

    public RectTransform healthBar;

    private NetworkStartPosition[] spawnPoints;


    //Color[] colorList = new Color[] { Color.red, Color.yellow, Color.blue, Color.cyan, Color.magenta, Color.green };

    //[SyncVar]
    //Color color = Color.gray;

    void Start()
    {
        //if (!isLocalPlayer)
        //{
        //    spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        //}
        currentHealth = maxHealth;
        if (isServer)
        {
            DeathMatchManager.AddPlayer(this);
        }
    }
    void Update()
    {

        //GetComponent<MeshRenderer>().material.color = color;

        if (!isLocalPlayer)
        {
            return;
        }
    }

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

            //disable player functions
            GetComponent<PlayerController_Net>().enabled = false;
            GetComponent<Bullet>().enabled = false;
        }
    }

    [ClientRpc]
    public void RpcWon()
    {
        if (isLocalPlayer)
        {
            informationText = FindObjectOfType<Text>();
            informationText.text = "You Win";
        }
    }

    void BackToLobby()
    {
        //Return to lobby
        FindObjectOfType<NetworkLobbyManager>().ServerReturnToLobby();
    }

    void OnChangeHealth(int currentHealth)
    {
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

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

    [Command]
    void CmdRespawnSvr()
    {
        var spawn = NetworkManager.singleton.GetStartPosition();
        var newPlayer = (GameObject)Instantiate(NetworkManager.singleton.playerPrefab, spawn.position, spawn.rotation);
        NetworkServer.Destroy(gameObject);
        NetworkServer.ReplacePlayerForConnection(connectionToClient, newPlayer, playerControllerId);
    }

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


}
