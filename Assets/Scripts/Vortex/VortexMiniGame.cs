using System;
using System.Collections;
using UnityEngine;

public class VortexMiniGame : MonoBehaviour,MiniGame
{
   
    [SerializeField]DifficultyScaling speedScaling;
    [SerializeField]DifficultyScaling timeScaling;
    [SerializeField]DifficultyScaling decelerationScaling;
    [SerializeField]String tutorialText;
    [SerializeField]Wheel wheel;
    int currentLevel = 0;
    float decelerationRate;
    private float speedToReach;
    private float currentSpeed;
    bool gameStarted = false;
    void Start()
    {
        
    }

    void Update()
    {
        if (!gameStarted)
        {
            return;
        }
        Debug.Log("Current  Speed "+currentSpeed);
        if (currentSpeed >= speedToReach)
        {
            OnGameWon?.Invoke();
            StopMiniGame();
        }
        if (currentSpeed > 0)
        {
            currentSpeed -= decelerationRate * Time.deltaTime;
        }
    }

    public Action OnGameWon { get; set; }
    public Action OnGameLost { get; set; }
    public void StartMiniGame(int difficulty)
    {
        currentLevel = difficulty;
        speedToReach = speedScaling.GetValue(difficulty);
        decelerationRate = decelerationScaling.GetValue(difficulty);
        if (wheel)
        {
            wheel.onSpeedUpdate += OnWheelUpdate;
        }
        gameStarted = true;
    }

    void OnWheelUpdate(float speed, bool isReverse)
    {
        currentSpeed += speed * Time.deltaTime;
    }

    public void StopMiniGame()
    {
        Time.timeScale = 0;
        gameStarted = false;
    }

    public string GetMiniGameTutorial()
    {
        return tutorialText;
    }

    public float GetMiniGameTime(int difficulty)
    {
        return timeScaling.GetValue(currentLevel);;
    }

    public bool IsGameOverOnTimeEnd()
    {
        return true;
    }
}
