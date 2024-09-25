using UnityEngine;
using TMPro;

public class Sea : MonoBehaviour
{
    public WaterLogic waterLogic;
    public GameObject ui;
    public Health health;
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

        _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_playerTransform == null)
        {
            Debug.LogError("Player object not found. Ensure it is tagged as 'Player'.");
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
            <= 30 => Color.yellow,
            _ => Color.green
        };

        _healthText.text = $"{health.health}";
        _healthText.color = health.health switch
        {
            <= 1 => Color.red,
            <= 20 => new Color(1f, 0.5f, 0f),
            <= 50 => Color.yellow,
            _ => Color.green
        };

        if (_playerTransform == null) return;
        float depth = -_playerTransform.position.y;
        depth = Mathf.Max(0, depth);
        depthText.text = depth.ToString("F0");
    }
}
