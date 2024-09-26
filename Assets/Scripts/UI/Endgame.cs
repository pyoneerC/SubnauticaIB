using System;
using UnityEngine;
using TMPro;

public class Endgame : MonoBehaviour
{
    //theres a blakck screen that overlays all this on endgame please make all this z up or shit and also we need to to the same to the explanation texts of what are the values cuz i only puyyed values so far now i will put the explicative elements like 'your score' you just fade this at the beggining and show it at the end with the real values

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

    private Health _health; // Reference to player health
    private FixLogic _fixLogic; // Reference to the fixing logic
    private Transform playerTransform; // Player's transform (captured automatically from prefab)

    private int score = 0; // Final score for winning
    private float timePlayed = 0f; // Total time played
    private bool gameEnded = false; // Check if the game has ended
    private int oxygenWarnings = 0; // Oxygen warnings triggered

    private Vector3 startPosition; // Start position of the player

    private void Start()
    {
        // Get player transform from prefab
        playerTransform = GameObject.FindWithTag("Player").transform;

        if (playerTransform == null)
        {
            Debug.LogError("Player prefab not found!");
            return;
        }

        // Capture player's starting position
        startPosition = playerTransform.position;

        // Initialize the text elements to be transparent (not visible)
        SetTextVisibility(false);

        // Start time tracking
        timePlayed = Time.time;

        scoreExplanationText.color = new Color(1, 1, 1, 0);
        scoreExplanationTextOutOfTen.color = new Color(1, 1, 1, 0);
        timePlayedTextExplanation.color = new Color(1, 1, 1, 0);
        timePerPipeTextExplanation.color = new Color(1, 1, 1, 0);
        oxygenWarningsAmountTextExplanation.color = new Color(1, 1, 1, 0);
        distanceCoveredTextExplanation.color = new Color(1, 1, 1, 0);
    }

    private void Update()
    {
        if (gameEnded) return;

        // Continuously check the player's health and the number of pipes fixed
        if (_health.health <= 0)
        {
            EndGame(false); // Loss
        }
        else if (_fixLogic.fixedCount >= 7)
        {
            EndGame(true); // Victory
        }
    }

    private void EndGame(bool isVictory)
    {
        gameEnded = true; // Stop further updates

        // Calculate total time played
        timePlayed = Time.time - timePlayed; // Time since the game started

        // Calculate distance covered using player's current position
        Vector3 endPosition = playerTransform.position;
        float distanceCovered = Vector3.Distance(startPosition, endPosition);

        // Calculate the score if the player wins
        if (isVictory)
        {
            score = UnityEngine.Random.Range(7, 11); // Random score between 7 and 10
            endgameText.text = "Victory!";
        }
        else
        {
            score = 0; // No score for losing
            endgameText.text = "You Lost!";
        }

        // Update the UI elements
        scoreText.text = $"Score: {score}";
        timePlayedText.text = $"Time Played: {timePlayed:F2} seconds";
        timePerPipeText.text = $"Time per Pipe: {timePlayed / 7:F2} seconds";
        oxygenWarningsAmountText.text = $"Oxygen Warnings: {oxygenWarnings}";
        distanceCoveredText.text = $"Distance Covered: {distanceCovered:F2} units";

        // Make the text visible
        SetTextVisibility(true);
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

    // Call this method whenever an oxygen warning is triggered
    public void OnOxygenWarningTriggered()
    {
        oxygenWarnings++;
    }
}
