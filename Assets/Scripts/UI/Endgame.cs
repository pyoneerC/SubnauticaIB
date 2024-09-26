using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Endgame : MonoBehaviour
{
    public TextMeshProUGUI scoreExplanationText;
    public TextMeshProUGUI scoreExplanationTextOutOfTen;
    public TextMeshProUGUI timePlayedTextExplanation;
    public TextMeshProUGUI timePerPipeTextExplanation;
    public TextMeshProUGUI oxygenWarningsAmountTextExplanation;
    public TextMeshProUGUI distanceCoveredTextExplanation;

    public TextMeshProUGUI endgameText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timePlayedText;
    public TextMeshProUGUI timePerPipeText;
    public TextMeshProUGUI oxygenWarningsAmountText;
    public TextMeshProUGUI distanceCoveredText;
    public Image reticle;
    public Button restartButton;
    public Button quitButton;

    public Image googlesSupportA;
    public Image googlesSupportB;
    public Image googles;

    public Image minimap;
    public TextMeshProUGUI oxygenCount;
    public TextMeshProUGUI oxygenCountExplanation;
    public TextMeshProUGUI heightCount;
    public TextMeshProUGUI heightCountExplanation;
    public TextMeshProUGUI healthCount;
    public TextMeshProUGUI healthCountExplanation;

    private Health _health;
    private FixLogic _fixLogic;
    private WaterLogic _waterLogic;
    private Transform _playerTransform;

    private int _score;
    private float _timePlayed;
    private bool _gameEnded;
    private int _oxygenAlertsCount;

    private Vector3 _startPosition;

    private void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").transform;

        if (_playerTransform == null)
        {
            Debug.LogError("Player prefab not found!");
            return;
        }

        _health = FindObjectOfType<Health>();
        _fixLogic = FindObjectOfType<FixLogic>();
        _waterLogic = FindObjectOfType<WaterLogic>();

        _startPosition = _playerTransform.position;

        SetTextVisibility(false);

        restartButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        _timePlayed = Time.time;

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
        _gameEnded = true;
        reticle.enabled = false;

        _timePlayed = Time.time - _timePlayed;
        _oxygenAlertsCount = _waterLogic.oxygenAlertsCount;

        if (isVictory)
        {
            _score = Random.Range(7, 11);
            endgameText.text = "Felicidades!";
        }
        else
        {
            _score = 0;
            endgameText.text = "Perdiste!";
        }

        UpdateUI();

        DisablePlayerInput();

        SetTextVisibility(true);
        SetExplanationTextVisibility(1f);

        endgameText.color = isVictory ? Color.green : Color.red;
        scoreText.color = isVictory ? Color.green : Color.red;

        googlesSupportA.gameObject.SetActive(false);
        googlesSupportB.gameObject.SetActive(false);
        googles.gameObject.SetActive(false);

        minimap.gameObject.SetActive(false);
        oxygenCount.gameObject.SetActive(false);
        oxygenCountExplanation.gameObject.SetActive(false);
        heightCount.gameObject.SetActive(false);
        heightCountExplanation.gameObject.SetActive(false);
        healthCount.gameObject.SetActive(false);
        healthCountExplanation.gameObject.SetActive(false);
    }

    private void DisablePlayerInput()
    {
        var player = GameObject.FindWithTag("Player");

        if (player == null) return;

        var playerMovement = player.GetComponent<FirstPersonController>();
        playerMovement.enabled = false;
    }


    private void UpdateUI()
    {
        scoreText.text = $"{_score}";
        timePlayedText.text = $"{_timePlayed:F2} s";
        timePerPipeText.text = $"{_timePlayed / 7:F2} s";
        oxygenWarningsAmountText.text = $"{_oxygenAlertsCount}";
        distanceCoveredText.text = $"{Vector3.Distance(_startPosition, _playerTransform.position):F2} m";

        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);

        restartButton.interactable = true;
        quitButton.interactable = true;

        restartButton.onClick.AddListener(() => { UnityEngine.SceneManagement.SceneManager.LoadScene(0); });
        quitButton.onClick.AddListener(Application.Quit);
    }

    private void SetTextVisibility(bool visible)
    {
        Color color = visible ? Color.white : new Color(1, 1, 1, 0);
        endgameText.color = color;
        scoreText.color = color;
        timePlayedText.color = color;
        timePerPipeText.color = color;
        oxygenWarningsAmountText.color = color;
        distanceCoveredText.color = color;
    }

    private void SetExplanationTextVisibility(float alpha)
    {
        Color color = new Color(1, 1, 1, alpha);
        scoreExplanationText.color = color;
        scoreExplanationTextOutOfTen.color = color;
        timePlayedTextExplanation.color = color;
        timePerPipeTextExplanation.color = color;
        oxygenWarningsAmountTextExplanation.color = color;
        distanceCoveredTextExplanation.color = color;
    }
}
