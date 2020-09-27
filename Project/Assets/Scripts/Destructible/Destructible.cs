using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public float maxHealth;
    public float health;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    public virtual bool TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Destructible Damaged - Health: " + health);
        if (health <= 0) 
        {
            Destruct();
            return true;
        }
        return false;
    }
    public virtual void Destruct()
    {
        Destroy(gameObject);
    }
}
