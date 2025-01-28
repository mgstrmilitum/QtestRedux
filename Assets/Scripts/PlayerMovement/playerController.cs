using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class playerController : MonoBehaviour, IDamage, IOpen
{
    [Header("Components")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject gunModel;

    [Header("Stats")]
    [SerializeField] int speed;
    [Range(1,10)][SerializeField] int sprintMod;
    [SerializeField] int crouchMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] float timeForJumpBoost;
    [SerializeField] int gravity;
    [Range(1, 10)][SerializeField] int health;
    [Range(1, 10)][SerializeField] int armor;
    [Range(1, 10)][SerializeField] int maxShield;
    int shootDamage;
    int shootDistance;
    int jumpSpeedMod;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform playerCenter;
    [SerializeField] float groundingDistance;
    [SerializeField] float shootRate;
    [SerializeField] Material mat;
    [SerializeField] AudioSource aud;
    [SerializeField] float xMouseSensitivity;
    [SerializeField] float yMouseSensitivity;
    float rotX;
    float rotY;
    [SerializeField] Transform playerView;
    bool shieldActive;

    [Header("Guns")]
    [SerializeField] List<GunStats> gunList = new List<GunStats>();

    [Header("Audio")]
    [SerializeField] AudioClip[] audSteps;
    [SerializeField][Range(0, 1)] float audStepsVol;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] [Range(0,1)] float audHurtVol;
    [SerializeField] AudioClip[] audJump;
    [SerializeField][Range(0, 1)] float audJumpVol;

    [SerializeField]int jumpCount = 0;
    int hpOriginal;
    float timeGrounded = 0f;
    float shootTimer = 0f;
    int gunListPos;
    public LayerMask groundLayer;
    public Vector3 moveDirection;
    public Vector3 playerVelocity;

    bool isSprinting = false;
    bool isCrouching = false;
    bool isPlayingSteps;
    bool justLanded = false;
    public bool isLanded = false;

    bool hasQuad;
    public bool hasInvis;
    bool isPaused = false;
    bool invertLook = false;

    Color myColor;


    // Start is called before the first frame update
    void Start()
    {
        myColor = mat.color;
        hpOriginal = health;
        maxShield = armor;
        shieldActive = true;
        UpdatePlayerUI();
        shootTimer = shootRate;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(controller.isGrounded);
        if (!GameManager.Instance.isPaused)
        {

            rotX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity;
            rotY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity;

            if (rotX < -90)
            {
                rotX = -90;
            }
            else if (rotX > 90)
            {
                rotX = 90;
            }

            this.transform.rotation = Quaternion.Euler(0, rotY, 0);
            if (invertLook)
            {
                playerView.rotation = Quaternion.Euler(-rotX, rotY, 0);
            }
            else
            {
                playerView.rotation = Quaternion.Euler(rotX, rotY, 0);
            }
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.blue);
            Movement();
            Sprint();
            Crouch();
            shootTimer += Time.deltaTime;
            if (hasInvis)
            {
                StartCoroutine(Invisibility());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //PowerUp powerUp = other.GetComponent<PowerUp>();
        //if (other.CompareTag("PowerUp"))
        //{
        //    if(powerUp.powerType == PowerType.QuadDamage)
        //    {
        //        hasQuad = true;
        //    }
        //    else if(powerUp.powerType == PowerType.MegaHealth)
        //    {
        //        health += 100;
        //    }
        //    else if(powerUp.powerType == PowerType.Invis)
        //    {
        //        hasInvis = true;
        //    }
        ////}
        //else if(other.CompareTag("Wall"))
        //{

        //}
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
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;

            if (moveDirection.magnitude > 0.3f && !isPlayingSteps)
            {
                StartCoroutine(PlaySteps());
            }
        }

        //moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        controller.Move(moveDirection * speed * Time.deltaTime);


        Jump();

        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime;

        if(Input.GetButton("Fire1") && gunList.Count > 0 && shootTimer >= shootRate)
        {
            Shoot();
        }

    }

    IEnumerator PlaySteps()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);

        isPlayingSteps = false;
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            Debug.Log("JUMP!");
            ++jumpCount;
            //rb.AddForce(Vector3.up * jumpSpeed);
            playerVelocity.y = jumpSpeed;
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
    }

    void Sprint()
    {
        //WHY DOES HOLDING SPRINT IN AIR CAUSE MOVEMENT FREEZE?
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            speed /= crouchMod;
            isCrouching = true;
            controller.height = 0.5f;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            speed *= crouchMod;
            isCrouching = false;
            controller.height = 2;
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        shootTimer = 0f;
        aud.PlayOneShot(gunList[gunListPos].shootSound[Random.Range(0, gunList[gunListPos].shootSound.Length)], gunList[gunListPos].shootSoundVolume);
        StartCoroutine(FlashMuzzle());

        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
        {
            Debug.Log("Hit a " + hit.collider.name + "!");
            Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if(dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }
        //Do feedback as quickly as you can!
    }

    IEnumerator FlashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(.05f);
        muzzleFlash.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        if (shieldActive)
        {
            armor -= amount;
            if (armor <= 0)
            {
                DeactivateShield();
            }
            UpdatePlayerUI();
            return;
        }

        health -= amount;
        UpdatePlayerUI();


        StartCoroutine(FlashDamagePanel());
        if (health <= 0)
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
        GameManager.Instance.playerShieldBar.fillAmount = (float)armor / maxShield;
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

        void DeactivateShield()
        {
            shieldActive = false;
        }

        public void AddShield(int amount)
        {
            if (armor < maxShield)
            {
                armor += amount;
                UpdatePlayerUI();
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
        gunList.Add(gun); //add a check to make sure we don't already have this gun
        gunListPos = gunList.Count - 1;
        //ChangeGun();
    }

    //void SelectGun()
    //{
    //    if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1) //possibly add scroll up to zero at this point
    //    {
    //        gunListPos++;
    //        ChangeGun();
    //    }
    //    else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
    //    {
    //        gunListPos--;
    //        ChangeGun();
    //    }
    //}

    //void ChangeGun()
    //{
    //    //change gun-related stats
    //    shootDamage = gunList[gunListPos].shootDamage;
    //    shootDistance = gunList[gunListPos].shootDistance;
    //    shootRate = gunList[gunListPos].shootRate;

    //    //change the gun model
    //    gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
    //    gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;

    //}
}
