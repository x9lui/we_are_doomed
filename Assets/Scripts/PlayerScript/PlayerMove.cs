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
    public float jumpForce = 8f; // Jump force
    public float gravityMultiplier = 2.5f; // Gravity multiplier

    [Header("Weapon GameObjects")]
    private GameObject fistWeapon = null; // Reference to the Fist weapon GameObject
    private GameObject pistolWeapon = null; // Reference to the Pistol weapon GameObject
    private GameObject shotgunWeapon = null; // Reference to the Shotgun weapon GameObject
    private GameObject rifleWeapon = null; // Reference to the Rifle weapon GameObject
    private GameObject rocketLauncherWeapon = null; // Reference to the Rocket Launcher weapon GameObject
    private GameObject meleeWeapon = null; // Reference to the Melee weapon GameObject

    public Gun currentGun { get; private set; }

    void Start()
    {
        MyCC = GetComponent<CharacterController>();
        inventory = GetComponent<InventoryScript>();

        fistWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.Fist));
        pistolWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.Pistol));
        shotgunWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.Shotgun));
        rifleWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.Rifle));
        rocketLauncherWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.RocketLauncher));
        meleeWeapon = FindWeaponInHUD(inventory.GetWeapon(InventoryScript.WeaponType.Melee));

        UpdateWeaponVisibility();


    }

    private GameObject FindWeaponInHUD(string weaponName)
    {
        if (string.IsNullOrEmpty(weaponName)) return null;

        Transform hudTransform = GameObject.Find("HUD")?.transform;

        if (hudTransform != null)
        {
            Transform weaponTransform = hudTransform.Find(weaponName);
            if (weaponTransform != null)
            {
                return weaponTransform.gameObject;
            }
        }

        return null;
    }

    void Update()
    {
        GetInput();
        MovePlayer();
        HandleInventoryInput();

        if (currentGun != null)
        {
            if (currentGun.getCanAuto())
            {
                // Solo dispara si el botón está presionado
                if (Input.GetMouseButton(0))
                    currentGun.Fire();
                // Detener animación al soltar
                if (Input.GetMouseButtonUp(0))
                    currentGun.StopFiringAnim();
            }
            else
            {
                // Solo dispara una vez por click
                if (Input.GetMouseButtonDown(0))
                    currentGun.Fire();
                if (Input.GetMouseButtonUp(0))
                    currentGun.StopFiringAnim();
            }
        }
    }

    void GetInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            inputvector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            inputvector.Normalize();
            inputvector = transform.TransformDirection(inputvector);
            isWalking = true;

            if (currentGun != null)
            {
                currentGun.Walk();
            }
        }
        else
        {
            inputvector = Vector3.Lerp(inputvector, Vector3.zero, momentumDamping * Time.deltaTime);
            isWalking = false;

            // Llamar a la función Idle del arma actual
            if (currentGun != null)
            {
                currentGun.Idle();
            }
        }

        // --- SALTO ---
        if (MyCC.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            myGravity = jumpForce; // Aplica fuerza de salto
        }

        camAnim.SetBool("isWalking", isWalking);
        movementVector = (inputvector * speed) + (Vector3.up * myGravity);
    }

    void MovePlayer()
    {
        MyCC.Move(movementVector * Time.deltaTime);
        if (MyCC.isGrounded)
        {
            if (myGravity < 0)
                myGravity = -9.81f;
        }
        else
        {
            myGravity -= 9.81f * gravityMultiplier * Time.deltaTime;
        }
    }

    void HandleInventoryInput()
    {
        if (currentGun != null && currentGun.isFiring)
            return;

        InventoryScript.WeaponType newSlot = currentSlot;

        if (Input.GetKeyDown(KeyCode.Alpha1)) newSlot = InventoryScript.WeaponType.Fist;
        if (Input.GetKeyDown(KeyCode.Alpha2)) newSlot = InventoryScript.WeaponType.Pistol;
        if (Input.GetKeyDown(KeyCode.Alpha3)) newSlot = InventoryScript.WeaponType.Shotgun;
        if (Input.GetKeyDown(KeyCode.Alpha4)) newSlot = InventoryScript.WeaponType.Rifle;
        if (Input.GetKeyDown(KeyCode.Alpha5)) newSlot = InventoryScript.WeaponType.RocketLauncher;
        if (Input.GetKeyDown(KeyCode.Alpha6)) newSlot = InventoryScript.WeaponType.Melee;

        string weaponName = inventory.GetWeapon(newSlot);
        if (!string.IsNullOrEmpty(weaponName))
        {
            currentSlot = newSlot;
            UpdateWeaponVisibility();
        }
        else
        {
            Debug.LogWarning($"Slot {newSlot} is empty. Cannot switch to this slot.");
        }

        // Eliminar el arma del slot actual si se presiona la tecla Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 dropPos = transform.position + transform.forward * 3f;
            bool removed = inventory.RemoveWeapon(currentSlot, dropPos);
            if (removed)
            {
                currentSlot = InventoryScript.WeaponType.Fist;
                UpdateWeaponVisibility();
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
                        Debug.Log($"Current weapon: {gun.GetType().Name}, {currentSlot}");
                    }
                    weaponFound = true;
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        if (!weaponFound)
        {
            Debug.Log($"No weapon found for slot: {currentSlot}");
            currentGun = null;
        }
    }

    void Fire()
    {
        if (currentGun != null)
        {
            if (currentGun.getCanAuto())
            {
                if (Input.GetMouseButton(0))
                {
                    currentGun.Fire();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    currentGun.Fire();
                }
            }
        }
        else
        {
            Debug.LogWarning("No active weapon to fire!");
        }
    }
}
