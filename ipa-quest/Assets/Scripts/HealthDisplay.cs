
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthDisplay : MonoBehaviour
{
    public Slider healthBarSlider;
    public int currentHealth;
    public int maxHealth;
    public TextMeshProUGUI healthText;
    

    private void Start()
    {
        currentHealth = PlayerController.Instance.playerHealth.HealthValue;
        maxHealth = PlayerController.Instance.playerHealth.MaxHealth;
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    private void Update()
    {
        healthBarSlider.maxValue = PlayerController.Instance.playerHealth.MaxHealth;
        healthBarSlider.value = PlayerController.Instance.playerHealth.HealthValue;
        healthText.text = healthBarSlider.value.ToString() + "/" + healthBarSlider.maxValue.ToString();
    }
}