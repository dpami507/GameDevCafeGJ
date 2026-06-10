using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    [SerializeField] RectTransform healthbarUICanvas;
    [SerializeField] Vector2 UIOffset;
    [SerializeField] Image healthBar;

    [SerializeField] BaseHealth health;

    private void Start()
    {
        healthbarUICanvas.anchoredPosition = UIOffset;
    }
    private void Update()
    {
        healthBar.fillAmount = health.GetHealthAsPercentage();
    }
}
