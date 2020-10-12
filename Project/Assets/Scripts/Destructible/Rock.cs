using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Ressource
{
    public GameObject stonePrefab;

    public override void Destruct()
    {
        Debug.Log("Stone Destructed");
        base.Destruct();
        GameObject.Instantiate(stonePrefab, transform.position, transform.rotation);
    }
}
