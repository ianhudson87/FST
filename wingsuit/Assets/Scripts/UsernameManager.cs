using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class UsernameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;

    void Start()
    {
        if(PlayerPrefs.HasKey("username")) {
            usernameInput.text = PlayerPrefs.GetString("username");
            OnUsernameInputValueChanged();
        }
    }

    public void OnUsernameInputValueChanged()
    {
        PhotonNetwork.NickName = usernameInput.text;
        PlayerPrefs.SetString("username", usernameInput.text);
    }
}
