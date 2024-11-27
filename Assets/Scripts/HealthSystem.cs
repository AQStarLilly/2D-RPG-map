using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    // Variables
    public int health;
    public string healthStatus;
    public int shield;
    public int lives;

    // Optional XP system variables
    //public int xp;
    //public int level;

    public HealthSystem()
    {
        ResetGame();
    }


    public string ShowHUD()
    {
        // Implement HUD display
        return $"Health: {health}, Lives: {lives}, Status: {healthStatus}"; //XP: {xp}/100, Level: {level}";
    }

    public void TakeDamage(int damage)  
    {
        if (damage < 0)
        {
            Debug.LogWarning("Damage value cannot be negative.");
            return;
        }

        // Implement damage logic
        if (shield >= damage)
        {
            shield -= damage; //All damage absorbed by the shield
        }
        else
        {
            int remainingDamage = damage - shield;
            shield = 0;
            health -= remainingDamage;

            if (health < 0)
                health = 0; //Health can't be less than 0
        }

        UpdateHealthStatus();
    }

    public void Heal(int hp)
    {
        //check if input is negative
        if (hp < 0)
        {
            Debug.LogWarning("Cannot heal with a negative amount.");
            return;
        }

        health += hp;  // add healing amount to health

        //clamp health to the max value of 100
        if (health > 100)
            health = 100;

        UpdateHealthStatus();
    }

    /*public void RegenerateShield(int hp)
    {
        //check if input is negatvie
        if (hp < 0)
        {
            Debug.LogWarning("Cannot regenerate shield with a negative value.");
            return;
        }

        shield += hp; // add regeneration value to shield

        //clamp shield to the max value of 100
        if (shield > 100)
            shield = 100;
    } 

    public void Revive()
    {
        // Implement revive logic
        if (lives > 0)
        {
            lives--;
            health = 100;
            shield = 100;
            UpdateHealthStatus();
        }
        else
        {
            Console.WriteLine("No lives left!");
        }
    } */

    public void ResetGame()
    {
        // Reset all variables to default values
        health = 100;
        //shield = 100;
       // lives = 3;
        //xp = 0;
        //level = 1;
        UpdateHealthStatus();
    }

    private void UpdateHealthStatus()
    {
        if (health <= 10)
        {
            healthStatus = "Imminent Danger";
        }
        else if (health <= 50)
        {
            healthStatus = "Badly Hurt";
        }
        else if (health <= 75)
        {
            healthStatus = "Hurt";
        }
        else if (health <= 90)
        {
            healthStatus = "Healthy";
        }
        else if (health <= 100)
        {
            healthStatus = "Perfect Health";
        }
    }
}
