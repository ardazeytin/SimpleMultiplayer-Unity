using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathMatchManager : MonoBehaviour {

    static List<Health> players = new List<Health>(); //list of players

    public static void AddPlayer(Health player)
    {
        players.Add(player);
    }

    public static bool RemovePlayerAndCheckWinner(Health player)
    {
        players.Remove(player);

        if (players.Count == 1)
        {
            return true;
        }

        return false;
    }

    public static Health GetWinner()
    {
        if (players.Count != 1)
        {
            return null;
        }

        return players[0];
    }
	
}
