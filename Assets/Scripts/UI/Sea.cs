using UnityEngine;
using TMPro;

public class Sea : MonoBehaviour
{
    public WaterLogic waterLogic;
    public GameObject ui;
    public Health health;

    private TextMeshProUGUI _oxygenText;
    private TextMeshProUGUI _healthText;

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

        _oxygenText = canvas.GetComponentInChildren<TextMeshProUGUI>();
        _healthText = canvas.GetComponentsInChildren<TextMeshProUGUI>()[1];

        if (_oxygenText == null)
        {
            Debug.LogError("No TextMeshProUGUI component found in the child of Canvas.");
            return;
        }

        if (waterLogic == null)
        {
            Debug.LogError("WaterLogic reference is missing.");
        }
    }

    private void Update()
    {
        if (!waterLogic || !_oxygenText) return;

        _oxygenText.text = $"{waterLogic.currentOxygen}";

        _oxygenText.color = waterLogic.currentOxygen switch
        {
            <= 6 => Color.red,
            <= 15 => new Color(1f, 0.5f, 0f),
            <= 21 => Color.yellow,
            _ => Color.green
        };

        _healthText.text = $"{health.health}";

        _healthText.color = health.health switch
        {
            <= 35 => Color.red,
            <= 70 => new Color(1f, 0.5f, 0f),
            _ => Color.green
        };
    }
}