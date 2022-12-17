using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class GunData : ScriptableObject
{
    #region Variables
    [Header("weaponCharacteristics")]
    public string gunName;
    public int maxAmmoCount;
    public int actualAmmo;
    public float reloadTime;
    public float damage;
    public float fireRate;
    public float range;
    public Vector2 recoil;

    public Vector3 offsset;

    public bool automatic;
    public bool sniper;
    #endregion
}
