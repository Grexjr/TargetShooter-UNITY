using System;
using UnityEngine;

public class Health : MonoBehaviour
{

    // HEALTH VARIABLES
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    // HEALTH EVENTS
    public System.Action<int,int> onHealthChanged;

    // Initialize health on awake
    void Awake()
    {
        MaxHealth = 10; // Default value
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        // Clamps health to zero
        CurrentHealth = Math.Max(0,CurrentHealth - damage);
        onHealthChanged?.Invoke(CurrentHealth,MaxHealth);
    }

    public void Heal(int heal)
    {
        // Clamps health to max health
        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + heal);
        onHealthChanged?.Invoke(CurrentHealth,MaxHealth);
    }

    /// <summary>
    /// Checks if the entity is dead or alive.
    /// </summary>
    /// <returns>True if dead, False if alive</returns>
    public Boolean CheckDeath()
    {
        if(CurrentHealth <= 0) return true;
        return false;
    }

    // Resets health to max health
    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
        onHealthChanged?.Invoke(CurrentHealth,MaxHealth);
    }


}
