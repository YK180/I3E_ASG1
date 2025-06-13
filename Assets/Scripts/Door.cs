using UnityEngine;

public class Door : MonoBehaviour
{
    public float openAngle = 90f;     // Angle to rotate when door opens
    public float openSpeed = 2f;      // Speed of door opening/closing

    private bool isOpen = false;      // Door state
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        // Store initial closed rotation
        closedRotation = transform.rotation;
        // Calculate open rotation by rotating around Y axis
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }

    // Update is called once per frame
    void Update()
    {
        // Smoothly rotate door to target rotation based on isOpen state
        if (isOpen)
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, closedRotation, Time.deltaTime * openSpeed);
    }
    

}
