using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] Image healthBar;

    [SerializeField] BaseHealth health;

    private void Update()
    {
        if(health == null)
        {
            health = FindFirstObjectByType<PlayerHealth>();
            return;
        }
        healthBar.fillAmount = health.GetHealthAsPercentage();
    }
}
