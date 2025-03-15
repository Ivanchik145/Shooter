using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public enum Weapons
    {
        None,
        Pistol,
        Rifle,
        Minigun
    }

 
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float ShiftSpeed = 3f; 
    [SerializeField] float jumpForce = 7f; 
    [SerializeField] float ControlSpeed = 10f;
    [SerializeField] GameObject pistol, rifle, miniGun;
    [SerializeField] Image pistolUI, rifleUI, miniGunUI, cursor;
    [SerializeField] AudioSource characterSounds;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] GameObject damageUi;

    public bool dead = false;
    private bool isPistol, isRifle, isMinigun;
    
    bool isGrounded = true;
    float currentSpeed;
    Vector3 direction;
    Rigidbody rb;
    Animator anim;
    float stamina = 5f;
    private int currentHealth = 100;
    private int maxHealth = 100;
    private TextUpdate textUpdate;
    GameManager gameManager;

    [PunRPC]
    public void ChooseWeapon(Weapons weapons)
    {
        anim.SetBool("Pistol", weapons == Weapons.Pistol);
        anim.SetBool("Assault", weapons == Weapons.Rifle);
        anim.SetBool("MiniGun", weapons == Weapons.Minigun);
        anim.SetBool("NoWeapon", weapons == Weapons.None);
        pistol.SetActive(weapons == Weapons.Pistol);
        rifle.SetActive(weapons == Weapons.Rifle);
        miniGun.SetActive(weapons == Weapons.Minigun);
        cursor.enabled = weapons != Weapons.None;
    
    }
    public void GetDamage(int count)
    {
        photonView.RPC("ChangeHealth", RpcTarget.All, count);
    }
    [PunRPC]
    public void ChangeHealth(int count)
    {
        currentHealth += count;
        textUpdate.SetHealth(currentHealth);
        damageUi.SetActive(true);
        Invoke("RemoveDamageUi", 0.1f);
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            dead = true;
            anim.SetBool("Die", true);
            transform.Find("Main Camera").GetComponent<ThirdPersonCamera>().isSpectator = true;
            ChooseWeapon(Weapons.None);
            gameManager.ChangePlayersList();
            this.enabled = false;
        }
    }
    public void GetComponent(int count)
    {
        photonView.RPC("ChangeHealth", RpcTarget.All, count);
    }
    void RemoveDamageUi()
    {
        damageUi.SetActive(false);
    }
    void Start()
    {    
        gameManager = FindObjectOfType<GameManager>();
        gameManager.ChangePlayersList();  
        textUpdate = GetComponent<TextUpdate>();  
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        currentSpeed = movementSpeed;
        ChooseWeapon(Weapons.None);
        if(!photonView.IsMine)
        {
            transform.Find("Main Camera").gameObject.SetActive(false);
            transform.Find("Canvas").gameObject.SetActive(false);
            this.enabled = false;
        }
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        direction = new Vector3(moveHorizontal, 0.0f, moveVertical);
        direction = transform.TransformDirection(direction);
       

        anim.SetBool("Run", direction != Vector3.zero);

        if(direction.x != 0 && direction.z != 0)
        {
            if(!characterSounds.isPlaying && isGrounded)
            {
                characterSounds.Play();
            }

        }    
        if(direction.x == 0 && direction.z == 0)
        {
            characterSounds.Stop();
        }
    


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            isGrounded = false;
            characterSounds.Stop();
            AudioSource.PlayClipAtPoint(jumpSound, transform.position);
            anim.SetBool("Jump", true);
        }


        if(Input.GetKey(KeyCode.LeftControl))
        {
            if(stamina > 0)
            {
                stamina -= Time.deltaTime;
                currentSpeed = ControlSpeed;
            }
            
        
            else
            {
                currentSpeed = movementSpeed;
            }
        }
        else if (!Input.GetKey(KeyCode.LeftControl))
        {            
            stamina += Time.deltaTime;                      
            currentSpeed = movementSpeed;
        }
        if(stamina > 5f)
        {
            stamina = 5f;
        }
        else if (stamina < 0)
        {
            stamina = 0;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = ShiftSpeed;
        }
        else if(Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = movementSpeed;
        }

        if(Input.GetKeyDown(KeyCode.Alpha1) )
        {
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.None);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2) && isPistol)
        {
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.Pistol);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3) && isRifle)
        {
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.Rifle);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4) && isMinigun)
        {
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.Minigun);
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "Pistol":
            if(!isPistol)
            {
                isPistol = true;
                ChooseWeapon(Weapons.Pistol);
                pistolUI.color = Color.white;
                Destroy(other.gameObject);
            }
            break;
            case "Rifle":
            if(!isRifle)
            {
                isRifle = true;
                ChooseWeapon(Weapons.Rifle);
                rifleUI.color = Color.white; 
                Destroy(other.gameObject);
            }
            break;
            case "Minigun":
            if(!isMinigun)
            {
                isMinigun = true;
                ChooseWeapon(Weapons.Minigun);
                miniGunUI.color = Color.white;
                Destroy(other.gameObject);
            }
            break;
        }
        
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(direction.x * currentSpeed, rb.velocity.y, direction.z * currentSpeed);
    }
    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        anim.SetBool("Jump", false);
    }

    
}