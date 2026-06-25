using System;
using System.Collections;
using UnityEngine;

public class MerryGoRoundGame : MonoBehaviour,MiniGame
{
    [SerializeField]DifficultyScaling speedScaling;
    [SerializeField]DifficultyScaling timeScaling;
    [SerializeField]DifficultyScaling decelerationScaling;
    [SerializeField]String tutorialText;
    [SerializeField]float speedWindow;
    [SerializeField]float timeToLoose;
    [SerializeField]Wheel wheel;
    
    [SerializeField]GameObject startCutscene;
    [SerializeField] private float cutsceneTime;
    [SerializeField] GameObject endCutscene;
    [SerializeField] private float endCutsceneTime;

    
    
    int currentLevel = 0;
    float decelerationRate;
    bool gameStarted = false;
    private float currentSpeed;
    private float previousSpeed;
    private float badTime;
    private float minSpeed;
    private float maxSpeed;
    
    [SerializeField] private SpriteRenderer merryGoround;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Sprite sprite3;
    [SerializeField] private Sprite sprite4;
    void Update()
    {
        if (!gameStarted)
        {
            return;
        }
        Debug.Log("Current  Speed "+currentSpeed);
        if (currentSpeed < minSpeed || currentSpeed>maxSpeed)
        {
            if (currentSpeed < minSpeed)
            {
                Debug.Log("Too Slow " + currentSpeed);
            }
            else
            {
                Debug.Log("Too Fast " + currentSpeed);
            }
            badTime+=Time.deltaTime;
        }

        if (badTime > timeToLoose)
        {
            OnGameLost?.Invoke();
            gameStarted = false;
            return;
        }
        if (Mathf.Approximately(previousSpeed, currentSpeed))
        {
            currentSpeed -= decelerationRate * Time.deltaTime;
        }
        else
        {
            previousSpeed = currentSpeed;
        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        float angle = wheel.GetAngle();
        if (angle > 90)
        {
            merryGoround.sprite = sprite1;
        }
        else if(angle < 180)
        {
            merryGoround.sprite = sprite2;
        }
        else if (angle < 270)
        {
            merryGoround.sprite = sprite3;
        }
        else if (angle < 360)
        {
            merryGoround.sprite = sprite4;
        }
    }

    public Action OnGameWon { get; set; }
    public Action OnGameLost { get; set; }
    public void StartMiniGame(int difficulty)
    {
        currentLevel = difficulty;
        float speedToReach = speedScaling.GetValue(difficulty);
        minSpeed = speedToReach - speedWindow;
        maxSpeed = speedToReach + speedWindow;
        decelerationRate = decelerationScaling.GetValue(difficulty);
        if (wheel)
        {
            wheel.onSpeedUpdate += OnWheelUpdate;
        }
        gameStarted = true;
    }

    private void OnWheelUpdate(float speed, bool isReverse)
    {
        currentSpeed = speed;
        Debug.Log("FFSpeed is"+currentSpeed);
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
       return false;
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
