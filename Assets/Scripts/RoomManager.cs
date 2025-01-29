using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("Connecting to room...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to server!");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom("Test", null, null);
    }
}
