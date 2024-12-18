using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// interface for damageable objects
public interface iDamage
{
    // method to apply damage
    void takeDamage(int amount, Weapon weapon);
}