using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Public values to change in editor
    public float bulletSpeed = 10.0f;

    // Range values for bullets
    private readonly float bulletRange = 50.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Moves the bullet forward along its forward vector which should be player rotation value
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);

        CheckBulletRemove();
    }

    void CheckBulletRemove()
    {
        // Save reference to current position of bullet
        Vector3 distance = transform.position;

        // Remove bullet if it gets too far in x or y directions
        if(Vector3.Distance(distance,Vector3.zero) >= bulletRange)
        {
            Destroy(gameObject);
        }
    }

    // Runs code when the bullet collides with another collider
    void OnTriggerEnter(Collider other)
    {
        // Remove the bullet if it hits an enemy
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }

    }
}
