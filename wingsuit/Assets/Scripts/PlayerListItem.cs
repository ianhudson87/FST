using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;
    Player player;
    public void SetUp(Player _player)
    {
        Debug.Log("here");
        player = _player;
        text.text = player.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player == otherPlayer)
        {
            Destroy(this.gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(this.gameObject);
    }
}
