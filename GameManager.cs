using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject SpawnsListObject;
    [SerializeField] private GameObject EnemyWalkSpawnList;
    [SerializeField] private GameObject EnemyTurretSpawnList;
    //Ссылка на текст
    [SerializeField] public TMP_Text playersText;
    //Массив, в котором будут хранится все игроки
    GameObject[] players;
    //Список, в котором будут хранится живые игроки
    List<string> activePlayers = new List<string>();

    private Transform[] enemyWalkSpawns;
    private Transform[] enemyTurretSpawns;
    private Transform[] spawns;
    private int spawnIndex = 0;
    int checkPlayers = 0;
    private int previousPlayerCount;
    // Start is called before the first frame update
    void Start()
    {
        enemyWalkSpawns = EnemyWalkSpawnList.GetComponentsInChildren<Transform>();
        enemyTurretSpawns = EnemyTurretSpawnList.GetComponentsInChildren<Transform>();
        spawns = SpawnsListObject.GetComponentsInChildren<Transform>();
        spawnIndex = Random.Range(1, spawns.Length);
        PhotonNetwork.Instantiate("Player", spawns[spawnIndex].position, Quaternion.identity);
        Invoke("SpawnEnemy", 5f);
        previousPlayerCount = PhotonNetwork.PlayerList.Length;
    }
    void Update()
    {
        if (PhotonNetwork.PlayerList.Length < previousPlayerCount)
        {
            ChangePlayersList();
        }
        previousPlayerCount = PhotonNetwork.PlayerList.Length;
    }
    //Метод для кнопки
    public void ExitGame()
    {
        PhotonNetwork.LeaveRoom();
    }
    //Метод фотон, который срабатывает при выходе
    public override void OnLeftRoom()
    {
        //запускаем сцену с Меню игры
        SceneManager.LoadScene(0);
        //обновляем список игроков в матче
        ChangePlayersList();
    }
    [PunRPC]
    public void PlayerList()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        activePlayers.Clear();
        foreach(GameObject player in players)
        {
            //если игрок жив, то
            if(player.GetComponent<PlayerController>().dead == false)
            {
                //добавляем его сетевое имя в список активных игроков
                activePlayers.Add(player.GetComponent<PhotonView>().Owner.NickName);             
            }
        
        }
    
        playersText.text = "Players in game : " + activePlayers.Count.ToString();
                
        //Если у нас остался 1 игрок, то..
        if (activePlayers.Count <= 1 && checkPlayers > 0)
        {        
            PlayerPrefs.SetString("Winner", activePlayers[0]);

            //Ищем всех врагов на карте и кладем их в массив
            var enemies = GameObject.FindGameObjectsWithTag("enemy");
            //Перебираем всех врагов в массиве
            foreach (GameObject enemy in enemies)
            {
                //Всем врагам вычитаем 100HP(если у твоих врагов больше HP, то измени кол-во отнимаемых HP)
                enemy.GetComponent<Enemy>().ChangeHealth(99999);
            }
            Invoke("Enemy", 5f);
            
        }
        checkPlayers++;
    }
    void EndGame()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }
    
    public void ChangePlayersList()
    {
        photonView.RPC("PlayerList", RpcTarget.All);   
    }
    private void SpawnEnemy()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            for(int i = 1; i < enemyWalkSpawns.Length; i++)
            {
                PhotonNetwork.Instantiate("WalkEnemy", enemyWalkSpawns[i].position, Quaternion.identity);
            }
            for(int i = 1; i < enemyTurretSpawns.Length; i++)
            {
                PhotonNetwork.Instantiate("Turret", enemyTurretSpawns[i].position, Quaternion.identity);
            }
        }
    }
    
}
