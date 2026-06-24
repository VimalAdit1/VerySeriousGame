using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipMiniGame : MonoBehaviour,MiniGame
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]List<Transform> spawnPoints;
    [SerializeField]IceBerg icebergPrefab;
    [SerializeField]DifficultyScaling timeScaling;
    [SerializeField]DifficultyScaling coolDownScaling;
    [SerializeField]DifficultyScaling spawnRateScaling;
    [SerializeField]DifficultyScaling speedScaling;
    [SerializeField]String tutorialText;
    
    [SerializeField] private Ship ship;
    
    [SerializeField]GameObject startCutscene;
    [SerializeField] private float cutsceneTime;
    [SerializeField] GameObject endCutscene;
    [SerializeField] private float endCutsceneTime;
    
    int currentLevel = 0;
    private int icebergsToSpawn;
    private float waitTime;
    private float icebergSpeed;
    private float coolDown;

    private Coroutine currentGame;

    
    public Action OnGameWon { get; set;}
    public Action OnGameLost { get;set;}

    public void StartMiniGame(int difficulty)
    {
        currentLevel = difficulty;
        icebergsToSpawn = Mathf.RoundToInt(spawnRateScaling.GetValue(currentLevel));
        waitTime = timeScaling.GetValue(currentLevel);
        icebergSpeed = speedScaling.GetValue(currentLevel);
        coolDown = coolDownScaling.GetValue(currentLevel);
        ship.SetMiniGameManager(this);
        currentGame =  StartCoroutine(StartGame());
    }

    public void StopMiniGame()
    {
        Time.timeScale = 0;
        if (currentGame != null)
        {
            StopCoroutine(currentGame);
        }
    }

    IEnumerator StartGame()
    {
        WaitForSeconds wait = new WaitForSeconds(coolDown);
        while (true)
        {
            for (int i = 0; i < icebergsToSpawn; i++)
            {
                int randomPosition = Random.Range(0, spawnPoints.Count);
                IceBerg iceBerg = Instantiate(icebergPrefab, spawnPoints[randomPosition].position, Quaternion.identity)
                    .GetComponent<IceBerg>();
                iceBerg.transform.SetParent(this.transform);
                iceBerg.SetSpeed(icebergSpeed);
                yield return wait;
            }
            yield return wait;
            yield return wait;
        }
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
