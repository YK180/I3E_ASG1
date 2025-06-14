    using UnityEngine;
    using TMPro;
    using System.Collections;
using System;
public class PlayerBehaviour : MonoBehaviour
    {
        public int coinCount = 0;
        public TextMeshProUGUI CoinText;

        // Total number of coins required to win
        public int totalCollectibles = 5;

        // UI element to display win message
        public TextMeshProUGUI winMessageText;

        public float interactDistance = 3f;      // How close the player needs to be
        public KeyCode interactKey = KeyCode.E;  // Interaction key

        
        public bool hasKeycard = false;

        public int maxHealth = 100;
        private int currentHealth;

        private Vector3 startPosition;
        private Vector3 initialCameraLocalPos;
        private Quaternion initialCameraLocalRot;
        private Transform mainCameraTransform;
        private MouseLook mouseLookScript;


        public TextMeshProUGUI interactionMessageText;
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI deathMessageText;

       
    void Start()
    {
        UpdateCoinUI();

        // Hide the win message at the start
        if (winMessageText != null)
            winMessageText.gameObject.SetActive(false);

        currentHealth = maxHealth;
        UpdateHealthUI();

        if (deathMessageText != null)
        {
            deathMessageText.text = "";
            deathMessageText.gameObject.SetActive(false);
        }


        startPosition = transform.position;
            
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
            initialCameraLocalPos = mainCameraTransform.localPosition;
            initialCameraLocalRot = mainCameraTransform.localRotation;

            mouseLookScript = mainCameraTransform.GetComponent<MouseLook>();
        }
            
        }


        void Update()
        {
            if (Camera.main == null) return;
            if (Input.GetKeyDown(interactKey))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance))
            {
                Door door = hit.collider.GetComponent<Door>();
                if (door != null)
                {
                    if (door.requiresKeycard && !hasKeycard)
                    {
                        // Show UI message
                        StartCoroutine(ShowMessage("Door is locked. Requires keycard.", 2f));
                    }
                    else
                    {
                        door.ToggleDoor();
                    }
                    return;
                }

                // Check for Keycard interaction
                Keycard keycard = hit.collider.GetComponent<Keycard>();
                if (keycard != null)
                {
                    hasKeycard = true;
                    Destroy(keycard.gameObject);
                    StartCoroutine(ShowMessage("Keycard collected!", 2f));
                    return;
                }


            }
        }

        }

        public void TakeDamage(float amount)
        {
            currentHealth -= Mathf.RoundToInt(amount);
            UpdateHealthUI();

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            Debug.Log("Player has died");

            if (deathMessageText != null)
            {
                deathMessageText.gameObject.SetActive(true);
                deathMessageText.text = "You died!";
            }
            
            StartCoroutine(RespawnPlayer());
        }

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(2f);

        // Reset position
        transform.position = startPosition;

        // Reset velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Reset health
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Reset player scale and jump
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.ResetJumpState();

        // Wait one frame to ensure visuals and movement are settled
        yield return new WaitForEndOfFrame();

        // Hide death message
        if (deathMessageText != null)
        {
            deathMessageText.text = "";
            deathMessageText.gameObject.SetActive(false);
        }

        // Reset camera position
        Transform cam = Camera.main.transform;
        cam.localPosition = initialCameraLocalPos;
        cam.localRotation = initialCameraLocalRot;

        MouseLook look = cam.GetComponent<MouseLook>();
        if (look != null)
            look.ResetLook();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
            
            if (mainCameraTransform != null)
            {
                mainCameraTransform.localPosition = initialCameraLocalPos;
                mainCameraTransform.localRotation = initialCameraLocalRot;

            if (mouseLookScript != null)
                mouseLookScript.ResetLook();
            }
        

        }

        void UpdateHealthUI()
        {
            if (healthText != null)
            {
                healthText.text = "Health: " + currentHealth;
            }
        }

        IEnumerator ShowMessage(string message, float duration)
        {
            interactionMessageText.text = message;
            interactionMessageText.gameObject.SetActive(true);

            yield return new WaitForSeconds(duration);

            interactionMessageText.gameObject.SetActive(false);
        }

        public void CollectCoin(int amount)
        {
            coinCount += amount;
            UpdateCoinUI();

            // Check if player won
            if (coinCount >= totalCollectibles)
            {
                WinGame();
            }
        }

        void WinGame()
        {
            if (winMessageText != null)
            {
                winMessageText.gameObject.SetActive(true);
                winMessageText.text = "Congratulations! You collected all coins!";
            }

            // Optional: Freeze player movement here or do other win actions
            Debug.Log("Player has won the game!");
        }

        void UpdateCoinUI()
        {
            if (CoinText != null)
            {
                CoinText.text = "Coins: " + coinCount + " / " + totalCollectibles;
            }
        }

        
    }
