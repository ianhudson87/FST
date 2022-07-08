using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    void Awake()
    {
        if(Instance)
        {
            // if another RoomManager exists, delete this one
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject); // don't destroy this object when new scene is loaded
        RoomManager.Instance = this;
    }

    public override void OnEnable() {
        base.OnEnable();
        // subscribe to SceneManager call back
        // now OnSceneLoaded will be called everytime the scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable() {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode){
        if(scene.buildIndex == 1){
            // we are in the game scene

            // instantiate PlayerManager for this player
            GameObject PlayerManagerObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            
        }
    }
}
