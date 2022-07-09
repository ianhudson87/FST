using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class NameDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text textField;
    [SerializeField] PhotonView playerPV;
    void Awake()
    {
        Debug.Log(playerPV.Owner.NickName);
        if(playerPV.IsMine) {
            // don't show your own nametag to yourself
            gameObject.SetActive(false);
        }
        textField.text = playerPV.Owner.NickName;
        if((int)playerPV.Owner.CustomProperties["teamIndex"] == 0) {
            textField.color = new Color32(255, 0, 0, 255);
        }
        else if((int)playerPV.Owner.CustomProperties["teamIndex"] == 1) {
            textField.color = new Color32(0, 0, 255, 255);
        }
    }
}
