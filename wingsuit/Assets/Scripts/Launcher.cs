// Connecting to Photon servers
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] TMP_Dropdown teamSelectorDropdown;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if(!PhotonNetwork.IsConnected) {
            Debug.Log("attempting connect");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        // when you connect to master server
        Debug.Log("joined photon master server");
        PhotonNetwork.JoinLobby(); // need to be in lobby to join/create rooms
        PhotonNetwork.AutomaticallySyncScene = true; // what it sounds like
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("joined photon lobby");
        MenuManager.Instance.OpenMenu("title");
    }

    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text)) {
            return;
        }
        if(string.IsNullOrEmpty(PhotonNetwork.NickName)) {
            PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        // open room menu
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        // set team to whatever the dropdown menu has
        OnChangeTeam();

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent){
            // clear the player list if you were in a room previously
            Destroy(child.gameObject);
        }

        for(int i = 0; i < players.Length; i++) {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        this.startGameButton.SetActive(PhotonNetwork.IsMasterClient); // isMasterClient is true if we are host of this room (the one that created the room, by default).
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // if owner of room leaves, new user becomes the master client. Enable the button for that user.
        this.startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Create room failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(1); // load scene with index 1 for all players
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        // set nickname
        if(string.IsNullOrEmpty(PhotonNetwork.NickName)) {
            PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");
        }

        // join the photon room
        PhotonNetwork.JoinRoom(info.Name);

        // loading menu
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        print("left room");
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent){
            Destroy(trans.gameObject);
        }
        for(int i = 0; i < roomList.Count; i++) {
            if(roomList[i].RemovedFromList){
                // room does not exist any more. maybe because no more players in the room
                continue;
            }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void OnChangeTeam(){
        Debug.Log("set team: " + teamSelectorDropdown.value);
        Hashtable hash = new Hashtable();
        hash.Add("teamIndex", teamSelectorDropdown.value);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
}
