using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class PlayerScoresListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI textField;
    Player trackedPlayer;
    public void Initialize(Player trackedPlayer) {
        this.trackedPlayer = trackedPlayer;
        Debug.Log("initialize!");
    }

    void Update() {
        if(trackedPlayer != null && trackedPlayer.CustomProperties["score"] != null) {
            textField.text = trackedPlayer.NickName + ": " + (int)(float)trackedPlayer.CustomProperties["score"];
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(otherPlayer == trackedPlayer) {
            Destroy(this.gameObject);
        }
    }
}
