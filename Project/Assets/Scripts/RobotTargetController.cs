using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTargetController : MonoBehaviour
{
    private void Destructed()
    {
        Destroy(transform.parent.gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        //Check to see if the tag on the collider is equal to RobotProjectile
        if (other.tag == "RobotProjectile")
        {
            float damage = other.GetComponent<RobotProjectileController>().DamageImpact();
            transform.parent.GetComponent<Destructible>().TakeDamage(damage);
        }
    }
}
