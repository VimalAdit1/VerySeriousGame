using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipMiniGame : MonoBehaviour, MiniGame
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]List<Transform> spawnPoints;
    [SerializeField]List<IceBerg> icebergPrefabs;
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
    
    [SerializeField] Sprite winSprite;
    [SerializeField] Sprite looseSprite;

    [Space(5), Header("Amb Clips")]
    public List<AudioClip> miniGameAmbience;
    public List<AudioClip> winAmbience;
    public List<AudioClip> loseAmbience;

    [Space(1)]
    public List<AudioClip> winSFX;
    public List<AudioClip> loseSFX;
    
    

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
                IceBerg icebergPrefab = icebergPrefabs[Random.Range(0, icebergPrefabs.Count)];
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
