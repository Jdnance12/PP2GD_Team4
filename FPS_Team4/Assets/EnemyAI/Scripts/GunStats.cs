using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    public GameObject model;
    enum damageType { damager, emp};
    [SerializeField] damageType type;
    public int shootDamage;
    public int shootDist;
    public float shootRate;

    public ParticleSystem hitEffect;
    public AudioClip[] audShootSound;
    public float shootSound;
}
