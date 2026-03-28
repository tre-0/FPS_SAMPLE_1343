using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CameraShake : MonoBehaviour
//Jitter camera localPosition, put on camera to activate
{
    [SerializeField] float shakeDuration = 0.1f;
    [SerializeField] float shakeMagnitude = 0.05f;
    //How long and strong the offset lasts each call

    public void TriggerShake()
    {   
        Debug.Log("TriggerShake called");
        StartCoroutine(Shake());
        //Snap back to original locla position when finished
        //So drift doesn't occur
        
    }
    //Call from gun.OnFired in Inspector

    IEnumerator Shake()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
    //Small random XY shake in local space
}
