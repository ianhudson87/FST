using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance; // singleton
    Player tagger;
    [SerializeField] PhotonView PV;
    // Dictionary<int, int> teamPoints = new Dictionary<int, int>();

    void Awake() {
        Instance = this;
    }

    void Start() {
        // choose a tagger and sync it across all players
        if(PhotonNetwork.IsMasterClient) {
            SetTagger(ChooseRandomPlayer());
        }
    }

    void SetTagger(Player tagger) {
        PV.RPC("RPC_SetTagger", RpcTarget.All, new object[] {tagger});
    }

    [PunRPC] void RPC_SetTagger(Player tagger) {
        this.tagger = tagger;
        HUDManager.Instance.CreateNotification("new tagger:" + tagger.NickName);
    }

    Player ChooseRandomPlayer() {
        int numPlayers = PhotonNetwork.PlayerList.Length;
        return PhotonNetwork.PlayerList[Random.Range(0, numPlayers)];
    }


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
