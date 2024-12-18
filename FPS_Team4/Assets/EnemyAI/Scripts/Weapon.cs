using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    EMP,
    Damage
}

[CreateAssetMenu]

public class Weapon : ScriptableObject
{ 
    public WeaponType type;
    public GameObject model;
    public int shootDamage;
    public int shootDist;
    public int shootRate;
}
