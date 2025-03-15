using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class Rifle : Pistol
{
    // Start is called before the first frame update
    void Start()
    {
       cooldown = 0.2f;
       auto = true; 
       currentAmmo = 30;
       ammoMax = 30;
       ammoBackPack = 60;
    }
}
