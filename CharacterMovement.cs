using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    #region Variables

    Target target;

    public ProceduralAnimation proceduralAnimation;
    public Transform Player;
    public float airResistance;
    public movementState state;

    [Header("Movement")]
    public float moveSpeed;   
    public float runningSpeed;
    float hInput;
    float vInput;
    Vector3 moveDirection;
    Vector3 velocity;
    float verticalVelocity = 0f;
    Vector3 playerPosition;

    [Header("Ground Check")]
    public float gravityForce = -9.81f;   
    public bool isGround;
    RaycastHit hit;
    public LayerMask Ground;      
    Vector3 groundPoint;
    public float groundDistance = 0.1f;
    public float feetToGround;
    [Header("Jumping")]
    public float jumpForce;
    public float cooldownJumping;
    public bool isJumping;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    #endregion

    public enum movementState
    {
        idle,
        walking,
        running
    }

    // Start is called before the first frame update
    void Start()
    {
        playerPosition = Player.position;
    }

    void FixedUpdate()
    {
       
        PlayerMovement();
    }

    // Update is called once per frame
    void Update()
    {
        Player.position += moveDirection;
        PlayerMovement();
        Gravity();
        if (Input.GetKey(jumpKey) && isJumping)
        {
            
            Jump();
        }
        else
        {
            isJumping = false;
        }
    }

    void PlayerMovement()
    {
        Vector3 leftFoot = proceduralAnimation.leftFoot.position;
        Vector3 rightFoot = proceduralAnimation.rightFoot.position;
        Physics.Raycast(leftFoot, -Vector3.up, out hit, feetToGround);
        Physics.Raycast(rightFoot, -Vector3.up, out hit, feetToGround);

        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");


        if (Input.GetKey(sprintKey))
        {
            state = movementState.running;
            moveDirection = new Vector3(hInput, 0, vInput) * runningSpeed * Time.deltaTime;
        }
        else
        {
            state = movementState.walking;
            moveDirection = new Vector3(hInput, 0, vInput) * moveSpeed * Time.deltaTime;
        }

    }

    void Gravity()
    {
        Vector3 leftFoot = proceduralAnimation.leftFoot.position;
        Vector3 rightFoot = proceduralAnimation.rightFoot.position;
        if (Physics.Raycast(leftFoot, transform.TransformDirection(-Vector3.up), out hit, groundDistance, Ground) 
            || Physics.Raycast(rightFoot, transform.TransformDirection(-Vector3.up), out hit, groundDistance, Ground))
        {
            Debug.DrawLine(leftFoot, hit.point, Color.yellow);           
            verticalVelocity = 0f;
            Jump();      
        }      

        //if (Physics.Raycast(rightFoot, transform.TransformDirection(-Vector3.up), out hit, groundDistance, Ground))
        //{
        //    Debug.DrawLine(rightFoot, hit.point, Color.yellow);
        //    verticalVelocity = 0f;
        //    Jump();
        //}

        else
        {
            Debug.DrawLine(Player.position, hit.point, Color.black);
            verticalVelocity = gravityForce * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity * Time.deltaTime;       
    }

    void Jump()
    {
        Vector3 leftFoot = proceduralAnimation.leftFoot.position;
        Vector3 rightFoot = proceduralAnimation.rightFoot.position;
        Physics.Raycast(leftFoot, -Vector3.up, out hit, feetToGround);
        Physics.Raycast(rightFoot, -Vector3.up, out hit, feetToGround);
        moveDirection.y = jumpForce * Time.deltaTime;
        Invoke(nameof(restJump), cooldownJumping);
        
    }

    void restJump()
    {
        isJumping = true;
    }

}