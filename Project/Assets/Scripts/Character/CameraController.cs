using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 relativePosition;
    // Start is called before the first frame update
    void Awake()
    {
        relativePosition = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = player.transform.position + relativePosition;
    }
}
