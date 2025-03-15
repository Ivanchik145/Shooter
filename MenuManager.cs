using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MenuMeneger : MonoBehaviourPunCallbacks
{

    [SerializeField] TMP_Text logText;
    [SerializeField] TMP_InputField inputField;




    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NickName = "Player" + Random.Range(1, 9999);
        Log("Player Name: " + PhotonNetwork.NickName);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();



    }
    public void ChangeName()
    {
        //Считываем то, что написал игрок в поле InputField
        PhotonNetwork.NickName = inputField.text;
        //Выводим в поле игрока его новый никнейм
        Log("New Player name: " + PhotonNetwork.NickName);
    }

    private void Log(string message)
    {
        logText.text += "\n";
        logText.text += message;
    }
    public void CreatRoom()
    {
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 15});
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinedRoom()
    {
        Log("Joined Room");
        PhotonNetwork.LoadLevel("Lobby");
    }
    public override void OnConnectedToMaster()
    {
        Log("Connected to Server.");
    }
}
