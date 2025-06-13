using UnityEngine;
using TMPro;

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

    void Start()
    {
        UpdateCoinUI();

        // Hide the win message at the start
        if (winMessageText != null)
            winMessageText.gameObject.SetActive(false);
        
        
    }


    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance))
            {
                Door door = hit.collider.GetComponentInParent<Door>();
                if (door != null)
                {
                    if (hasKeycard)
                    {
                        door.ToggleDoor();
                    }
                    else
                    {
                        Debug.Log("You need a keycard to open this door.");
                    }
                    return;
                }

                Keycard keycard = hit.collider.GetComponentInParent<Keycard>();
                if (keycard != null)
                {
                    keycard.CollectKeycard(this);
                    return;
                }
            }
        }

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
