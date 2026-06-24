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
    
    [SerializeField]GameObject startCutscene;
    [SerializeField] private float cutsceneTime;
    [SerializeField] GameObject endCutscene;
    [SerializeField] private float endCutsceneTime;
    
    
    int currentLevel = 0;
    float decelerationRate;
    private float speedToReach;
    private float currentSpeed;
    private float previousSpeed;
    bool gameStarted = false;
    

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
        if (Mathf.Approximately(previousSpeed, currentSpeed))
        {
            currentSpeed -= decelerationRate * Time.deltaTime;
        }
        else
        {
            previousSpeed = currentSpeed;
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

    public IEnumerator PlayStartCutscene()
    {
        yield return StartCoroutine(PlayCutscene(startCutscene,cutsceneTime));
        GameManager.instance.PrepareMiniGame();
    }

    private IEnumerator PlayCutscene(GameObject cutscene, float cutscenelength)
    {
        Debug.Log("Playing Cutscene");
        GameObject newCutscene = Instantiate(cutscene, GameManager.instance.GetCanvas().transform);
        newCutscene.transform.SetParent(GameManager.instance.GetCanvas().transform);
        yield return new WaitForSecondsRealtime(cutscenelength);
        Debug.Log("Cutscene Ended");
        Destroy(newCutscene);
    }

    public IEnumerator PlayEndCutscene()
    {
        yield return  StartCoroutine(PlayCutscene(endCutscene,endCutsceneTime));
        GameManager.instance.OnCutsceneEnd();
    }
}
