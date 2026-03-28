using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class FPSController : MonoBehaviour
//First-person movement
//Must sit on the same gameObject as the CharacterController.
{
    CharacterController controller;
    //Movement uses characterController.Move instead of Rigidbody
    [SerializeField] GameObject cam;
    //Child that recieves up and down look
    [SerializeField] Gun initialGun;
    //Gun is assigned in inspector so that there is a refernece for the script to use

    [SerializeField] float movementSpeed = 2.0f;
    [SerializeField] float lookSensitivityX = 1.0f;
    [SerializeField] float lookSensitivityY = 1.0f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpForce = 10;
    //All of the serialized fields to change player movement

    Vector3 velocity;
    //Velocity is for hazard knockback, however it is unused for now
    bool grounded;
    float xRotation;
    //Handles movement of character and mouse movement
    Gun currentGun = null;
    //Current gun that recieves ammo and fires. Is set up in void Start() to make the current gun into the initial gun
    //Meaning the current gun can now be the gun from the inspector.

    [SerializeField] UnityEvent OnInteract;
    //For interacting with objects.
    [SerializeField] public UnityEvent OnDamaged;
    //Used for "damaging" the player. Activates when a player collides with any parent with the hazard script (child).

    public GameObject Cam { get { return cam; } }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
       
        if (controller == null)
        {
            enabled = false;
            return;
        }
        if (initialGun == null)
        {
            return;
        }

        currentGun = initialGun;

    }
    //locks cursor, caches characterController, and connects the current gun with the inspector gun (initialGun).

    public void AddToInteract(UnityAction interact)
    {
        OnInteract.AddListener(interact);
    }


    public void RemoveInteract()
    {
        OnInteract.RemoveAllListeners();
    }
    //Lets world objects register/Unregister when the player interacts with an object.

    void Update()
    {
        if (controller == null) return;
        Movement();
        Look();
        FireGun();

        if (Input.GetKeyDown(KeyCode.E))
            OnInteract.Invoke();

        Vector3 noVelocity = new Vector3(0, velocity.y, 0);
        velocity = Vector3.Lerp(velocity, noVelocity, 5 * Time.deltaTime);
        //Knockback on X and Z, but is unused as of now.
    }
    //Controls movement, looking, shooting, and interacting with an object by pressing E
    //Also has horizontal knockback delay, but that is unused as of now.
    //Its in update because every frame is checking for this.

    void Movement()
    {
        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0) velocity.y = -0.5f; 

        Vector2 movement = GetPlayerMovementVector();
        Vector3 move = transform.right * movement.x + transform.forward * movement.y;
        controller.Move(move * movementSpeed * (GetSprint() ? 2 : 1) * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && grounded)
            velocity.y += Mathf.Sqrt(jumpForce * -1 * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    //Main movement of the capsule. Is in void Update() so every frame runs this command


    void Look()
    {
        Vector2 looking = GetPlayerLook();
        float lookX = looking.x * lookSensitivityX * Time.deltaTime;
        float lookY = looking.y * lookSensitivityY * Time.deltaTime;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * lookX);
    }
    //Lets the player look around with the camera.

    void FireGun()
    {
        if (GetPressFire()) currentGun.AttemptFire();
    }
    //Uses Fire1 to whatever gun is currently equiped.

    public void IncreaseAmmo(int amount)
    {
        currentGun.AddAmmo(amount);
    }
    //Called by AmmoPickup after going to an ammo station and pressing E

    bool GetPressFire() 
    { 
        return Input.GetButtonDown("Fire1"); 
    }
    //Input for firing the gun
    Vector2 GetPlayerMovementVector() 
    { 
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); 
    }
    //Getting horizontal and vertical movement.
    Vector2 GetPlayerLook() 
    { 
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); 
    }
    //Getting Mousemovement for the camera
    bool GetSprint() 
    { 
        return Input.GetButton("Sprint"); 
    }
    //Sprint button when holding down shift


    private void OnControllerColliderHit(ControllerColliderHit hit)
    //Is called when the player hits a solid object with the damager script
    {   
        Debug.Log("Controller hit: " + hit.collider.name);
        var damager = hit.gameObject.GetComponentInParent<Damager>();
        //!!!!! MOST IMPORTANT PART OF THE SCRIPT !!!!!
        //This was the key on activating the hud change. GetComponentInParent is good for checking whole
        //Prefabs and other objects to see if they are carrying a child script named "Damager".
      
        if (damager)
        {
            var collisionPoint = hit.collider.ClosestPoint(transform.position);
            var knockbackAngle = (transform.position - collisionPoint).normalized;
            velocity = (20 * knockbackAngle);
        
            OnDamaged.Invoke();
        }
        //If damager, then push player away on contact, and invoke "OnDamaged.Invoke"
        //OnDamaged.Invoke is an event, and requires actionListeners to check if this is event is being invoked.
    }
    

   
}