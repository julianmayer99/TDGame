using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControler : MonoBehaviour
{
    private Rigidbody rb;

    //States
    enum Status
    {
        Move,
        Carry,
        Destroy
    }
    private Status currentState = Status.Move;

    //Movement
    private Vector3 input_movement = new Vector3();

    private float move_accel = 50f; //In units per second
    private float move_accelMin = 0.1f;
    private float move_maxSpeed = 22f;
    private float move_rotateSpeed = 8f;
    private Vector3 move_speedDesired = new Vector3();

    //Animating
    private Animator animator;

    private float moveAnim_walking_speedThreshhold = 0.2f;
    private float moveAnim_running_speedThreshhold = 18f;
    [SerializeField]
    private GameObject anim_flame_main;
    [SerializeField]
    private GameObject anim_flame_left;
    [SerializeField]
    private GameObject anim_flame_right;

    //Carrying

    //Destroy
    private int destroy_shotCount;
    private int destroy_shotCountMax = 2;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void FixedUpdate()
    {
        if (currentState == Status.Move)
        {
            MoveState();
            MovementAnimating();
        }
        else
        {
            move_speedDesired = Vector3.zero;
        }
        MoveApply();

        switch (currentState)
        { 
            //Moving
            case Status.Move:
                MoveState();
                MoveApply();
                MovementAnimating();
                break;
            //Carrying
            case Status.Carry:
                break;
            //Destroy
            case Status.Destroy:
                DestroyAnimating();
                break;
            
        }
    }

    private void MovementInit()
    {
        currentState = Status.Move;
        animator.SetTrigger("Move");

        //Activate Right hand flame
        anim_flame_right.transform.Find("Flame").GetComponent<SkinnedMeshRenderer>().enabled = true;
    }
    private void MoveState()
    {
        //Decellerate
        if (input_movement.magnitude < move_accelMin)
        {
            move_speedDesired *= 0f;
            Debug.Log("NO INPUT constraint " + input_movement);
        }
        //Accellerate
        else
        {
            move_speedDesired = input_movement * move_maxSpeed;
            if (rb.velocity.magnitude > move_maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * move_maxSpeed;
                Debug.Log("MAX Velocity constraint");
            }
            else
            {
                Debug.Log("INPUT: " + input_movement + "/ DesiredSpeed: " + move_speedDesired);
            }
        }
    }
    private void MoveApply()
    {
        //Apply Speed
        move_speedDesired.y = rb.velocity.y;
        rb.velocity = Vector3.Lerp(rb.velocity, move_speedDesired, move_accel);
        //Set Heading
        if (input_movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(input_movement), move_rotateSpeed * Time.deltaTime);
        }
    }
    private void MovementAnimating()
    {
        //Running
        if (move_speedDesired.magnitude > moveAnim_running_speedThreshhold)
        {
            animator.SetInteger("walking", 2);
            MovementAnimatingFlame(2);
        }
        //Walking
        else if (move_speedDesired.magnitude > moveAnim_walking_speedThreshhold)
        {
            animator.SetInteger("walking", 1);
            MovementAnimatingFlame(1);
        }
        //Idle
        else
        {
            animator.SetInteger("walking", 0);
            MovementAnimatingFlame(0);
        }
    }
    private void MovementAnimatingFlame(int level)
    {
        anim_flame_main.GetComponent<Animator>().SetInteger("Level", level);
        anim_flame_right.GetComponent<Animator>().SetInteger("Level", level);
        anim_flame_left.GetComponent<Animator>().SetInteger("Level", level);
    }

    private void DestroyInit()
    {
        currentState = Status.Destroy;
        animator.SetTrigger("Destroy");
        destroy_shotCount = 1;

        //Deactivate Right hand flame
        anim_flame_right.transform.Find("Flame").GetComponent<SkinnedMeshRenderer>().enabled = false;
    }
    private void DestroyAdd()
    {
        destroy_shotCount++;
        if (destroy_shotCount > destroy_shotCountMax)
            destroy_shotCount = destroy_shotCountMax;
        Debug.Log("DestroyAdd " + destroy_shotCount);
    }
    private void DestroyAnimating()
    {
    }


    //Listeners
    private void DestroyAnimationEvent(int val)
    {
        if (val == 1)
        {
            destroy_shotCount--;
        }
        else
        {
            if (destroy_shotCount > 0)
            {
                animator.SetTrigger("Destroy");
            }
            else
            {
                MovementInit();
            }
        }
    }
    private void OnMove(InputValue val)
    {
        input_movement.Set(val.Get<Vector2>().x, 0f, val.Get<Vector2>().y);
    }
    private void OnDestroy()
    {
        if (currentState != Status.Destroy)
        {
            DestroyInit();
        }
        else
        {
            DestroyAdd();
        }
    }
}
