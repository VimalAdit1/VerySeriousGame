using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VortexMiniGame : MonoBehaviour,MiniGame
{
   
    [SerializeField]DifficultyScaling speedScaling;
    [SerializeField]DifficultyScaling timeScaling;
    [SerializeField]DifficultyScaling decelerationScaling;
    [SerializeField]String tutorialText;
    [SerializeField]Wheel wheel;
    [SerializeField] Animator animator;
    
    [SerializeField]GameObject startCutscene;
    [SerializeField] private float cutsceneTime;
    [SerializeField] GameObject endCutscene;
    [SerializeField] private float endCutsceneTime;

    [Space(5), Header("Amb Clips")]
    public List<AudioClip> miniGameAmbience;
    public List<AudioClip> winAmbience;
    public List<AudioClip> loseAmbience;

    [Space(1)]
    public List<AudioClip> winSFX;
    public List<AudioClip> loseSFX;


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

        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        if (currentSpeed == 0)
        {
            animator.SetTrigger(Constants.idleTag);
        }
        else if (currentSpeed < speedToReach*.30)
        {
            animator.SetTrigger(Constants.slowTag);
        }
        else if (currentSpeed < speedToReach*.80)
        {
            animator.SetTrigger(Constants.mediumTag);
        }
        else
        {
            animator.SetTrigger(Constants.fastTag);
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

    public AudioClip GetGameWonAmb()
    {
        if (winAmbience.Count == 0)
            return null;

        int randVal = UnityEngine.Random.Range(0, winAmbience.Count);
        return winAmbience[randVal];
    }

    public AudioClip GetGameWonSFX()
    {
        if (winSFX.Count == 0)
            return null;

        int randVal = UnityEngine.Random.Range(0, winSFX.Count);
        return winSFX[randVal];
    }

    public AudioClip GetGameLostAmb()
    {
        if (loseAmbience.Count == 0)
            return null;

        int randVal = UnityEngine.Random.Range(0, loseAmbience.Count);
        return loseAmbience[randVal];
    }

    public AudioClip GetGameLostSFX()
    {
        if (loseSFX.Count == 0)
            return null;

        int randVal = UnityEngine.Random.Range(0, loseSFX.Count);
        return loseSFX[randVal];
    }

    public AudioClip GetMiniGameAmb()
    {
        if (miniGameAmbience.Count == 0)
            return null;

        int randVal = UnityEngine.Random.Range(0, miniGameAmbience.Count);
        return miniGameAmbience[randVal];
    }
}
