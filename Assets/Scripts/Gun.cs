using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
//Spawns bullets, plays shooting animation, and notifies listeners on fire.
{
    [SerializeField] Transform gunBarrelEnd;
    //World position and rotation where bullet prefab is instantiated
    [SerializeField] GameObject bulletPrefab;
    //Must include rigidbody if bullet script sets velocity in start
    [SerializeField] Animator anim;
    //Animation trigger

    [SerializeField] int maxAmmo;
    [SerializeField] float timeBetweenShots = 0.1f;
    //clip size and minum seconds between shots



    int ammo;
    float elapsed = 0;
    //time since last successful shot

    [SerializeField] public UnityEvent OnFired;
    [SerializeField] public UnityEvent<int> OnAmmoChanged;
    //Inspector hooks:
    //Camera shake, muzzle VFX, and HUD ammo text (OnAmmoChanged recieves remaining ammo)

    void Start()
    {
        ammo = maxAmmo;
    }
    //start with full clip

    void Update()
    {
        elapsed += Time.deltaTime;
    }
    //Accumulate fire-rate timer for AttemptFire


    public bool AttemptFire()
    {
        if (ammo <= 0) return false;
        if (elapsed < timeBetweenShots) return false;
        

        Debug.Log("Bang");
        Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        anim.SetTrigger("shoot");
        timeBetweenShots = 0;
        ammo -= 1;
        Debug.Log("OnFired invoked");
        OnFired.Invoke();
        OnAmmoChanged.Invoke(ammo);

        return true;
    }
    //Returns false if empty or still cooling down.
    //Spawns bullets and raise events

    public void AddAmmo(int amount)
    {
        ammo += amount;
        OnAmmoChanged.Invoke(ammo);
    }
    //Refill clip, and tells listener the new ammo count


}