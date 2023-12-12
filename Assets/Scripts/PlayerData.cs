using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    [Header("PlayerData")]

    public float health = 100f;
    public float maxHealth = 100f;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float healthRegen = 1f;
    public float staminaRegen = 2f;
    public float staminaRegenDelay = 1f;
    public float staminaRegenDelayTimer = 0f;

    public float healthRegenDelay = 1f;
    public float healthRegenDelayTimer = 0f;
    [Header("UI")]
    public Slider healthSlider;
    public Slider staminaSlider;


    private void Update() {
        if (staminaRegenDelayTimer <= staminaRegenDelay) {
            staminaRegenDelayTimer += Time.deltaTime;
        }
        if (staminaRegenDelayTimer > staminaRegenDelay) {
            staminaRegenDelayTimer = staminaRegenDelay;
        }
        if (staminaRegenDelayTimer == staminaRegenDelay) {
            if (stamina < maxStamina) {
                stamina += staminaRegen * Time.deltaTime;
            }
            if (stamina > maxStamina) {
                stamina = maxStamina;
            }
        }
        if (healthRegenDelayTimer <= healthRegenDelay) {
            healthRegenDelayTimer -= Time.deltaTime;
        }
        if (healthRegenDelayTimer > healthRegenDelay) {
            healthRegenDelayTimer = healthRegenDelay;
        }
        if (healthRegenDelayTimer == healthRegenDelay) {
            if (health < maxHealth) {
                health += healthRegen * Time.deltaTime;
            }
            if (health > maxHealth) {
                health = maxHealth;
            }
        }
        if (health <= 0) {
            health = 0;
            SceneManager.LoadScene(8);
        }
    }

    private void LateUpdate() {
        // Map health to slider
        healthSlider.value = health / maxHealth;
        // Map stamina to slider
        staminaSlider.value = stamina / maxStamina;
    }

    public bool RunStamina() {
        if (stamina > 0) {
            stamina -= 10f * Time.deltaTime;
            staminaRegenDelayTimer = 0;
            return true;
        } else {
            stamina = 0;
            return false;
        }
    }

    public bool TakeDamage(float damage) {
        if (health > 0) {
            health -= damage;
            healthRegenDelayTimer = 0;
            return true;
        } else {
            health = 0;
            return false;
        }
    }
}
