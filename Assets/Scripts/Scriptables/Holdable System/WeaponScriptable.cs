using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum damageType
{
    physical,
    holy,
    arcane,
    death
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "New Item Scriptable/New Weapon", order = 3)]
public class WeaponScriptable : HoldableScriptable
{

    [Header("Weapon Settings")]
    [SerializeField]
    damageType damageType;
    [SerializeField]
    int damage;

    public int Damage { get => damage; }
    internal damageType DamageType { get => damageType; }
}
