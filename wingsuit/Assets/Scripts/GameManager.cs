using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance; // singleton
    Player tagger; // don't use this variable, ever!!!!
    public Player SyncedTagger {
        // This value is synced across all clients. Anyone can change it.
        set {
            tagger = value;
            PV.RPC("RPC_SetTagger", RpcTarget.Others, new object[] {tagger});
        }
        get {
            return tagger;
        }
    }
    [SerializeField] PhotonView PV;
    [SerializeField] float startingScore;
    [SerializeField] float tagCooldownTime;

    float tagCooldownTimer = 0f;
    public float SyncedTagCooldownTimer {
        set {
            tagCooldownTimer = value;
            PV.RPC("RPC_SetTagCooldownTimer", RpcTarget.Others, new object[] {tagCooldownTimer});
        }
        get {
            return tagCooldownTimer;
        }
    }
    Dictionary<Player, float> playerScores = new Dictionary<Player, float>(); // only used by master client.
    // Dictionary<int, int> teamPoints = new Dictionary<int, int>();

    void Awake() {
        Instance = this;

        if(PhotonNetwork.IsMasterClient) {
            foreach(Player p in PhotonNetwork.PlayerList) {
                playerScores.Add(p, startingScore);
            }
            SyncPlayerScores();
        }
    }

    override public void OnMasterClientSwitched(Player newMaster) {
        
        if(newMaster == PhotonNetwork.LocalPlayer) {
            // take the scores in the players' custom properties and convert it into a playerScores dictionary
            foreach(Player p in PhotonNetwork.PlayerList) {
                playerScores.Add(p, (float)p.CustomProperties["score"]);
            }
        }
    }

    void Start() {
        // choose a tagger and sync it across all players
        if(PhotonNetwork.IsMasterClient) {
            SyncedTagger = ChooseRandomPlayer();
            NotificationManager.Instance.SendNotification(RpcTarget.All, SyncedTagger.NickName + " is the tagger!");
        }
    }

    void Update() {
        // Debug.Log(PhotonNetwork.PlayerList.Length);
        if(PhotonNetwork.IsMasterClient) {
            UpdatePlayerScores();
            SyncPlayerScores();
            UpdateTagCooldownTimer();
        }
        // foreach(Player p in PhotonNetwork.PlayerList) {
        //     Debug.Log("player " + p + ", time: " + p.CustomProperties["timeToWin"]);
        // }
        
    }

    void UpdateTagCooldownTimer() {
        Debug.Log("cooldown timer: " + SyncedTagCooldownTimer);
        if(SyncedTagCooldownTimer > 0) {
            SyncedTagCooldownTimer = Mathf.Max(0, SyncedTagCooldownTimer - Time.deltaTime);
        }
    }

    void UpdatePlayerScores() {
        // change the local dictionary values
        foreach(Player p in PhotonNetwork.PlayerList) {
            // Debug.Log("change score for" + p + " id: " + p.ActorNumber);
            // Debug.Log("current keys" + playersTimeToWin.Keys);
            if(p == tagger) {
                playerScores[p] -= Time.deltaTime;
            }
        }
    }

    void SyncPlayerScores() {
        // sync the local dictionary values (that are on the master client) to the custom properties
        foreach(Player p in PhotonNetwork.PlayerList) {
            Hashtable hash = new Hashtable();
            hash.Add("score", playerScores[p]);
            p.SetCustomProperties(hash);
        }
    }

    public void ApplyTag(Player tagger, Player taggee) {
        if(SyncedTagCooldownTimer == 0) {
            SyncedTagCooldownTimer = tagCooldownTime;
            NotificationManager.Instance.SendNotification(RpcTarget.All, tagger.NickName + " tagged " + taggee.NickName);
            SyncedTagger = taggee;
            // Debug.Log("cooldown" + SyncedTagCooldownTimer);
        }
    }

    // void SetTagger(Player tagger) {
    //     PV.RPC("RPC_SetTagger", RpcTarget.All, new object[] {tagger});
    // }

    [PunRPC] void RPC_SetTagger(Player tagger) {
        this.tagger = tagger;
    }

    [PunRPC] void RPC_SetTagCooldownTimer(float time) {
        this.tagCooldownTimer = time;
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
