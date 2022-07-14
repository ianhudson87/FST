using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;
    [SerializeField] PhotonView PV;
    void Awake()
    {
        Instance = this;
    }

    public void SendNotification(RpcTarget target, string message) {
        PV.RPC("RPC_SendNotification", target, new object[] {message});
    }

    [PunRPC] void RPC_SendNotification(string message) {
        HUDManager.Instance.CreateNotification(message);
    }
}
