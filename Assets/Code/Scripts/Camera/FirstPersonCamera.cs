using UnityEngine;


// Class that runs camera logic for first person camera
public class FirstPersonCamera : MonoBehaviour
{
    // Define variables
    float xRotation = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Processes look based on 
    public void UpdatePitchAndYaw(float mouseX, float mouseY)
    {
        // VERTICAL
        // Modifly private float to clamp it
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation,-90f,90f);

        // Rotate camera locally
        transform.localRotation = Quaternion.Euler(xRotation,0f,0f);

        // HORIZONTAL
        transform.Rotate(Vector3.up * mouseX);
    }
}
