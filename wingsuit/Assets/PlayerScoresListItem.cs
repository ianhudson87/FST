using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class PlayerScoresListItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textField;
    Player trackedPlayer;
    public void Initialize(Player trackedPlayer) {
        this.trackedPlayer = trackedPlayer;
        Debug.Log("initialize!");
    }

    void Update() {
        if(trackedPlayer != null && trackedPlayer.CustomProperties["timeToWin"] != null) {
            textField.text = trackedPlayer.NickName + ": " + (int)(float)trackedPlayer.CustomProperties["timeToWin"];
        }
    }
}
