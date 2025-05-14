using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 20f; // Speed of the player movement
    private CharacterController MyCC; // Reference to the CharacterController component
    private Vector3 inputvector; // Direction of movement
    private Vector3 movementVector; // Movement vector
    private float myGravity = -9.81f; // Gravity value
    public Animator camAnim; // Reference to the Animator component
    private bool isWalking; // Flag to check if the player is walking
    public float momentumDamping = 5f; // Damping value for momentum
    private InventoryScript inventory; // Reference to the InventoryScript
    private InventoryScript.WeaponType currentSlot = InventoryScript.WeaponType.Fist; // Current selected slot

    [Header("Weapon GameObjects")]
    private GameObject fistWeapon = null; // Reference to the Fist weapon GameObject
    private GameObject pistolWeapon = null; // Reference to the Pistol weapon GameObject
    private GameObject shotgunWeapon = null; // Reference to the Shotgun weapon GameObject
    private GameObject rifleWeapon = null; // Reference to the Rifle weapon GameObject
    private GameObject sniperWeapon = null; // Reference to the Sniper weapon GameObject
    private GameObject rocketLauncherWeapon = null; // Reference to the Rocket Launcher weapon GameObject
    private GameObject meleeWeapon = null; // Reference to the Melee weapon GameObject

    public Gun currentGun { get; private set; }

    void Start()
    {
        MyCC = GetComponent<CharacterController>(); // Get the CharacterController component attached to the player
        inventory = GetComponent<InventoryScript>(); // Get the InventoryScript component attached to the player

        // Dynamically find weapon GameObjects based on inventory
        fistWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.Fist));
        pistolWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.Pistol));
        shotgunWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.Shotgun));
        rifleWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.Rifle));
        sniperWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.Sniper));
        rocketLauncherWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.RocketLauncher));
        meleeWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.Melee));

        UpdateWeaponVisibility(); // Ensure the correct weapon is active at the start


    }

    private GameObject FindWeaponInHUD(string weaponName)
    {
        // Return null if the weapon name is null or empty
        if (string.IsNullOrEmpty(weaponName)) return null;

        // Search for the weapon GameObject in the HUD
        Transform hudTransform = GameObject.Find("HUD")?.transform;

        if (hudTransform != null)
        {
            Transform weaponTransform = hudTransform.Find(weaponName);
            if (weaponTransform != null)
            {
                return weaponTransform.gameObject;
            }
        }

        // Return null if the weapon is not found
        return null;
    }

    void Update()
    {
        GetInput(); // Call the method to get player input
        MovePlayer(); // Call the method to move the player 
        HandleInventoryInput(); // Handle inventory input

        // Detectar si el jugador presiona el botón de disparo
        if (Input.GetButtonDown("Fire1"))
        {
            Fire(); // Llamar a la función Fire
        }
    }

    void GetInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            inputvector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); // Get input from the player
            inputvector.Normalize(); // Normalize the input vector to ensure consistent movement speed
            inputvector = transform.TransformDirection(inputvector); // Transform the input vector to world space
            isWalking = true; // Set the walking flag to true

            // Llamar a la función Walk del arma actual
            if (currentGun != null)
            {
                currentGun.Walk();
            }
        }
        else
        {
            inputvector = Vector3.Lerp(inputvector, Vector3.zero, momentumDamping * Time.deltaTime); // Smoothly reduce the input vector to zero
            isWalking = false; // Set the walking flag to false

            // Llamar a la función Idle del arma actual
            if (currentGun != null)
            {
                currentGun.Idle();
            }
        }

        camAnim.SetBool("isWalking", isWalking); // Set the walking animation parameter in the Animator
        movementVector = (inputvector * speed) + (Vector3.up * myGravity); // Calculate the movement vector based on input and speed
    }

    void MovePlayer()
    {
        MyCC.Move(movementVector * Time.deltaTime); // Move the player using the CharacterController component
        if (MyCC.isGrounded) // Check if the player is grounded
        {
            myGravity = -9.81f; // Reset gravity when grounded
        }
        else
        {
            myGravity -= 9.81f * Time.deltaTime; // Apply gravity when not grounded
        }
    }



    void HandleInventoryInput()
    {
        // Check for number keys to switch slots
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentSlot = InventoryScript.WeaponType.Fist;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentSlot = InventoryScript.WeaponType.Pistol;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentSlot = InventoryScript.WeaponType.Shotgun;
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentSlot = InventoryScript.WeaponType.Rifle;
        if (Input.GetKeyDown(KeyCode.Alpha5)) currentSlot = InventoryScript.WeaponType.Sniper;
        if (Input.GetKeyDown(KeyCode.Alpha6)) currentSlot = InventoryScript.WeaponType.RocketLauncher;
        if (Input.GetKeyDown(KeyCode.Alpha7)) currentSlot = InventoryScript.WeaponType.Melee;

        // Update weapon visibility when switching slots
        UpdateWeaponVisibility();

        // Remove weapon from the current slot when pressing Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool removed = inventory.RemoveWeapon(currentSlot);
            if (removed)
            {
                Debug.Log($"Weapon removed from slot: {currentSlot}");
                UpdateWeaponVisibility(); // Update visibility after removing a weapon
            }
        }
    }

    void UpdateWeaponVisibility()
    {
        // Obtener todos los hijos del HUD que tengan un componente que herede de Gun
        Transform hudTransform = GameObject.Find("HUD")?.transform;
        if (hudTransform == null)
        {
            Debug.LogWarning("HUD not found! UPDATE WEAPON VISIBILITY FAILED");
            return;
        }

        bool weaponFound = false; // Bandera para verificar si se encontró el arma actual

        foreach (Transform child in hudTransform)
        {
            Gun gun = child.GetComponent<Gun>();
            if (gun != null)
            {
                // Mostrar solo el arma correspondiente al slot actual
                if (gun.name == inventory.GetWeapon(currentSlot))
                {
                    child.gameObject.SetActive(true);
                    currentGun = gun; // Actualizar la referencia al arma activa
                    if (currentGun != gun)
                    {
                        Debug.Log($"Current weapon: {gun.GetType().Name}, {currentSlot}"); // Escribir el nombre del arma actual en la consola
                    }
                    weaponFound = true;
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        // Si no se encontró el arma actual, escribir un mensaje en la consola
        if (!weaponFound)
        {
            Debug.Log($"No weapon found for slot: {currentSlot}");
            currentGun = null; // No hay arma activa
        }
    }

    void Fire()
    {
        if (currentGun != null)
        {
            currentGun.Fire(); // Llamar al método Fire del arma activa
        }
        else
        {
            Debug.LogWarning("No active weapon to fire!");
        }
    }
}
