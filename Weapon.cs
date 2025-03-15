using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPunCallbacks
{

    [SerializeField] protected GameObject particle;
    [SerializeField] protected GameObject cam;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] AudioSource shoot;
    [SerializeField] AudioClip bulletSound, noBulletSound, reload;
    protected bool auto = false;
    protected float cooldown = 0f;

    private float timer = 0;
    //Сколько патронов в обойме
    protected int currentAmmo;
    public float reloadTime;
    //Сколько патронов помещается в обойму
    public int ammoMax;
    //Сколько патронов в запасе
    public int ammoBackPack ;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = ammoMax;
        timer = cooldown;
    }
    private void AmmoTextUpdate()
    {
        ammoText.text = currentAmmo + " / " + ammoBackPack;
    }
    private void Reload()
    {
        if(ammoBackPack == 0) return;
        if(currentAmmo == ammoMax) return;
        //создаем временную переменную, которая высчитывает сколько патронов нам нужно добавить
        int ammoNeed = ammoMax - currentAmmo; 
        //если кол-во патронов в запасе больше или равно кол-ву, которое нам нужно добавить то,
        if (ammoBackPack >= ammoNeed) 
        {
            //из кол-ва патронов в запасе вычитаем кол-во, которое добавляем в обойму
            ammoBackPack -= ammoNeed;
            //в обойму добавляем нужное количество патронов
            currentAmmo += ammoNeed;
        }
        //иначе(если в запасе меньше патронов, чем нам нужно)
        else 
        {
            //добавляем в обойму столько патронов, сколько осталось в запасе
            currentAmmo += ammoBackPack;
            //обнуляем кол-во патронов в запасе
            ammoBackPack = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            AmmoTextUpdate();
            timer += Time.deltaTime;
            if(Input.GetMouseButton(0)) Shoot();
            //если игрок нажмет кнопку R
            if (Input.GetKeyDown(KeyCode.R))
            {
                //если у нас кол-во патронов в обойме НЕ максимальное И, если в запасе патронов больше нуля, то
                
                
                //активируем метод перезарядки с задержкой
                //время задержки можно установить самостоятельно
                Invoke("Reload", reloadTime);
                shoot.PlayOneShot(reload);
                
            } 
        }
        
    }  
    

    public void Shoot()
    {
        if(Input.GetMouseButtonDown(0) || auto)
        {
            if(timer >= cooldown)
            {
                if(currentAmmo > 0)
                {
                    timer = 0;
                    currentAmmo--;
                    OnShoot();
                    shoot.PlayOneShot(bulletSound);
                }
                else
                {
                    shoot.PlayOneShot(noBulletSound);
                }
                
            }
        }
    }

    protected virtual void OnShoot()
    {
       
    }
    
    
}
