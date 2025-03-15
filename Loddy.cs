using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class Loddy : MonoBehaviourPunCallbacks
{
    
    [SerializeField] GameObject startButton;
    
    [SerializeField] TMP_Text ChatText;
    [SerializeField] TMP_InputField InputText;
    [SerializeField] TMP_Text PlayersText;

    private void Log(string message)
    {
        ChatText.text += "\n";;
        ChatText.text += message;
    }

    [PunRPC]
    private void ShowMessage(string message)
    {
        ChatText.text += "\n";
        ChatText.text += message;
    }

    [PunRPC]
    public void ShowPlayers()
    {
        //Обнуляем список игроков и оставляем только надпись Players: 
        PlayersText.text = "Players: ";
        //Запускаем цикл, который перебирает всех игроков на сервере
        foreach (Photon.Realtime.Player otherPlayer in PhotonNetwork.PlayerList)
        {
            //Переходим на новую строку
            PlayersText.text += "\n";
            //Выводим ник игрока
            PlayersText.text += otherPlayer.NickName;
        }
    }

    public void Send()
    {
        if(string.IsNullOrWhiteSpace(InputText.text)) { return; }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            photonView.RPC("ShowMessage", RpcTarget.All, PhotonNetwork.NickName + ": " + InputText.text);
            InputText.text = string.Empty;
        }
        


    
    }

     void RefreshPlayers()
    {
        //управлять вызов будет только Мастер Клиент(игрок, который создал сервер)
        if (PhotonNetwork.IsMasterClient)
        {
            //Вызываем метод ShowPlayers для всех игроков в Лобби
            photonView.RPC("ShowPlayers", RpcTarget.All);
        }
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftLobby()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Log(newPlayer.NickName + "joined the room");
        RefreshPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Log(otherPlayer.NickName + "left the room");
        RefreshPlayers();
        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        RefreshPlayers();
        if(!PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(false);
        }
        //Если у нас есть сохраненный ключ Winner и игрок мастер-клиент
        if (PlayerPrefs.HasKey("Winner") && PhotonNetwork.IsMasterClient)
        {
            //создаем временную переменную, в которую кладем сохраненное имя игрока 
            string winner = PlayerPrefs.GetString("Winner");
            //вызываем метод отображения сообщений и выводим в чат имя игрока, который победил в прошлом матче
            photonView.RPC("ShowMessage", RpcTarget.All, "The last match was won: " + winner);
            //удаляем все ключи, чтобы при перезапуске игры оно не вывелось в чат 
            PlayerPrefs.DeleteAll();
        }
    }

    
}
