using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const string PlayerPrefsHighScore = "HighestScore";
    public static GameManager Instance  {get; private set; }
    public event EventHandler<OnGameEndEventArgs> OnGameEnd;
    public class OnGameEndEventArgs : EventArgs { public bool isHighScore; };

    public event EventHandler OnHighScoreBeaten;

    public event EventHandler OnScoreMultiplierChanged;

    public event EventHandler<OnScoreChangedEventArgs> OnScoreChanged;
    public class OnScoreChangedEventArgs : EventArgs { public int currentScore; };

    [SerializeField] private Transform _pauseGameScreen;

    private bool _isHighScoreBeaten = false;
    private bool _isGameOver = false;
    private bool _isGamePaused = false;
    private float _gameScore = 0;
    private float _scoreMultiplier = 100;

    private float _multiplierTimer = 0;


    private int _highScore = 0;
    private int _highScoreSession = 0;


    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
        _highScore = PlayerPrefs.GetInt(PlayerPrefsHighScore);
        _multiplierTimer = 0;
        Debug.Log(_highScore);
    }
    void Start()
    {
        Player.Instance.OnPlayerHealthDepleted += Instance_OnGameOver;
        Player.Instance.OnPlayerHealthDecreased += Player_OnHealthDecreased;
    }

    private void Player_OnHealthDecreased(object sender, EventArgs e)
    {
        _scoreMultiplier = 100;
        OnScoreMultiplierChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Instance_OnGameOver(object sender, EventArgs e)
    {
        _isGameOver = true;
        
        if(_highScoreSession > _highScore)
        {
            Debug.Log("Highest score set");
            OnGameEnd?.Invoke(this, new OnGameEndEventArgs
            {
                isHighScore = true
            }); 
            PlayerPrefs.SetInt(PlayerPrefsHighScore, _highScoreSession);

            return;
        }

        OnGameEnd?.Invoke(this, new OnGameEndEventArgs
        {
            isHighScore = false
        });
        PlayerPrefs.SetInt(PlayerPrefsHighScore, _highScoreSession);

        return;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateScore();
        ShowScoreOnUi();
        CheckIfHighScoreIsBeaten();
        CalculateScoreMultiplier();
    }

    private void CalculateScoreMultiplier()
    {
        if (_scoreMultiplier >= 500) return;

       _multiplierTimer += Time.deltaTime;
     
        if(_multiplierTimer > 3f)
        {
            _multiplierTimer = 0;
            _scoreMultiplier += 10;

            OnScoreMultiplierChanged?.Invoke(this, EventArgs.Empty);
        }        
    }

    private void CalculateScore()
    {
        if (!_isGameOver)
        {
            _gameScore += _scoreMultiplier * Time.deltaTime;
        }

    }

    private void CheckIfHighScoreIsBeaten()
    {
        if (_isHighScoreBeaten) return; 

        if( _highScoreSession > _highScore)
        {
            _isHighScoreBeaten = true;
            OnHighScoreBeaten?.Invoke(this, EventArgs.Empty);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        _isGamePaused = true;
        _pauseGameScreen.gameObject.SetActive(true);
    }

    public void UnpauseGame()
    {
        _isGamePaused = false;
        Time.timeScale = 1.0f;
    }

    private void ShowScoreOnUi()
    {
        int integerScore = (int)_gameScore;
        if (integerScore % 10 != 0) return;

        _highScoreSession = integerScore;
        OnScoreChanged?.Invoke(this, new OnScoreChangedEventArgs
        {
            currentScore = _highScoreSession
        });
    }

    public float GetScoreMultiplier()
    {
        float multiplier = _scoreMultiplier / 100;
        return multiplier;
    }

    public int GetHighScore()
    {
        return _highScoreSession;
    }

    public bool IsGameOver()
    {
        return _isGameOver;
    }

    public bool IsGamePaused()
    {
        return _isGamePaused;
    }
}
