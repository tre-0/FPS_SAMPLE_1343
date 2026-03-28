using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PlayerHUD : MonoBehaviour
//Main canvas HUD, controlling UI elements
{
    [SerializeField] Image healthBar;
    //Image for health and UI
    [SerializeField] TMP_Text currentAmmoText;
    //Current ammo in the clip
    [SerializeField] TMP_Text maxAmmoText;
    //Maximum ammo text for current gun
    [SerializeField] Image damageFlash;
    //Damager flash (fullscreen image) when the player collides with an object with the damager script.

    [SerializeField] FPSController player;
    //Needs to be referenced to access parts of FPSController as player.
    void OnEnable()
    {

        if (player == null) return;
        player.OnDamaged.AddListener(TriggerDamageFlash);
    }

    void OnDisable()
    {
        if (player == null) return;
        player.OnDamaged.RemoveListener(TriggerDamageFlash);
    }
    // Enables and disables so we don't call 
    // TriggerDamageFlash after this HUD is disabled.
    
    public void UpdateAmmoDisplay(int newAmmo)
    {
        currentAmmoText.text = newAmmo.ToString();
    }
    //Updates the current ammo text into a string, showing the current ammo the gun has left

    public void TriggerDamageFlash()
    {
        if (damageFlash == null) return;
        StopAllCoroutines();
        //Restarts flash if damaged again mid-fade
        StartCoroutine(FlashRed());
    }
    // Called by FPSController.OnDamaged in the Inspector
    //Triggers the red flash after touching an object

    IEnumerator FlashRed()
    {
        if (damageFlash == null) yield break;
  
        damageFlash.color = new Color(1, 0, 0, 0.4f);
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.4f, 0f, elapsed / duration);
            damageFlash.color = new Color(1, 0, 0, alpha);
            yield return null;
        }
        //Fade alpha from 0.4 to 0 over a duration of seconds.
        //In the inspector, the alpha is 0, so the HUD stays invisible until this occurs.

        damageFlash.color = new Color(1, 0, 0, 0);
    }
    //brief overlay change to a transparent red for a brief moment.

}