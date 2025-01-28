using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class playerController : MonoBehaviour, IDamage, IPickup
{
    [Header("-----Components-----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform playerCenter;
    [SerializeField] float groundingDistance;
    [SerializeField] Material mat;
    [SerializeField] AudioSource audio;
    [Header("-----Stats-----")]
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int crouchMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] float timeForJumpBoost;
    [SerializeField] int gravity;
    [SerializeField] int health;
    [SerializeField] int armor;
    [SerializeField] int jumpSpeedMod;
    [SerializeField] int jumpCount = 0;
    [Header("-----Guns-----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [Header("-----Audio-----")]
    [SerializeField] AudioClip[] audioHurt;
    [SerializeField] float audioHurtVolume;
    [SerializeField] AudioClip[] audioSteps;
    [SerializeField] float audioStepsVolume;
    
    
    int hpOriginal;
    float timeGrounded = 0f;
    int gunListPos;

    public LayerMask groundLayer;
    public Vector3 moveDirection;
    public Vector3 playerVelocity;

    bool isSprinting = false;
    bool justLanded = false;
    public bool isLanded = false;
    float shootTimer;

    bool hasQuad;
    public bool hasInvis;
    bool isPlayingSteps;

    Color myColor;


    // Start is called before the first frame update
    void Start()
    {
        myColor = mat.color;
        hpOriginal = health;
        UpdatePlayerUI();
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.blue);

        if(!GameManager.Instance.isPaused)
        {
            Movement();
            SelectGun();
        }
        
        Movement();
        Crouch();
        if(hasInvis)
        {
            StartCoroutine(Invisibility());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PowerUp powerUp = other.GetComponent<PowerUp>();
        if (other.CompareTag("PowerUp"))
        {
            if(powerUp.powerType == PowerType.QuadDamage)
            {
                hasQuad = true;
            }
            else if(powerUp.powerType == PowerType.MegaHealth)
            {
                health += 100;
            }
            else if(powerUp.powerType == PowerType.Invis)
            {
                hasInvis = true;
            }
        }
        else if(other.CompareTag("Wall"))
        {

        }
        //if (other.CompareTag("Floor"))
        //{
        //    jumpCount = 0;
        //    playerVelocity.y = 0f;
        //    timeGrounded = 0f;
        //    isLanded = true;
        //}
    }

    private void OnTrigger(Collider other)
    {
        if(other.CompareTag("Floor"))
        {
            timeGrounded += Time.deltaTime;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        timeGrounded = 0f;
    }

    void Movement()
    {
        StartCoroutine(DecreaseMegaHealthArmor());
        //if (controller.isGrounded)
        //{
        //    jumpCount = 0;
        //    playerVelocity = Vector3.zero;
        //    //timeGrounded = 0f;
        //}

        timeGrounded += Time.deltaTime;
        //moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        //controller.Move(moveDirection * speed * Time.deltaTime);
        //rb.AddForceAtPosition(moveDirection * speed * Time.deltaTime, playerCenter.position, ForceMode.Force);
        rb.linearVelocity = moveDirection * speed;

        //if (timeGrounded <= timeForJumpBoost)
        //{
        //    justLanded = true;
        //    //StartCoroutine(QuickJump());
        //}
        //else
        //{
        //    Jump();
        //}

        Jump();
        //controller.Move(playerVelocity * Time.deltaTime);
        float yVelo = rb.linearVelocity.y;
        yVelo -= gravity * Time.deltaTime;
        if (!isLanded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, yVelo, rb.linearVelocity.z);
        }
        rb.angularVelocity = Vector3.zero;
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    IEnumerator playSteps()
    {
        isPlayingSteps = true;

        audio.PlayOneShot(audioSteps[Random.Range(0, audioSteps.Length)], audioStepsVolume);

        if(!isSprinting)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }
    }

    void Jump()
    {
        RaycastHit hit;
        if(Physics.Raycast(playerCenter.position, Vector3.down, out hit, groundingDistance))
        {
            if(hit.collider.CompareTag("Floor"))
            {
                isLanded = true;
                jumpCount = 0;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                Debug.Log("Touching the ground!");
            }
        }

        if(isLanded && Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            ++jumpCount;
            isLanded = false;
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            //rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpSpeed, rb.linearVelocity.z);
        }
    }

    void Sprint()
    {
        //WHY DOES HOLDING SPRINT IN AIR CAUSE MOVEMENT FREEZE?
        //if(Input.GetButtonDown("Sprint"))
        //{
        //    speed *= sprintMod;
        //    isSprinting = true;
        //}
        //else if (Input.GetButtonUp("Sprint"))
        //{
        //    speed /= sprintMod;
        //    isSprinting = false;
        //}
    }

    void Crouch()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed /= crouchMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed *= crouchMod;
            isSprinting = false;
        }
    }

    void Shoot()
    {
        shootTimer = 0;
        audio.PlayOneShot(gunList[gunListPos].shootSound[Random.Range(0, gunList[gunListPos].shootSound.Length)], gunList[gunListPos].shootSoundVol);

        RaycastHit hit;
        
        if(gunList.Count > 0)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
            {
                Debug.Log("Hit a " + hit.collider.name + "!");

                Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);
                StartCoroutine(FlashMuzzleFire());

                IDamage dmg = hit.collider.GetComponent<IDamage>();
                if (dmg != null)
                {
                    dmg.TakeDamage(shootDamage);
                }
            }
        }
        //Do feedback as quickly as you can!
    }

    IEnumerator FlashMuzzleFire()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        UpdatePlayerUI();
        audio.PlayOneShot(audioHurt[Random.Range(0, audioHurt.Length)], audioHurtVolume);

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
    }

    public void GetGunStats(GunStats gun)
    {
        shootDamage = gun.shootDamage;
        shootDistance = gun.shootDistance;
        shootRate = gun.shootRate;
    }

    IEnumerator QuickJump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax && justLanded == true)
        {
            ++jumpCount;
            playerVelocity.y = jumpSpeed * jumpSpeedMod;
        }
        justLanded = false;
        yield return new WaitForSeconds(timeForJumpBoost);
    }

    IEnumerator AccelMovement()
    {
        yield return null;
    }

    IEnumerator DecelMovement()
    {
        yield return null;
    }

    IEnumerator DecreaseMegaHealthArmor()
    {
        while(health > 100 || armor > 100)
        {
            if (health > 100)
            {
                --health;
            }
            if(armor > 100)
            {
                --armor;
            }
            yield return new WaitForSeconds(1f);
        }
        
    }

    IEnumerator Invisibility()
    {
        while (hasInvis)
        {
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, .15f);
            yield return new WaitForSeconds(3f);
            hasInvis = false;
        }
        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1f);
    }

    IEnumerator QuadDamage()
    {
        yield return new WaitForSeconds(0f);
    }

    public void GetGunStats(GunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

        ChangeGun();
    }

    void SelectGun()
    {
        if(Input.GetAxis("Mouse Scrollwheel") > 0 && gunListPos < gunList.Count-1)
        {
            gunListPos++;
        }
        else if(Input.GetAxis("Mouse Scrollwheel") < 0 && gunListPos > 0)
        { 
            gunListPos--;
        }
    }

    void ChangeGun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootDistance = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }
}
