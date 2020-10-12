using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ressource : Destructible
{
    // Update is called once per frame
    public override void Destruct()
    {
        Debug.Log("Ressource Destructed");
        base.Destruct();
    }
}
