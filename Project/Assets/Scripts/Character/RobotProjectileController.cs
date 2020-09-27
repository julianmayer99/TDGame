using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotProjectileController : MonoBehaviour
{
    private enum Status
    {
        Armed,
        Impact,
        RunOut
    }
    private Status currentState = Status.Armed;

    private Rigidbody rb;
    private Vector3 pos;
    private MeshRenderer mesh;

    public ParticleSystem particles;
    public ParticleSystem particlesBurst;


    [SerializeField]
    private Material mat;
    [SerializeField]
    private Material mat_active;

    private float speed = 30f;
    private float damage = 34f;


    private float armedLifetime = 3f;

    private float armed_targetingDistance = 300f;
    private Transform armed_target = null;
    private float armed_targetDistance = Mathf.Infinity;
    private float armed_lerpFactor = 0f;
    private float armed_lerpFactorBuildUp = 0.5f;


    private float impactLifetime = 1f;

    private float runOutLifetime = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        particlesBurst.Stop();
    }

    private void Start()
    {        
        rb = GetComponent<Rigidbody>();
        rb.velocity = (transform.rotation * Vector3.forward).normalized * speed;
        mesh = GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pos = transform.position;
        switch (currentState)
        {
            case Status.Armed:
                ArmedState();
                break;
            case Status.Impact:
                ImpactState();
                break;
            case Status.RunOut:
                RunOutState();
                break;
        }
    }

    private void ArmedState()
    {
        //LifeTime
        armedLifetime -= Time.deltaTime;
        if (armedLifetime <= 0) currentState = Status.RunOut;


        //LerpFactor
        armed_lerpFactor += Time.deltaTime / armed_lerpFactorBuildUp;

        //Targeting
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("RobotTarget");
        armed_targetDistance = Mathf.Infinity;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - pos;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < armed_targetDistance)
            {
                armed_target = go.transform;
                armed_targetDistance = curDistance;
            }
        }

        if (armed_targetDistance <= armed_targetingDistance)
        {
            mesh.material = mat_active;
            //Steer
            rb.velocity = Vector3.Lerp(rb.velocity.normalized, (armed_target.position - pos).normalized, Mathf.Min(armed_lerpFactor * armed_targetingDistance / armed_targetDistance, 1f * Time.deltaTime)).normalized * speed;
        }
        else
        {
            mesh.material = mat;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }

        transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
    }

    private void ImpactInit()
    {
        currentState = Status.Impact;
        particles.Stop();
        particlesBurst.Play();
        GetComponentInChildren<Light>().enabled = false;
        rb.velocity = Vector3.zero;
        mesh.enabled = false;
    }
    private void ImpactState()
    {
        //Lifetime
        impactLifetime -= Time.deltaTime;
        if (impactLifetime <= 0) Destroy(gameObject);
    }

    private void RunOutState()
    {
        //Lifetime
        runOutLifetime -= Time.deltaTime;
        if (runOutLifetime <= 0) Destroy(gameObject);

    }
    public float DamageImpact()
    {
        ImpactInit();
        return damage;
    }


}
