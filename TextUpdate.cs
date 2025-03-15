using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class TextUpdate : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] TMP_Text playerNickName;


    private int health = 100;

    [PunRPC]
    public void RotateName()
    {
        playerNickName.GetComponent<RectTransform>().localScale = new Vector3(-1, 1, 1);
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
        playerNickName.text = photonView.Controller.NickName + "\n" + health.ToString();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            
            playerNickName.text = photonView.Controller.NickName + "\n" + health.ToString();
            photonView.RPC("RotateName", RpcTarget.Others);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            playerNickName.text = photonView.Controller.NickName + "\n" + "Health: " + health.ToString();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
