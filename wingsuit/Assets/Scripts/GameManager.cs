using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance; // singleton
    // Dictionary<int, int> teamPoints = new Dictionary<int, int>();

    void Awake() {
        Instance = this;

        // if(! PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("teamPoints")) {
        //     // create the teamPoints dictionary
        //     Dictionary<int, int> teamPoints = new Dictionary<int, int>();


        //     // TODO: replace 2 with (int) PhotonNetwork.CurrentRoom.CustomProperties["numTeams"]
        //     int numTeams = 2;
        //     for(int i = 0; i < numTeams; i++) {
        //         teamPoints.Add(i, 0);
        //     }

        //     Hashtable hash = new Hashtable();
        //     hash.Add("teamPoints", teamPoints);

        //     PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        // }
    }

    // public void AddPoints(int teamIndex, int points) {
    
    //     Dictionary<int, int> teamPoints = (Dictionary<int, int>) PhotonNetwork.CurrentRoom.CustomProperties["teamPoints"];
    //     teamPoints[teamIndex] += points;

    //     Hashtable hash = new Hashtable();
    //     hash.Add("teamPoints", teamPoints);
    //     PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
    //     // PhotonNetwork.SetCustomProperties(hash);

    //     Debug.Log("points updated");
    //     foreach(int key in teamPoints.Keys){
    //         Debug.Log("team" + key + ", points " + teamPoints[key]);
    //     }

    // }

    public void RespawnPlayers() {
        PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();
        foreach(PlayerController pc in playerControllers) {
            pc.Respawn();
        }
    }

    // public void RespawnBall(Ball ball) {
    //     ball.gameObject.SetActive(true);
    //     ball.Reset();
    // }
}
