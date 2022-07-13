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
    [SerializeField] float startingTimeToWin;
    Dictionary<Player, float> playersTimeToWin = new Dictionary<Player, float>(); // only used by master client
    // Dictionary<int, int> teamPoints = new Dictionary<int, int>();

    void Awake() {
        Instance = this;
        foreach(Player p in PhotonNetwork.PlayerList) {
            // Hashtable hash = new Hashtable();
            playersTimeToWin.Add(p, startingTimeToWin);
        }
        SyncPlayersTimeToWin();
    }

    void Start() {
        // choose a tagger and sync it across all players
        if(PhotonNetwork.IsMasterClient) {
            SetTagger(ChooseRandomPlayer());
        }
    }

    void Update() {
        if(PhotonNetwork.IsMasterClient) {
            UpdatePlayersTimeToWin();
            SyncPlayersTimeToWin();
        }
        // foreach(Player p in PhotonNetwork.PlayerList) {
        //     Debug.Log("player " + p + ", time: " + p.CustomProperties["timeToWin"]);
        // }
    }

    void UpdatePlayersTimeToWin() {
        // change the local dictionary values
        foreach(Player p in PhotonNetwork.PlayerList) {
            if(p == tagger) {
                playersTimeToWin[p] -= Time.deltaTime;
                // if(p.CustomProperties.ContainsKey("timeToWin")) {
                //     Hashtable hash = new Hashtable();
                //     hash.Add("timeToWin", (float) p.CustomProperties["timeToWin"] - Time.deltaTime);
                //     p.SetCustomProperties(hash);
                // }
            }
        }
    }

    void SyncPlayersTimeToWin() {
        // sync the local dictionary values to the custom properties
        foreach(Player p in PhotonNetwork.PlayerList) {
            Hashtable hash = new Hashtable();
            hash.Add("timeToWin", playersTimeToWin[p]);
            p.SetCustomProperties(hash);
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
