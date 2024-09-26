using UnityEngine;
using TMPro;

/// <summary>
/// Manages the ocean's logic, including player oxygen and health levels,
/// and displays the current depth on the UI.
/// </summary>
public class Sea : MonoBehaviour
{
    /// <summary>
    /// Reference to the WaterLogic component managing water-related mechanics.
    /// </summary>
    [Tooltip("Reference to the WaterLogic component.")]
    public WaterLogic waterLogic;

    /// <summary>
    /// The UI GameObject containing all UI elements.
    /// </summary>
    [Tooltip("The UI GameObject containing UI elements.")]
    public GameObject ui;

    /// <summary>
    /// Reference to the player's health component.
    /// </summary>
    [Tooltip("Reference to the player's Health component.")]
    public Health health;

    /// <summary>
    /// TextMeshProUGUI component displaying the player's depth.
    /// </summary>
    [Tooltip("Text component displaying the player's depth.")]
    public TextMeshProUGUI depthText;

    private TextMeshProUGUI _oxygenText;
    private TextMeshProUGUI _healthText;
    private Transform _playerTransform;

    private void Start()
    {
        if (ui == null)
        {
            Debug.LogError("UI GameObject is not assigned.");
            return;
        }

        var canvas = ui.GetComponentInChildren<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("No Canvas component found in the child of UI GameObject.");
            return;
        }

        // Fetch TextMeshProUGUI components for oxygen and health
        var textComponents = canvas.GetComponentsInChildren<TextMeshProUGUI>();

        if (textComponents.Length < 2)
        {
            Debug.LogError("Not enough TextMeshProUGUI components found in the child of Canvas.");
            return;
        }

        _oxygenText = textComponents[0];
        _healthText = textComponents[1];

        if (_oxygenText == null)
        {
            Debug.LogError("No TextMeshProUGUI component found for oxygen.");
            return;
        }

        if (waterLogic == null)
        {
            Debug.LogError("WaterLogic reference is missing.");
        }

        // Find the player transform
        _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_playerTransform == null)
        {
            Debug.LogError("Player object not found. Ensure it is tagged as 'Player'.");
        }
    }

    private void Update()
    {
        // Check for missing references to prevent errors during runtime
        if (waterLogic == null || _oxygenText == null || _healthText == null || _playerTransform == null)
            return;

        // Update oxygen display
        UpdateOxygenDisplay();

        // Update health display
        UpdateHealthDisplay();

        // Update depth display
        UpdateDepthDisplay();
    }

    /// <summary>
    /// Updates the oxygen display and changes its color based on the current oxygen level.
    /// </summary>
    private void UpdateOxygenDisplay()
    {
        _oxygenText.text = $"{waterLogic.currentOxygen}";
        _oxygenText.color = waterLogic.currentOxygen switch
        {
            <= 6 => Color.red,
            <= 15 => new Color(1f, 0.5f, 0f),
            <= 30 => Color.yellow,
            _ => Color.green
        };
    }

    /// <summary>
    /// Updates the health display and changes its color based on the current health level.
    /// </summary>
    private void UpdateHealthDisplay()
    {
        _healthText.text = $"{health.health}";
        _healthText.color = health.health switch
        {
            <= 1 => Color.red,
            <= 20 => new Color(1f, 0.5f, 0f),
            <= 50 => Color.yellow,
            _ => Color.green
        };
    }

    /// <summary>
    /// Updates the depth display based on the player's position.
    /// </summary>
    private void UpdateDepthDisplay()
    {
        float depth = -_playerTransform.position.y;
        depth = Mathf.Max(0, depth);
        depthText.text = depth.ToString("F0");
    }
}
