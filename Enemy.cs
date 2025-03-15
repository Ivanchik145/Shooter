using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class Enemy : MonoBehaviourPunCallbacks
{
    [SerializeField] protected int health;
    [SerializeField] protected int damage;
    [SerializeField] protected float cooldown;
    [SerializeField] protected float attackDistance;
    [SerializeField] Image healthBar;
    protected GameObject player;
    protected GameObject[] players;
    protected float distance;
    protected Animator anim;
    protected Rigidbody rb;
    protected float timer;

    private bool dead = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        player = FindObjectOfType<PlayerController>().gameObject;
        CheckPlayers();
    }

    void CheckPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Invoke("CheckPlayers", 3f);
    }

    [PunRPC]
    public void ChangeHealth(int count)
    {
        health += count;
        float fillPercent = health / 100f;
        healthBar.fillAmount = fillPercent;
        if(health <= 0)
        {
            dead = true;
            GetComponent<Collider>().enabled = false;
            anim.enabled = true;
            anim.SetBool("Die", true);
            this.enabled = false;
            
        }
    }
    public void GetDamage(int count)
    {
        photonView.RPC("ChangeHealth", RpcTarget.All, count);
    }
    public virtual void Move()
    {

    }

    public virtual void Attack()
    {

    }
    // Update is called once per frame
    private void Update() 
    {
        //Создаём переменную, которая будет хранить минимальную дистанцию
        //Mathf.Infinity - положительная бесконечность
        float closestDistance = Mathf.Infinity;
        //перебираем весь список игроков
        foreach (GameObject closestPlayer in players)
        {
            //высчитываем расстояние до игрока
            float checkDistance = Vector3.Distance(closestPlayer.transform.position, transform.position);
            //если дистанция до игрока меньше, чем дистанция до предыдущего проверенного игрока, то...
            if (checkDistance < closestDistance)
            {
                //если ближайший игрок жив
                if(closestPlayer.GetComponent<PlayerController>().dead == false) 
                {
                    //в переменную player помещаем этого игрока
                    player = closestPlayer;
                    //изменяем значение переменной closestDistance на расстояние до этого игрока
                    closestDistance = checkDistance;
                }
            }
        }
        //проверяем есть ли в переменной player игрок
        //это нужно для того, чтобы не возникало ошибок
        if (player != null)
        {            
            //остальная часть ниже с предыдущих уроков
            distance = Vector3.Distance(transform.position, player.transform.position);
            if (!dead)
            {
                Attack();
            }
        }
    }
    private void FixedUpdate()
    {
        if (!dead && player != null)
        {
            Move();
        }
    }
}
