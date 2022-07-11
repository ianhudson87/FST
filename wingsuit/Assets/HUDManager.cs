using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance; // singleton
    [SerializeField] Transform NotificationListContent;
    [SerializeField] GameObject NotificationListItemPrefab;

    void Awake() {
        Debug.Log("instance set");
        Instance = this;
    }
    public void CreateNotification(string notifText) {
        GameObject notif = GameObject.Instantiate(NotificationListItemPrefab, NotificationListContent);
        notif.GetComponent<NotificationListItem>().SetText(notifText);
    }
}
