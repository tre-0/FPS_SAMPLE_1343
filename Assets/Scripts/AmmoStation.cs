using UnityEngine;
using UnityEngine.Events;

public class AmmoPickup : MonoBehaviour
//Trigger pickup, and deletes itself after player presses "E" on object
{
    [SerializeField] int ammoAmount = 10;
    //Total bullets per pickup
    [SerializeField] GameObject promptUI;
    //optional UI prompt


    FPSController player;
    //Set when player enters trigger.
    //Cleared after pickup

    void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<FPSController>();
        if (player == null) return;

        if(player != null)
        {
            Debug.Log("add to interact");
            player.AddToInteract(GiveAmmo);
            //Binds GiveAmmo to FPSController's E key (OnInteract.Invoke)
        }

    }
    //Requires CharacterController player and trigger collider on this object,
    //This basically triggers the ammo pickup mechanic if a character with the FPSController is present


    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<FPSController>() == null) return;
        promptUI.SetActive(false);
        Unsubscribe();
    }
    //prompt and remove interact listener so E does nothing now

    void GiveAmmo()
    {
        Debug.Log("refill");
        if (player == null) return;
        player.IncreaseAmmo(ammoAmount);
        Unsubscribe();
        Destroy(gameObject);
    }
    //Gives ammo after pressing E, unsubscribes, and destroys pickup after pressing E

    void Unsubscribe()
    {
        // cleanup
        player.RemoveInteract();
        player = null;
    }
    //Rmeoves all OnInteract listeners.

    void OnDestroy()
    {
        Unsubscribe();
    }
    //Destorys object entirely
}