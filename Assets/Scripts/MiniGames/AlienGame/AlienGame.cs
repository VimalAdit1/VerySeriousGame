using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlienGame : MonoBehaviour,MiniGame
{
    
    [SerializeField]DifficultyScaling timeScaling;
    [SerializeField]String tutorialText;
    
    [SerializeField]GameObject startCutscene;
    [SerializeField] private float cutsceneTime;
    [SerializeField] GameObject endCutscene;
    [SerializeField] private float endCutsceneTime;
    
    [SerializeField] SpriteRenderer referenceSprite;
    [SerializeField] SpriteRenderer currentSprite;
    
    [SerializeField] Wheel frequencyWheel;
    [SerializeField] Wheel amplitudeWheel;
    [SerializeField] float minFrequency=0.0f;
    [SerializeField] float maxFrequency=15.0f;
    [SerializeField] float minAmplitude=0f;
    [SerializeField] float maxAmplitude=0.2f;
    
    [SerializeField] float frequencyThreshold = 0.5f;
    [SerializeField] float amplitudeThreshold = 0.01f;
    
    float targetFrequency;
    float targetAmplitude;
    
    float currentFrequency;
    float currentAmplitude;
    Material referenceMaterial;
    Material currentMaterial;
    int currentLevel = 0;
    
    bool isGameStarted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        referenceMaterial = referenceSprite.material;
        currentMaterial = currentSprite.material;

        if (frequencyWheel != null)
        {
            frequencyWheel.onSpeedUpdate += TuneFrequency;
        }

        if (amplitudeWheel)
        {
            amplitudeWheel.onSpeedUpdate += TuneAmplitude;
        }
    }

    private void TuneAmplitude(float speed, bool isReverse)
    {
        speed *=Time.deltaTime;
        speed = isReverse ? -speed : speed;
        currentAmplitude += speed;
        if (currentAmplitude >= maxAmplitude)
        {
            currentAmplitude = maxAmplitude;
        }
        else if (currentAmplitude < minAmplitude)
        {
            currentAmplitude = minAmplitude;
        }
        currentMaterial.SetFloat(Constants.amplitudeParameter, currentAmplitude);
    }



    private void TuneFrequency(float speed, bool isReverse)
    {
        speed *=Time.deltaTime;
        speed = isReverse ? -speed : speed;
        currentFrequency += speed;
        if (currentFrequency >= maxFrequency)
        {
            currentFrequency = maxFrequency;
        }
        else if (currentFrequency < minFrequency)
        {
            currentFrequency = minFrequency;
        }
        currentMaterial.SetFloat(Constants.frequencyParameter, currentFrequency);
    }
    
    public Action OnGameWon { get; set; }
    public Action OnGameLost { get; set; }
    public void StartMiniGame(int difficulty)
    {
        currentLevel = difficulty;
        targetFrequency = Random.Range(5f, maxFrequency);
        targetAmplitude = Random.Range(0.1f, maxAmplitude);
        
        referenceMaterial.SetFloat(Constants.frequencyParameter, targetFrequency);
        referenceMaterial.SetFloat(Constants.amplitudeParameter, targetAmplitude);

        currentAmplitude = 0.1f;
        currentFrequency = 1f;
        
        currentMaterial.SetFloat(Constants.frequencyParameter, currentFrequency);
        currentMaterial.SetFloat(Constants.amplitudeParameter, currentAmplitude);

        isGameStarted = true;
    }

    public void StopMiniGame()
    {
        Time.timeScale = 0;
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

    void Update()
    {
        if (isGameStarted)
        {
            if (Mathf.Abs(currentFrequency - targetFrequency) < frequencyThreshold && Mathf.Abs(currentAmplitude - targetAmplitude) < amplitudeThreshold)
            {
                OnGameWon?.Invoke();
                isGameStarted = false;
            }
        }
    }

    public IEnumerator PlayEndCutscene()
    {
        yield return  StartCoroutine(PlayCutscene(endCutscene,endCutsceneTime));
        GameManager.instance.OnCutsceneEnd();
    }
}
