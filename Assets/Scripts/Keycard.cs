using UnityEngine;

public class Keycard : MonoBehaviour
{
    public string keycardID = "Keycard01";  // Optional, for future multi-keycard logic

    public float spinSpeed = 100f; // Degrees per second

    public void CollectKeycard(PlayerBehaviour player)
    {
        player.hasKeycard = true;
        Debug.Log("Keycard collected!");
        Destroy(gameObject);  // Remove keycard from scene
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Spin around the Z-axis
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
        
    }
}
