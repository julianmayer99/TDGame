using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterReferences : MonoBehaviour
{
    // This Script is used to store References for other Scripts to acces
    public Material mt_headNeutral;
    public Material mt_headHappy;
    public Material mt_headFocus;

    public GameObject prefab_projectile;


    public GameObject anim_flame_main;
    public GameObject anim_flame_left;
    public GameObject anim_flame_right;

    public ParticleSystem anim_partSystemShot;
    public ParticleSystem anim_partSystemCarry;
}
