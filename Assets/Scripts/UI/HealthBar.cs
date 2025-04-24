using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    public Gradient Gradient;
    [SerializeField] public Image fill;

    public void Start()
    {
        fill.color = Gradient.Evaluate(healthSlider.normalizedValue);
    }
    public void SetHealth(float health)
    {
        healthSlider.value = health;

        fill.color = Gradient.Evaluate(healthSlider.normalizedValue);
    }

    public void setMaxHealth(float health)
    {
        healthSlider.maxValue = health;
    }

    public void AddHealth(float health)
    {
        float currentHealth = healthSlider.value;
        currentHealth = currentHealth + health;
        SetHealth(currentHealth);
    }

    public void SubtractHealth(float health)
    {
        float currentHealth = healthSlider.value;
        currentHealth = currentHealth - health;
        SetHealth(currentHealth);
    }

    public float readHealth()
    {
        return healthSlider.value;
    }

    public float getMaxHealth()
    {
        return healthSlider.maxValue;
    }

}
