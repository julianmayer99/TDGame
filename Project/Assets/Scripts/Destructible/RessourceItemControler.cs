using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceItemControler : MonoBehaviour
{
    private Rigidbody rb;
    private MeshRenderer mr;

    private bool carried = false;
    private bool selected = false;

    private Material mt_default;
    [SerializeField] private Material mt_selected;

    private float lerpFactor = 0.1f;
    private float releaseFactor = 0.4f;
    private float gravity = 30f;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        mt_default = mr.material;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!carried)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, lerpFactor * Time.deltaTime);
            rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }
    }

    public void PickUp()
    {
        carried = true;
    }
    public void Release()
    {
        rb.velocity *= releaseFactor;
        carried = false;
    }

    public void Select()
    {
        selected = true;
        mr.material = mt_selected;
    }
    public void Deselect()
    {
        selected = false;
        mr.material = mt_default;
    }
}
