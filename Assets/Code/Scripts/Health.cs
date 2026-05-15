using System;
using UnityEngine;

public class Health : MonoBehaviour
{

    public int maxHealth, currentHealth;

    // Initialize health on awake
    void Awake()
    {
        maxHealth = 10; // Default value
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // Clamps health to zero
        currentHealth = Math.Max(0,currentHealth - damage);
    }

    public void Heal(int heal)
    {
        // Clamps health to max health
        currentHealth = Math.Min(maxHealth, currentHealth + heal);
    }

    /// <summary>
    /// Checks if the entity is dead or alive.
    /// </summary>
    /// <returns>True if dead, False if alive</returns>
    public Boolean CheckDeath()
    {
        if(currentHealth <= 0) return true;
        return false;
    }

    // Resets health to max health
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }


}
