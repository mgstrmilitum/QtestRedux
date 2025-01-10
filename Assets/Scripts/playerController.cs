using System.Collections;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int crouchMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
    [SerializeField] int health;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] int jumpSpeedMod;


    int jumpCount = 0;
    int hpOriginal;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    bool isSprinting = false;
    bool justJumped = false;


    // Start is called before the first frame update
    void Start()
    {
        hpOriginal = health;
        UpdatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.blue);
        Movement();
        Crouch();
    }

    void Movement()
    {
        if(controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }

        //moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
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
        RaycastHit hit;
        
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
        {
            Debug.Log("Hit a " + hit.collider.name + "!");
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if(dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }

        //Do feedback as quickly as you can!
    }

    public void TakeDamage(int amount)
    {
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
    }

    IEnumerator QuickJump()
    {
        yield return new WaitForSeconds(.05f);
    }
}
