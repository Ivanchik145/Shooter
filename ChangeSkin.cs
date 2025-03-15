using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeSkin : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] GameObject[] body;
    [SerializeField] GameObject[] head;
    [SerializeField] bool isMale;



    public void Replace(bool value)
    {
        body[0].SetActive(value);
        head[0].SetActive(value);
        body[1].SetActive(!value);
        head[1].SetActive(!value);
    }

    // Start is called before the first frame update
    void Start()
    {
        Replace(isMale);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(isMale);
        }
        else
        {
            isMale = (bool)stream.ReceiveNext();
            Replace(isMale);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                isMale = !isMale;
                Replace(isMale);
            }
        }
    }
}
