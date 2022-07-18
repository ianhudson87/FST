using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance; // singleton
    [SerializeField] Transform NotificationListContent;
    [SerializeField] GameObject NotificationListItemPrefab;
    [SerializeField] Transform PlayerScoresListContent;
    [SerializeField] GameObject PlayerScoresListItemPrefab;
    [SerializeField] GameObject StartGameMenu;
    PlayerController localPlayerController;
    [SerializeField] Image boostBar;

    void Update() {
        if(localPlayerController == null) {
            // PlayerController[] playerControllers = 
            foreach(PlayerController pc in FindObjectsOfType<PlayerController>()) {
                if(pc.PV.IsMine) {
                    localPlayerController = pc;
                    break;
                }
            }
        }

        BoostBar();
    }

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

    void BoostBar() {
        boostBar.fillAmount = localPlayerController.boostRemaining / localPlayerController.maxBoostAmount;
    }

    // public void DisplayStartGameMenu(bool shown) {

    // }
}
