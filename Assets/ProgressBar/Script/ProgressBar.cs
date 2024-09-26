using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour
{
    [Header("Title Settings")]
    [SerializeField] private string title;
    [SerializeField] private Color titleColor = Color.white;
    [SerializeField] private Font titleFont;
    [SerializeField] private int titleFontSize = 14;

    [Header("Bar Settings")]
    [SerializeField] private Color barColor = Color.green;
    [SerializeField] private Color barBackgroundColor = Color.gray;
    [SerializeField] private Sprite barBackgroundSprite;
    [Range(1f, 100f)]
    [SerializeField] private int alertThreshold = 20;
    [SerializeField] private Color barAlertColor = Color.red;

    [Header("Sound Alert")]
    [SerializeField] private AudioClip alertSound;
    [SerializeField] private bool repeatAlert = false;
    [SerializeField] private float repeatRate = 1f;

    private Image bar;
    private Image barBackground;
    private TextMeshProUGUI titleText;
    private AudioSource audioSource;

    private float barValue;
    public float BarValue
    {
        get => barValue;
        set
        {
            barValue = Mathf.Clamp(value, 0, 100);
            UpdateProgressBar(barValue);
        }
    }

    private void Awake()
    {
        bar = transform.Find("Bar").GetComponent<Image>();
        barBackground = transform.Find("BarBackground").GetComponent<Image>();
        titleText = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        InitializeUI();
        UpdateProgressBar(barValue);
    }

    private void InitializeUI()
    {
        titleText.text = title;
        titleText.color = titleColor;
        titleText.fontSize = titleFontSize;

        bar.color = barColor;
        barBackground.color = barBackgroundColor;
        barBackground.sprite = barBackgroundSprite;
    }

    private void UpdateProgressBar(float value)
    {
        bar.fillAmount = value / 100;
        titleText.text = $"{title} {value:F0}%";

        bar.color = value <= alertThreshold ? barAlertColor : barColor;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            // Update the preview in the editor
            UpdatePreviewInEditor();
        }
        else
        {
            HandleSoundAlerts();
        }
    }

    private void UpdatePreviewInEditor()
    {
        UpdateProgressBar(50); // Example for preview
        InitializeUI();
    }

    private void HandleSoundAlerts()
    {
        if (barValue <= alertThreshold && repeatAlert && Time.time > audioSource.time + repeatRate)
        {
            audioSource.PlayOneShot(alertSound);
        }
    }
}
