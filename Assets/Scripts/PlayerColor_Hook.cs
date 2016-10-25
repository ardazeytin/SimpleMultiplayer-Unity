using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class PlayerColor_Hook : LobbyHook
{

    //Override method to get player's selected color in lobby and set to in game mesh color 
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerColor player = gamePlayer.GetComponent<PlayerColor>();

        player.color = lobby.playerColor;
    }
}
