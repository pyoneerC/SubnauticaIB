using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image barBackground;
    [SerializeField] private Image bar;
    [SerializeField] private TextMeshProUGUI title;

    [Header("Settings")]
    [Range(0f, 100f)]
    [SerializeField] private float barValue = 20f;
    [SerializeField] private Color barColor = Color.green;

    private void Start()
    {
        if (bar == null || title == null)
        {
            Debug.LogError("Bar or Title component is missing.");
            return;
        }

        UpdateProgressBar(barValue);
    }

    private void UpdateProgressBar(float value)
    {
        bar.fillAmount = value / 100f;
        bar.color = barColor;
        title.text = $"Fixing... ({value:F0}%)";
    }

    private void OnValidate()
    {
        UpdateProgressBar(barValue);
    }
}