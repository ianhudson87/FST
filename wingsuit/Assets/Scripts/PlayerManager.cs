using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject controllerObj; // the player controller that we create

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if(PV.IsMine){
            CreateController();
            
        }
    }

    void CreateController()
    {
        Debug.Log("Instantiate Player Controller");
        // Instantiate player controller
        Debug.Log("custom props" + (int) PV.Owner.CustomProperties["teamIndex"]);
        // Transform spawnpoint = SpawnManager.instance.GetSpawnPoint((int) PV.Owner.CustomProperties["teamIndex"]);
        Debug.Log("got here");
        // controllerObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        controllerObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(100,100,100), Quaternion.identity, 0, new object[] { PV.ViewID });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controllerObj);
        CreateController(); // respawn
    }

    public void Respawn()
    {
        PhotonNetwork.Destroy(controllerObj);
        CreateController(); // respawn
    }

    // public void SetCurrentLoadout(Loadout loudout) {
    //     currentLoadout = loudout;
    // }
}
