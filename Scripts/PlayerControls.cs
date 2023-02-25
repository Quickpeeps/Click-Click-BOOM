using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private Camera mainCam;

    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float dashSpeed = 1f;
    [SerializeField] private float dashTime = 1f;
    [SerializeField] private LayerMask jumpableGround;
    private bool isGrounded;
    private bool canDash;
    private bool isDashing;
    private Vector2 dashDirection;

    [Header("HP Variables")]
    [SerializeField] private int maxHP = 5;
    [SerializeField] private float dmgCooldown = 1f;
    private int currentHP;
    private bool isRecovering;

    [Header("Combat Variables")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform crossHair;
    [SerializeField] private Transform projectileTransform;
    [SerializeField] private Transform rotationPoint;
    [SerializeField] private float timetoReload;
    [SerializeField] private float maxAmmo;
    [SerializeField] private float recoilForce;
    private float currentAmmo;
    private float reloadTimer;
    private bool combatReady;
    private Vector3 mousePosition;
    private Vector3 recoilDirection;

    private void Start()
    {
        mainCam = Camera.main;
        currentHP = maxHP;
        currentAmmo = maxAmmo;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        Debug.Log("Your current HP is " + currentHP);
    }

    private void Update()
    {
        Movement();
        Combat(); 
    }

    void Movement()
    {
        //Keeps the player at the center of the screen
        mainCam.transform.position = new Vector3(gameObject.transform.position.x, mainCam.transform.position.y, mainCam.transform.position.z);

        //Left and right movement
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
       
        //Checks if sprite is grounded
        bool IsGrounded()
        {
            return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        }

        //Jump function
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        //Dash function
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            isDashing = true;
            canDash = false;
            dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
            StartCoroutine(StopDashing());
        }
        if (dashDirection == Vector2.zero)
        {
            dashDirection = new Vector2(transform.localScale.x, 0);
        }
        if (isDashing)
        {
            rb.velocity = dashDirection.normalized * dashSpeed;
            return;
        }
        if (IsGrounded())
        {
            canDash = true;
        }
        IEnumerator StopDashing()
        {
            yield return new WaitForSeconds(dashTime);
            isDashing = false;
        }
     
    }

    void Combat()
    {
        //Sets the mouse position as the mouse's current position 
        mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);

        //Rotate the anchorpoint towards the mouse's position
        Vector3 rotation = mousePosition - rotationPoint.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        rotationPoint.rotation = Quaternion.Euler(0, 0, rotZ);

        //Pew Pew
        if (Input.GetMouseButtonDown(0) && currentAmmo != 0)
        {
            //currently only adds force upwards, doesnt recoil the character side to side
            currentAmmo = currentAmmo - 1;
            Instantiate(projectile, projectileTransform.position, Quaternion.identity);
            recoilDirection = (crossHair.position - transform.position) *-1;
            rb.AddRelativeForce(recoilDirection * recoilForce, ForceMode2D.Impulse);
        }

        //Reload if currentAmmo reaches 0
        if (currentAmmo == 0)
        {
            reloadTimer += Time.deltaTime;

            if (reloadTimer > timetoReload)
            {
                currentAmmo = maxAmmo;
                reloadTimer = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Player took damage
        if (collision.tag == "Danger" & isRecovering == false)
        {
            isRecovering = true;
            currentHP = currentHP - 1;
            Debug.Log("Your current HP is " + currentHP);
            StartCoroutine(DamageCooldown());
        }
        //Player has died
        if (currentHP == 0)
        {
            Debug.Log("The player has died");
        }
        //Stops the player from getting spammed with damage
        IEnumerator DamageCooldown()
        {
            yield return new WaitForSeconds(dmgCooldown);
            isRecovering = false;
        }

    }

}

