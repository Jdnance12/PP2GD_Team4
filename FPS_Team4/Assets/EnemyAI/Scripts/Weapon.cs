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
    public float shootRate;


    public void ApplyDamage(GameObject target)
    {
        var damageable = target.GetComponent<iDamage>();

        if(damageable != null )
        {
            if(type == WeaponType.Damage)
            {
                damageable.takeDamage(shootDamage);
            }
            else if(type == WeaponType.EMP)
            {
                damageable.takeEMP(shootDamage);
            }
        }
    }
}
