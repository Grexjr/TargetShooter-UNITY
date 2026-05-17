using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    // CONSTANTS
    private readonly String healthDivider = "/";

    // HEALTH BAR UI VARIABLES
    [SerializeField] private Health playerHealth;
    [SerializeField] private Slider healthbar;
    [SerializeField] private TextMeshProUGUI healthText;

    void Start()
    {
        // Subscribe to necessary events
        playerHealth.onHealthChanged += UpdateHealth;

        // Initialize values
        healthbar.maxValue = playerHealth.MaxHealth;
        healthbar.value = playerHealth.CurrentHealth;
        healthText.text = playerHealth.CurrentHealth + healthDivider + playerHealth.MaxHealth;
    }

    private void UpdateHealth(int current, int max)
    {
        healthbar.maxValue = max;
        healthbar.value = current;
        healthText.text = current + healthDivider + max;
    }
}
