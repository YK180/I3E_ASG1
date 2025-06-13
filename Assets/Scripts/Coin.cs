using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;
    public float spinSpeed = 100f; // Degrees per second

    private void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("Player"))
        {
            // Add to player's coin count
            PlayerBehaviour collector = other.GetComponent<PlayerBehaviour>();
            if (collector != null)
            {
                collector.CollectCoin(value);
            }

            // Destroy coin
            Destroy(gameObject);
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Spin around the Z-axis
        transform.Rotate(0, 0 , spinSpeed * Time.deltaTime);
        
    }


}



