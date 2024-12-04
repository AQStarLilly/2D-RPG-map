using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;
    public TextMeshProUGUI healthDisplay;

    public delegate void OnDeathEvent(GameObject obj);
    public static event OnDeathEvent OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthDisplay();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Current health: {currentHealth}");
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthDisplay();

        if(currentHealth <= 0)
        {           
            Die();
        }
    }
    
    private void UpdateHealthDisplay()
    {
        if(healthDisplay != null)
        {
            healthDisplay.text = $"Health: {currentHealth}/{maxHealth}";
        }
    }
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");       
        GameManager.Instance.RemoveObjectFromMap(gameObject);
        HealthSystem.OnDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }

}
