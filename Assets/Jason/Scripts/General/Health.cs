using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour
{
    [Header("Health Info")]
    public float currentHealth;
    public float maxHealth;
    public bool isDead => currentHealth <= 0f;

    public List<StatusEffect> activeStatusEffects = new List<StatusEffect>();

    // For tracking damage resistances
    [Header("Resistances")]
    public float physicalResistance = 0f;
    public float fireResistance = 0f;
    public float poisonResistance = 0f;
    public float mentalResistance = 0f;

    [Header("Health Display")]
    [SerializeField] private HealthBarController healthBar;

    [Header("Damage")]
    [SerializeField] DamageEffectController damageEffectController;

    // Event for health change (for UI update or effects)
    public event Action<float> OnHealthChanged;

    private void Start()
    {
        // Initialize with max health (could be based on player stats or equipment)
        damageEffectController = GetComponent<DamageEffectController>();

        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.SetHealthPercent(1f);

        OnHealthChanged += h =>
        {
            float pct = h / maxHealth;
            healthBar?.SetHealthPercent(pct);
        };
    }

    void Update()
    {
        // Update and apply active status effects
        foreach (var statusEffect in activeStatusEffects)
        {
            statusEffect.duration -= Time.deltaTime;
            if (statusEffect.duration <= 0f)
            {
                activeStatusEffects.Remove(statusEffect);
                continue;
            }
            statusEffect.ApplyEffect(this); // Apply the effect each frame
        }
    }

    //public void UpdateHealthUI(float health)
    //{
    //    healthSlider.value = health / maxHealth;
    //}

    public void TakeDamage(float amount, DamageType damageType)
    {
        if (isDead)
            return;

        // Calculate damage after resistances
        float effectiveDamage = CalculateEffectiveDamage(amount, damageType);

        currentHealth -= effectiveDamage;
        damageEffectController.PlayDamageEffects(currentHealth / maxHealth, amount);
        OnHealthChanged?.Invoke(currentHealth);

        // Apply status effects for specific damage types (e.g., poison from poison damage)
        ApplyStatusEffect(damageType);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    private void Die()
    {
        // Handle death logic, e.g., triggering a death animation, game over, etc.
        Debug.Log($"{gameObject.name} has died.");
    }

    private float CalculateEffectiveDamage(float damage, DamageType damageType)
    {
        float finalDamage = damage;

        switch (damageType)
        {
            case DamageType.Physical:
                finalDamage *= (1f - physicalResistance);
                break;
            case DamageType.Fire:
                finalDamage *= (1f - fireResistance);
                break;
            case DamageType.Poison:
                finalDamage *= (1f - poisonResistance);
                break;
            case DamageType.Mental:
                finalDamage *= (1f - mentalResistance);
                break;
        }

        return Mathf.Max(finalDamage, 0f); // Prevent negative damage
    }

    private void ApplyStatusEffect(DamageType damageType)
    {
        if (damageType == DamageType.Poison)
        {
            // Example: Apply poison status effect that damages over time
            activeStatusEffects.Add(new StatusEffect(StatusEffectType.Poison, 5f));  // Lasting for 5 seconds
        }
    }
}

public enum DamageType
{
    Physical,
    Fire,
    Poison,
    Mental
}

public enum StatusEffectType
{
    Poison,
    Bleed,
    Fear
}

public class StatusEffect
{
    public StatusEffectType type;
    public float duration;

    public StatusEffect(StatusEffectType type, float duration)
    {
        this.type = type;
        this.duration = duration;
    }

    public void ApplyEffect(Health targetHealth)
    {
        // Logic for applying the status effect (e.g., poison deals damage over time)
        if (type == StatusEffectType.Poison)
        {
            targetHealth.TakeDamage(2f, DamageType.Poison); // Poison damage every tick
        }
    }
}
