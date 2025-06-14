using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform Orientation;

    float xRotation;
    float yRotation;

    void Start()
    {
        
    }

    void Update()
    {
        //Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        //Rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void ResetLook()
    {
        xRotation = 0f;
        yRotation = 0f;

        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        Orientation.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
}