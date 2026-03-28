using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Damager hit something: " + collision.gameObject.name);

    }

}
//Basically, logs a message to reassure that Damager hits the player, showing that the script works properly.
