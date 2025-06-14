using UnityEngine;

public class HazardArea : MonoBehaviour
{
    public int damagePerSecond = 10;

    private void OnTriggerEnter(Collider other)
    {
        PlayerBehaviour player = other.GetComponent<PlayerBehaviour>();
        if (player != null)
        {
            //Dies instantly
            player.TakeDamage(player.maxHealth);
        }

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
