using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Gun : MonoBehaviour
{
    #region Variables
    public GunData data;
    public VisualEffect bulletEfect;

    [Header("sounds")]
    public AudioSource audioSource;
    public AudioClip shoot;
    public AudioClip reload;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        data.actualAmmo = data.maxAmmoCount;
    }

    #endregion
}
