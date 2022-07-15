using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance; // singleton
    [SerializeField] Transform NotificationListContent;
    [SerializeField] GameObject NotificationListItemPrefab;
    [SerializeField] Transform PlayerScoresListContent;
    [SerializeField] GameObject PlayerScoresListItemPrefab;
    [SerializeField] GameObject StartGameMenu;

    void Awake() {
        Debug.Log("instance set");
        Instance = this;

        // player scores
        foreach(Player p in PhotonNetwork.PlayerList) {
            GameObject.Instantiate(PlayerScoresListItemPrefab, PlayerScoresListContent).GetComponent<PlayerScoresListItem>().Initialize(trackedPlayer: p);
        }
    }
    public void CreateNotification(string notifText) {
        GameObject notif = GameObject.Instantiate(NotificationListItemPrefab, NotificationListContent);
        notif.GetComponent<NotificationListItem>().SetText(notifText);
    }

    public void DisplayStartGameMenu(bool shown) {

    }
}
