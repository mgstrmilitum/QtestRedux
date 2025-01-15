using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

public class QMove : MonoBehaviour
{
    public CharacterController controller;
    [SerializeField] Transform playerView;
    [SerializeField] float gravity = 20f;
    [SerializeField] float friction = 6f;
    [SerializeField] float xMouseSensitivity = 30f;
    [SerializeField] float yMouseSensitivity = 30f;
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
    [SerializeField] float playerFriction = 0f;
    //camera rotations
    float rotX;
    float rotY;

    Vector3 moveDirectionNorm = Vector3.zero;
    Vector3 playerVelocity = Vector3.zero;
    [SerializeField] float playerTopVelocity = 0f;

    //float addspeed;
    //float accelspeed;
    //float currentspeed;
    //float zspeed;
    //float _speed;
    //float dot;
    //float k;
    //float accel;
    //float newspeed;
    //float control;
    //float drop;

    public bool wishJump = false;

    //Player commands, stores wish commands player requests (forward/back, left/right, jump, etc)
    Cmd cmd;

    void Update()
    {
        rotX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity;
        rotY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity;

        if(rotX < -90)
        {
            rotX = -90;
        }
        else if(rotX > 90)
        {
            rotX = 90;
        }

        this.transform.rotation = Quaternion.Euler(0, rotY, 0);
        playerView.rotation = Quaternion.Euler(rotX, rotY, 0);

        QueueJump();

        if(controller.isGrounded)
        {
            GroundMove();
        }
        else
        {
            AirMove();
        }

        controller.Move(playerVelocity * Time.deltaTime);
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


    //private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float maxVelo)
    //{
    //    float projVelo = Vector3.Dot(prevVelocity, accelDir);
    //    float accelVelo = accelerate * Time.fixedDeltaTime;

    //    if(projVelo + accelVelo > maxVelo)
    //    {
    //        accelVelo = maxVelo - projVelo;
    //    }

    //    return prevVelocity + accelDir * accelVelo;
    //}

    //private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    //{
    //    float speed = prevVelocity.magnitude;

    //    if(speed != 0)
    //    {
    //        float drop = speed * friction * Time.fixedDeltaTime;
    //        prevVelocity *= Mathf.Max(speed - drop, 0) / speed;
    //    }

    //    return Accelerate(accelDir, prevVelocity, groundAccelerate, maxGroundVelo);
    //}

    //private Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
    //{
    //    return Accelerate(accelDir, prevVelocity, airAccelerate, maxAirVelo);
    //}
}
