using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Endgame : MonoBehaviour
{
    // Variables for UI elements
    public TextMeshProUGUI scoreExplanationText;
    public TextMeshProUGUI scoreExplanationTextOutOfTen;
    public TextMeshProUGUI timePlayedTextExplanation;
    public TextMeshProUGUI timePerPipeTextExplanation;
    public TextMeshProUGUI oxygenWarningsAmountTextExplanation;
    public TextMeshProUGUI distanceCoveredTextExplanation;

    public TextMeshProUGUI endgameText; // Victory or Loss text
    public TextMeshProUGUI scoreText; // 0 if player loses, random score 7 to 10 if wins
    public TextMeshProUGUI timePlayedText; // Time played
    public TextMeshProUGUI timePerPipeText; // Time played divided by 7
    public TextMeshProUGUI oxygenWarningsAmountText; // Number of times an oxygen warning was triggered
    public TextMeshProUGUI distanceCoveredText; // Distance covered as the sum of all 3 components (x, y, z)
    public Image reticle;
    public Button restartButton;
    public Button quitButton;

    private Health _health; // Reference to player health
    private FixLogic _fixLogic; // Reference to the fixing logic
    private WaterLogic _waterLogic; // Reference to the water logic
    private Transform _playerTransform; // Player's transform (captured automatically from prefab)

    private int _score; // Final score for winning
    private float _timePlayed; // Total time played
    private bool _gameEnded; // Check if the game has ended
    private int _oxygenAlertsCount; // Oxygen alerts triggered

    private Vector3 _startPosition; // Start position of the player

    private void Start()
    {
        // Get player transform from prefab
        _playerTransform = GameObject.FindWithTag("Player").transform;

        if (_playerTransform == null)
        {
            Debug.LogError("Player prefab not found!");
            return;
        }

        // Initialize references
        _health = FindObjectOfType<Health>();
        _fixLogic = FindObjectOfType<FixLogic>();
        _waterLogic = FindObjectOfType<WaterLogic>();

        // Capture player's starting position
        _startPosition = _playerTransform.position;

        // Initialize the text elements to be transparent (not visible)
        SetTextVisibility(false);

        //hide buttons
        restartButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        
        //bind buttons
        restartButton.onClick.AddListener(() => { UnityEngine.SceneManagement.SceneManager.LoadScene(0); });
        quitButton.onClick.AddListener(Application.Quit);

        // Start time tracking
        _timePlayed = Time.time;

        // Initialize explanation texts to be invisible
        SetExplanationTextVisibility(0);
    }

    private void Update()
    {
        if (_gameEnded) return;

        var healthDepleted = _health.health <= 0;
        var pipesFixed = _fixLogic.fixedCount >= 7;
        var oxygenDepleted = _waterLogic.currentOxygen <= 1;

        if (healthDepleted || pipesFixed || oxygenDepleted)
        {
            EndGame(pipesFixed);
        }
    }

    private void EndGame(bool isVictory)
    {
        _gameEnded = true; // Stop further updates
        reticle.enabled = false; // Hide the reticle

        // Calculate total time played
        _timePlayed = Time.time - _timePlayed; // Time since the game started
        _oxygenAlertsCount = _waterLogic.oxygenAlertsCount; // Adjusted to use the correct instance

        // Calculate distance covered using player's current position
        Vector3 endPosition = _playerTransform.position;
        float distanceCovered = Vector3.Distance(_startPosition, endPosition);

        // Calculate the score if the player wins
        if (isVictory)
        {
            _score = Random.Range(7, 11); // Random score between 7 and 10
            endgameText.color = Color.green; // Change text color to green
            endgameText.text = "Felicidades!"; // Victory message
        }
        else
        {
            _score = 0; // No score for losing
            endgameText.text = "Perdiste!"; // Loss message
            endgameText.color = Color.red; // Change text color to red
        }

        // Update the UI elements
        UpdateUI();

        // Make the text visible
        SetTextVisibility(true);
        SetExplanationTextVisibility(1f);
    }

    private void UpdateUI()
    {
        scoreText.text = $"{_score}";
        timePlayedText.text = $"{_timePlayed:F2} s";
        timePerPipeText.text = $"{_timePlayed / 7:F2} s";
        oxygenWarningsAmountText.text = $"{_oxygenAlertsCount}";
        distanceCoveredText.text = $"{Vector3.Distance(_startPosition, _playerTransform.position):F2} m";

        // Show buttons
        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    private void SetTextVisibility(bool visible)
    {
        Color color = visible ? Color.white : new Color(1, 1, 1, 0); // White or Transparent
        endgameText.color = color;
        scoreText.color = color;
        timePlayedText.color = color;
        timePerPipeText.color = color;
        oxygenWarningsAmountText.color = color;
        distanceCoveredText.color = color;
    }

    private void SetExplanationTextVisibility(float alpha)
    {
        Color color = new Color(1, 1, 1, alpha); // Set alpha for explanation texts
        scoreExplanationText.color = color;
        scoreExplanationTextOutOfTen.color = color;
        timePlayedTextExplanation.color = color;
        timePerPipeTextExplanation.color = color;
        oxygenWarningsAmountTextExplanation.color = color;
        distanceCoveredTextExplanation.color = color;
    }
}
