using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControler : MonoBehaviour
{
    private Rigidbody rb;
    private CharacterReferences rf;
    private Animator animator;

    //States
    enum Status
    {
        Move,
        Carry,
        Destroy
    }
    private Status currentState = Status.Move;

    //Movement
    private float movementInput_accelMin = 0.1f;
    private float movement_rotateSpeed = 8f;
    private float movement_accel = 40f;
    private Vector3 movement_speedDesired = new Vector3();


    //Move
    private Vector3 input_movement = new Vector3();
    private float move_maxSpeed = 22f;
        //Animating
    private float moveAnim_walking_speedThreshhold = 0.2f;
    private float moveAnim_running_speedThreshhold = 18f;

    //Carrying
    private float carry_maxSpeed = 14f;
    private GameObject carry_object;

    private float carry_force = 20f;

    //Destroy
    private int destroy_shotCount;
    private int destroy_shotCountMax = 2;
    private void Awake()
    {
        rf = GetComponent<CharacterReferences>();
        rb = GetComponent<Rigidbody>();
        animator = transform.GetChild(0).GetComponent<Animator>();

        //Deactivate ParticleSystems
        rf.anim_partSystemShot.Stop();
        rf.anim_partSystemCarry.Stop();
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
        switch (currentState)
        { 
            //Moving
            case Status.Move:
                MoveState();
                break;
            //Carrying
            case Status.Carry:
                CarryState();
                break;
            //Destroy
            case Status.Destroy:
                break;
            
        }
        MovementApply();
    }

    //Movement
    private void MoveInit()
    {
        currentState = Status.Move;
        animator.SetTrigger("Move");

        //Activate Right hand flame
        rf.anim_flame_right.transform.Find("Flame").GetComponent<SkinnedMeshRenderer>().enabled = true;
        rf.anim_flame_right.transform.Find("Spot Light").GetComponent<Light>().enabled = true;


        //Switch HeadMaterial
        rf.head.GetComponent<SkinnedMeshRenderer>().material = rf.mt_headNeutral;
    }
    private void MoveState()
    {
        Movement(move_maxSpeed);
        MovementAnimating();
    }
    private void Movement(float maxSpeed)
    {
        //Set DesiredSpeed
        movement_speedDesired = input_movement * maxSpeed;
    }
    private void MovementApply()
    {
        //Apply Speed
        Vector3 currentSpeed = new Vector3(rb.velocity.x,0,rb.velocity.z);
        currentSpeed = Vector3.Lerp(currentSpeed, movement_speedDesired, movement_accel);
        currentSpeed.y = rb.velocity.y;
        rb.velocity = currentSpeed;
        //Set Heading
        if (input_movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(input_movement), movement_rotateSpeed * Time.deltaTime);
        }
    }
    private void MovementAnimating()
    {
        //Running
        if (movement_speedDesired.magnitude > moveAnim_running_speedThreshhold)
        {
            animator.SetInteger("walking", 2);
            MovementAnimatingFlame(2);
        }
        //Walking
        else if (movement_speedDesired.magnitude > moveAnim_walking_speedThreshhold)
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
        rf.anim_flame_main.GetComponent<Animator>().SetInteger("Level", level);
        rf.anim_flame_right.GetComponent<Animator>().SetInteger("Level", level);
        rf.anim_flame_left.GetComponent<Animator>().SetInteger("Level", level);
    }

    //Carry
    private void CarryInit()
    {
        //Check if carry is possible
        carry_object = rf.col_carryRange.GetComponent<CarryRangeControler>().selected;
        if (carry_object != null) 
        { 
            currentState = Status.Carry;
            animator.SetTrigger("Carry");

            carry_object.GetComponent<RessourceItemControler>().PickUp();

            //Deactivate Right hand flame
            rf.anim_flame_right.transform.Find("Flame").GetComponent<SkinnedMeshRenderer>().enabled = false;
            rf.anim_flame_right.transform.Find("Spot Light").GetComponent<Light>().enabled = false;

            //Activate Particle Systems
            rf.anim_partSystemCarry.Play();
        }
    }
    private void CarryState()
    {
        Movement(carry_maxSpeed);
        MovementAnimating();
        if (carry_object != null)
        {
            carry_object.GetComponent<Rigidbody>().velocity = carry_force * (rf.carry.transform.position - carry_object.GetComponent<Transform>().position);
        }
    }
    private void CarryRelease()
    {
        //Release
        carry_object.GetComponent<RessourceItemControler>().Release();
        //Stop Particle Systems
        rf.anim_partSystemCarry.Stop();
        rf.anim_partSystemCarry.Clear();
        MoveInit();
    }

    //Destroy
    private void DestroyInit()
    {
        currentState = Status.Destroy;
        animator.SetTrigger("Destroy"); 
        
        movement_speedDesired = Vector3.zero;
        destroy_shotCount = 1;

        //Deactivate Right hand flame
        rf.anim_flame_right.transform.Find("Flame").GetComponent<SkinnedMeshRenderer>().enabled = false;
        rf.anim_flame_right.transform.Find("Spot Light").GetComponent<Light>().enabled = false;

        //Switch HeadMaterial
        rf.head.GetComponent<SkinnedMeshRenderer>().material = rf.mt_headFocus;

    }
    private void DestroyAdd()
    {
        destroy_shotCount++;
        if (destroy_shotCount > destroy_shotCountMax)
            destroy_shotCount = destroy_shotCountMax;
    }
    private void DestroyEmitShot()
    {
        destroy_shotCount--;
        GameObject p = Instantiate(rf.prefab_projectile) as GameObject;
        p.transform.position = rf.anim_flame_right.transform.position;
        p.transform.rotation = transform.rotation;

        //Particles
        rf.anim_partSystemShot.Play();
    }


    //Listeners
    public void DestroyAnimationEvent(int val)
    {
        if (val == 1)
        {
            DestroyEmitShot();
        }
        else
        {
            if (destroy_shotCount > 0)
            {
                animator.SetTrigger("Destroy");
            }
            else
            {
                MoveInit();
            }
        }
    }
    private void OnMove(InputValue val)
    {
        input_movement.Set(val.Get<Vector2>().x, 0f, val.Get<Vector2>().y);//Decellerate
        if (input_movement.magnitude < movementInput_accelMin)
        {
            input_movement = Vector3.zero;
        }
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
    private void OnCarry()
    {
        if (currentState == Status.Move)
        {
            CarryInit();
        }
        else if (currentState == Status.Carry)
        {
            CarryRelease();
        }
    }
}
