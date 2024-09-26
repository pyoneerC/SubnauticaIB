using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the behavior of a progress bar, including updating the fill amount, color, and title.
/// Can be used to visually represent a task's progress.
/// </summary>
[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("The background image of the progress bar.")]
    [SerializeField] private Image barBackground;

    [Tooltip("The image that represents the filled portion of the progress bar.")]
    [SerializeField] private Image bar;

    [Tooltip("The text component that displays the progress title.")]
    [SerializeField] private TextMeshProUGUI title;

    [Header("Settings")]
    [Tooltip("The current value of the progress bar (0 to 100).")]
    [Range(0f, 100f)]
    [SerializeField] private float barValue = 20f;

    [Tooltip("The color of the filled portion of the progress bar.")]
    [SerializeField] private Color barColor = Color.green;

    /// <summary>
    /// Initializes the progress bar and updates the UI components based on the current value.
    /// </summary>
    private void Start()
    {
        ValidateComponents();
        UpdateProgressBar(barValue);
    }

    /// <summary>
    /// Updates the progress bar's fill amount, color, and text based on the provided value.
    /// </summary>
    /// <param name="value">The current progress value (0 to 100).</param>
    private void UpdateProgressBar(float value)
    {
        // Ensure the value is clamped between 0 and 100.
        value = Mathf.Clamp(value, 0f, 100f);

        if (bar != null)
        {
            // Update the bar's fill amount and color.
            bar.fillAmount = value / 100f;
            bar.color = barColor;
        }

        if (title != null)
        {
            // Update the title text to reflect the current value.
            title.text = $"Fixing... ({value:F0}%)";
        }
    }

    /// <summary>
    /// Called automatically by Unity whenever a serialized field is modified in the Inspector.
    /// Ensures that changes in the Inspector are immediately reflected in the UI.
    /// </summary>
    private void OnValidate()
    {
        ValidateComponents();
        UpdateProgressBar(barValue);
    }

    /// <summary>
    /// Validates that all required UI components (bar, title) are assigned, and logs an error if any are missing.
    /// </summary>
    private void ValidateComponents()
    {
        if (bar == null)
        {
            Debug.LogError("Bar component is missing. Please assign a valid Image for the progress bar.");
        }

        if (title == null)
        {
            Debug.LogError("Title component is missing. Please assign a valid TextMeshProUGUI for the title.");
        }
    }

    /// <summary>
    /// Sets the value of the progress bar and updates the UI accordingly.
    /// </summary>
    /// <param name="newValue">The new progress value (0 to 100).</param>
    public void SetValue(float newValue)
    {
        barValue = newValue;
        UpdateProgressBar(barValue);
    }

    /// <summary>
    /// Sets the color of the progress bar and updates the UI accordingly.
    /// </summary>
    /// <param name="newColor">The new color for the filled portion of the progress bar.</param>
    public void SetColor(Color newColor)
    {
        barColor = newColor;
        UpdateProgressBar(barValue);
    }
}
