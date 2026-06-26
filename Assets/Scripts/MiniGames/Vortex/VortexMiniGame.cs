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
    [SerializeField] private ParticleSystem bubbleVFX;
    
    [SerializeField]GameObject startCutscene;
    [SerializeField] private float cutsceneTime;
    [SerializeField] GameObject endCutscene;
    [SerializeField] private float endCutsceneTime;
    [SerializeField] GameObject looseCutscene;
    [SerializeField] private float looseCutsceneTime;
    
    [SerializeField] Sprite winSprite;
    [SerializeField] Sprite looseSprite;

    [Space(5), Header("Amb Clips")]
    public List<AudioClip> miniGameAmbience;
    public List<AudioClip> winAmbience;
    public List<AudioClip> loseAmbience;

    [Space(1)]
    public List<AudioClip> winSFX;
    public List<AudioClip> loseSFX;

    public AudioClip waterVortex;
    public float vortexPlayEveryXSecs = 8;

    int currentLevel = 0;
    float decelerationRate;
    private float speedToReach;
    private float currentSpeed;
    private float previousSpeed;
    bool gameStarted = false;
    float vortexAudioTimer;

    private void Start()
    {
        vortexAudioTimer = 0;
    }

    void Update()
    {
        if (!gameStarted)
        {
            return;
        }

        vortexAudioTimer -= Time.deltaTime;

        if (vortexAudioTimer <= 0f)
        {
            AudioManager.instance.PlaySFX(waterVortex);
            vortexAudioTimer = vortexPlayEveryXSecs;
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
        if (currentSpeed <= 0)
        {
            animator.SetTrigger(Constants.idleTag);
            bubbleVFX.Stop();
        }
        else if (currentSpeed < speedToReach*.30)
        {
            animator.SetTrigger(Constants.slowTag);
            bubbleVFX.Stop();
        }
        else if (currentSpeed < speedToReach*.80)
        {
            animator.SetTrigger(Constants.mediumTag);
            if(!bubbleVFX.isPlaying)bubbleVFX.Play();
        }
        else
        {
            animator.SetTrigger(Constants.fastTag);
            if(!bubbleVFX.isPlaying)bubbleVFX.Play();
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

        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return StartCoroutine(GameManager.instance.GetGameUI().StartCountDown());
        
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

    public IEnumerator PlayLooseCutscene()
    {
        yield return  StartCoroutine(PlayCutscene(looseCutscene,looseCutsceneTime));
        GameManager.instance.OnLooseCutsceneEnd();
    }

    public Sprite GetEndScreenSprite(bool isWin)
    {
        return isWin? winSprite : looseSprite;
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
    
    private void OnDestroy()
    {
        OnGameWon = null;
        OnGameLost = null;
    }
}
