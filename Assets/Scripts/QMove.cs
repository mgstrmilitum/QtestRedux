using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;




struct Cmd
{
    public float forwardMove;
    public float rightMove;
    public float upMove;
}

struct PlayerStats
{
    public int shotsFired;
    public int shotsHit;
    public int numSuicides;
}

struct AmmoCount
{
    public int numBullets;
    public int numShells;
    public int numGrenades;
    public int numRockets;
    public int numLightning;
    public int numSlugs;
    public int numCells;

}

public class QMove : MonoBehaviour, IDamage , IPickup, IOpen
{
    //movement/control related
    public CharacterController controller;
    [SerializeField] Transform playerView;
    [SerializeField] float gravity = 20f;
    [SerializeField] float friction = 6f;
    [SerializeField] public float xMouseSensitivity = 2f;
    [SerializeField] public float yMouseSensitivity = 2f;
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float runAcceleration = 14f;
    [SerializeField] float runDeacceleration = 10f;
    [SerializeField] float airAcceleration = 2f;
    [SerializeField] float airDeacceleration = 2f;
    [SerializeField] float airControl = .3f;
    [SerializeField] float sideStrafeAccel = 50f;
    [SerializeField] float sideStrafeSpeed = 1f;
    [SerializeField] float jumpSpeed = 8f;
    [SerializeField] bool holdJumpToBhop = false;

    [SerializeField] public bool invertLook = false;

    [SerializeField] float playerFriction = 0f;
    public bool wishJump = false;
    Vector3 moveDirectionNorm = Vector3.zero;
    public Vector3 playerVelocity = Vector3.zero;
    [SerializeField] float playerTopVelocity = 0f;
    [SerializeField] int crouchSpeedFactor; //2 halves speed, 4 quarters speed, etc
    [SerializeField] float crouchScaleFactor; //how much character controller component is shrunk (1 halves character, .5 is 1/4th size, etc)
    bool isSprinting = false;
    [SerializeField] float sprintSpeedFactor; //2 doubles run speed, 4 quadruples, etc
    bool isSliding = false;


    //health/shield info
    int originalHealth;
    public bool shieldActive;
    [SerializeField] int health;
    [SerializeField] int maxShield, currentShield;

    //camera rotations
    float rotX;
    float rotY;

    //dev mode variables
    int frameCount = 0;
    float dt = 0f;
    float fps = 0f;
    [SerializeField] float fpsDisplayRate = 4f;
    public GUIStyle style;

    //Player commands, stores wish commands player requests (forward/back, left/right, jump, etc)
    Cmd cmd;

    void Start()
    {
        AssignSettings();
        originalHealth = health;
        currentShield = maxShield;
        UpdatePlayerUI();
        //working on dev mode (show movement meta data, turn ammo infinite, 999 shields/health, etc)
    }
    void Update()
    {
        if(GameManager.Instance.devMode)
        {
            ++frameCount;
            dt += Time.deltaTime;

            if(dt > 1.0 / fpsDisplayRate)
            {
                fps = Mathf.Round(frameCount / dt);
                frameCount = 0;
                dt -= 1f / fpsDisplayRate;
            }
        }
        ShieldBehavior();
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

            QueueJump();

            if (controller.isGrounded)
            {
                GroundMove();
            }
            else
            {
                AirMove();
            }

            controller.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void SetMovementDir()
    {
        cmd.forwardMove = Input.GetAxisRaw("Vertical");
        cmd.rightMove = Input.GetAxisRaw("Horizontal");
    }

    private void QueueJump()
    {
        if(holdJumpToBhop)
        {
            wishJump = Input.GetButton("Jump");
            return;
        }

        if(Input.GetButtonDown("Jump") && !wishJump)
        {
            wishJump = true;
        }
        if(Input.GetButtonUp("Jump"))
        {
            wishJump = false;
        }
    }

    private void AirMove()
    {
        Vector3 wishdir;
        float wishVelo = airAcceleration;
        float accel;

        SetMovementDir();

        wishdir = new Vector3(cmd.rightMove, 0, cmd.forwardMove);
        wishdir = transform.TransformDirection(wishdir);

        float wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        wishdir.Normalize();
        moveDirectionNorm = wishdir;

        //air control
        float wishspeed2 = wishspeed;
        if(Vector3.Dot(playerVelocity, wishdir) < 0)
        {
            accel = airDeacceleration;
        }
        else
        {
            accel = airAcceleration;
        }

        //if the player is ONLY strafing left/right
        if(cmd.forwardMove == 0 && cmd.rightMove != 0)
        {
            if(wishspeed > sideStrafeSpeed)
            {
                wishspeed = sideStrafeSpeed;
            }
            accel = sideStrafeAccel;
        }

        Accelerate(wishdir, wishspeed, accel);
        if(airControl > 0)
        {
            AirControl(wishdir, wishspeed2);
        }

        playerVelocity.y -= gravity * Time.deltaTime;
    }

    private void AirControl(Vector3 wishdir, float wishspeed)
    {
        float zspeed, speed, dot, k;

        //Can't control movement if not moving forward/back (maybe not needed)
        if (Mathf.Abs(cmd.forwardMove) < .001f || Mathf.Abs(wishspeed) < .001f)
        {
            return;
        }

        zspeed = playerVelocity.y;
        playerVelocity.y = 0;

        //next two lines are equivalent to idTech's VectorNormalize()
        speed = playerVelocity.magnitude;
        playerVelocity.Normalize();

        dot = Vector3.Dot(playerVelocity, wishdir);
        k = 32;
        k *= airControl * dot * dot * Time.deltaTime;

        //change direction while slowing
        if(dot > 0)
        {
            playerVelocity.x = playerVelocity.x * speed + wishdir.x;
            playerVelocity.y = playerVelocity.y * speed + wishdir.y;
            playerVelocity.z = playerVelocity.z * speed + wishdir.z;

            playerVelocity.Normalize();
            moveDirectionNorm = playerVelocity;
        }

        playerVelocity.x *= speed;
        playerVelocity.y = zspeed; // Note this line
        playerVelocity.z *= speed;
    }

    //called when engine determines player is on ground
    private void GroundMove()
    {
        Vector3 wishdir;

        if(!wishJump)
        {
            ApplyFriction(1f);
        }
        else
        {
            ApplyFriction(0f);
        }

        SetMovementDir();

        wishdir = new Vector3(cmd.rightMove, 0, cmd.forwardMove);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();
        moveDirectionNorm = wishdir;

        var wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        #region Sprint
        if(Input.GetButton("Sprint"))
        {
            wishspeed *= sprintSpeedFactor;
        }

        if (Input.GetButtonUp("Sprint"))
        {
            wishspeed /= sprintSpeedFactor;
        }

        #endregion

        #region Crouch/Uncrouch
        if (Input.GetButton("Crouch"))
        {
            wishspeed /= crouchSpeedFactor;
            controller.height = crouchScaleFactor;
            Debug.Log("Crouching!");

        }
        if (Input.GetButtonUp("Crouch"))
        {
            wishspeed *= crouchSpeedFactor;
            controller.height = 2f;
            Debug.Log("Standing again!");
        }
        #endregion

        Accelerate(wishdir, wishspeed, runAcceleration);

        // Reset the gravity velocity
        playerVelocity.y = -gravity * Time.deltaTime;

        if (wishJump)
        {
            playerVelocity.y = jumpSpeed;
            wishJump = false;
        }
    }

    //applies friction to player (for both airbone and grounded players, does it need to be?)
    private void ApplyFriction(float t)
    {
        Vector3 vec = playerVelocity;
        float speed, newspeed, control, drop;

        vec.y = 0f;
        speed = vec.magnitude;
        drop = 0f;

        if(controller.isGrounded)
        {
            control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * friction * t * Time.deltaTime;
        }

        newspeed = speed - drop;
        playerFriction = newspeed;

        if (newspeed < 0)
        {
            newspeed = 0;
        }
        if (speed > 0)
        {
            newspeed /= speed;
        }

        playerVelocity.x *= newspeed;
        playerVelocity.z *= newspeed;
    }

    private void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    {
        float addspeed;
        float accelspeed;
        float currentspeed;

        currentspeed = Vector3.Dot(playerVelocity, wishdir);
        addspeed = wishspeed - currentspeed;

        if (addspeed <= 0)
        {
            return;
        }

        accelspeed = accel * Time.deltaTime * wishspeed;

        if (accelspeed > addspeed)
        {
            accelspeed = addspeed;
        }

        playerVelocity.x += accelspeed * wishdir.x;
        playerVelocity.z += accelspeed * wishdir.z;
    }

    IEnumerator FlashDamagePanel()
    {
        GameManager.Instance.damagePanel.SetActive(true);
        yield return new WaitForSeconds(.1f);
        GameManager.Instance.damagePanel.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        if (shieldActive)
        {
            currentShield -= amount;
            if (currentShield <= 0)
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
    void UpdatePlayerUI()
    {
        GameManager.Instance.playerHealthBar.fillAmount = (float)health / originalHealth;
        GameManager.Instance.playerShieldBar.fillAmount = (float)currentShield / maxShield;
    }
    void DeactivateShield()
    {
        shieldActive = false;
    }
    void ShieldBehavior()
    {
        if (currentShield != 0) { shieldActive = true;}
        if (currentShield < 0) { currentShield = 0; }
        if (currentShield > maxShield) { currentShield = maxShield; }

    }
    public void AddShield(int amount)
    {
        if (currentShield < maxShield)
        {
            currentShield += amount;
            UpdatePlayerUI();
        }
    }

    //Comment this function out if in Dev Mode!!!
    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 400, 100), "FPS: " + fps, style);
        var ups = controller.velocity;
        ups.y = 0;
        GUI.Label(new Rect(0, 15, 400, 100), "Speed: " + Mathf.Round(ups.magnitude * 100) / 100 + "ups", style);
        GUI.Label(new Rect(0, 30, 400, 100), "Top Speed: " + Mathf.Round(playerTopVelocity * 100) / 100 + "ups", style);
    }

   public void AddHealth(int amount)
    {
        health += amount;
        if (health > 100) { health = 100; }
        UpdatePlayerUI();
    }
    public void OnPickup(Collider other)
    {

    }

    public void AdjustSens(float amount)
    {
        xMouseSensitivity = amount;
        yMouseSensitivity = amount;
    }

    public void InvertLook ()
    {
        invertLook = !invertLook;
    }

public void AssignSettings()
    {
        xMouseSensitivity = PlayerPrefs.GetFloat("mouseSens", 2);
        yMouseSensitivity = PlayerPrefs.GetFloat("mouseSens", 2);
        invertLook = (PlayerPrefs.GetInt("invertAxis", 0) != 0);
    }
}
