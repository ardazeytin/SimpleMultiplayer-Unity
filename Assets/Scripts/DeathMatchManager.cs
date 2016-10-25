using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathMatchManager : MonoBehaviour {

    static List<Health> players = new List<Health>(); //list of players

    //Add new player to list
    public static void AddPlayer(Health player)
    {
        players.Add(player);
    }

    //Remove dead players(loser) from players list 
    public static bool RemovePlayerAndCheckWinner(Health player)
    {
        players.Remove(player);

        if (players.Count == 1)
        {
            return true;
        }

        return false;
    }

    //Find last alive player from list to set winner
    public static Health GetWinner()
    {
        if (players.Count != 1)
        {
            return null;
        }

        return players[0];
    }
	
}
