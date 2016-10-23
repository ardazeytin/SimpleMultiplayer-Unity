using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class PlayerColor_Hook : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerColor player = gamePlayer.GetComponent<PlayerColor>();

        player.color = lobby.playerColor;
    }
}
