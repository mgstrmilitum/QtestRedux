using System.Collections;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
    [SerializeField] int health;
    [SerializeField] bool shieldActive;
    [SerializeField] int maxShield,currentShield;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] int jumpSpeedMod;


    int jumpCount = 0;
    int hpOriginal;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    //bool isSprinting = false;
    //bool justJumped = false;

    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading;


    // Start is called before the first frame update
    void Start()
    {
        hpOriginal = health;
        currentShield = maxShield;
        UpdatePlayerUI();
        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        ShieldBehavior();
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
      
        if(Input.GetButtonDown("Sprint"))
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

    void ShieldBehavior()
    {
        if(currentShield < 0){currentShield = 0;}
        if(currentShield > maxShield) { currentShield = maxShield;}

    }

    public void AddShield(int amount)
    {
        if(currentShield < maxShield)
        {
            currentShield += amount;
            UpdatePlayerUI();
        }
    }
}
