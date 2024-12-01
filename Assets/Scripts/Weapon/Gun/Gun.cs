using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Gun : Weapon, IGun 
{
    public float reloadTime;
    public float fireSpeed;
    public int ammoPerMag;

    public enum GunType
    {
        Pistol,
        Rifle,
        Shotgun,
        SGM,
        MG,
        Sniper,
        Special
    }

    public enum GunName
    {
        Barreta,
        Deagle,
        M4A1,
        Ak47
    }

    public enum FireMod
    {
        Auto,
        Single
    }

    public virtual void Shoot() { }

    public virtual void Reload() { }
}
