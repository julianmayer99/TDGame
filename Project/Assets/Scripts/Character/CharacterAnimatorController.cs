﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void DestroyAnimationEvent(int i)
    {
        transform.parent.GetComponent<CharacterControler>().DestroyAnimationEvent(i);
    }
}
