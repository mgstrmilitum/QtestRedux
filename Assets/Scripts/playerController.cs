using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
<<<<<<< Updated upstream

=======
    [SerializeField] int health;
    [SerializeField] bool shieldActive;
    [SerializeField] int maxShield,currentShield;
>>>>>>> Stashed changes
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;

    int jumpCount = 0;

    Vector3 moveDirection;
    Vector3 playerVelocity;

<<<<<<< Updated upstream
    bool isSprinting = false;
=======
    //bool isSprinting = false;
    //bool justJumped = false;

    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading;

>>>>>>> Stashed changes

    // Start is called before the first frame update
    void Start()
    {
<<<<<<< Updated upstream
        
=======
        hpOriginal = health;
        currentShield = maxShield;
        UpdatePlayerUI();
        currentAmmo = maxAmmo;
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {

        //if(currentAmmo <= 0)
        //{
        //    StartCoroutine(Reload());
            
        //}
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.blue);
        Movement();
        Sprint();
    }   

    void Movement()
    {
        if(controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }

      
        moveDirection = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDirection * speed * Time.deltaTime);

        Jump();

        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime;

        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            ++jumpCount;
            playerVelocity.y = jumpSpeed;
        }
    }

    void Sprint()
    {
<<<<<<< Updated upstream
        //WHY DOES HOLDING SPRINT IN AIR CAUSE MOVEMENT FREEZE?
        if(Input.GetButtonDown("Sprint") && controller.isGrounded)
=======
      
        if(Input.GetButtonDown("Sprint"))
>>>>>>> Stashed changes
        {
            speed *= sprintMod;
            //isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            //isSprinting = false;
        }
    }

    void Shoot()
    {
        RaycastHit hit;

        currentAmmo--;

        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
        {
            Debug.Log("Hit a " + hit.collider.name + "!");
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if(dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }

      
    }

    IEnumerator Reload()
    {
        Debug.Log("Reloading...");
        currentAmmo = maxAmmo;
        yield return new WaitForSeconds(reloadTime);
    }
<<<<<<< Updated upstream
=======

    public void TakeDamage(int amount)
    {
        if (shieldActive)
        {
            currentShield -= amount;
            if(currentShield <= 0)
            {
                DeactivateShield();
            }
            UpdatePlayerUI();
            return;
        }

            health -= amount;
            UpdatePlayerUI();
        

        StartCoroutine(FlashDamagePanel());
        if(health <= 0)
        {
            GameManager.Instance.YouLose();
        }
    }

    IEnumerator FlashDamagePanel()
    {
        GameManager.Instance.damagePanel.SetActive(true);
        yield return new WaitForSeconds(.1f);
        GameManager.Instance.damagePanel.SetActive(false);
    }

    void UpdatePlayerUI()
    {
        GameManager.Instance.playerHealthBar.fillAmount = (float)health / hpOriginal;
        GameManager.Instance.playerShieldBar.fillAmount = (float)currentShield / maxShield;
    }

    void DeactivateShield()
    {
        shieldActive = false;
    }

>>>>>>> Stashed changes
}
