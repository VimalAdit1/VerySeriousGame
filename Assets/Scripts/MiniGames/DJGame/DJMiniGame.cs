using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DJMiniGame : MonoBehaviour,MiniGame
{
    [SerializeField]List<Transform> spawnPoints;
    [SerializeField]List<Sprite> noteSprite;
    [SerializeField]String tutorialText;
    [SerializeField]DifficultyScaling timeScaling;
    [SerializeField]DifficultyScaling coolDownScaling;
    [SerializeField]DifficultyScaling spawnRateScaling;
    [SerializeField]DifficultyScaling speedScaling;
    [SerializeField]MusicNote musicNotePrefab;
    [SerializeField]int musicNotesToMiss;
    
    [SerializeField]GameObject startCutscene;
    [SerializeField] private float cutsceneTime;
    [SerializeField] GameObject endCutscene;
    [SerializeField] private float endCutsceneTime;
    
    int currentLevel = 0;
    private int notesToSpawn;
    private float waitTime;
    private float noteSpeed;
    private float coolDown;
    int musicNotesMissed;
    
    private Coroutine currentGame;
    public Action OnGameWon { get; set; }
    public Action OnGameLost { get; set; }
    public void StartMiniGame(int difficulty)
    {
        currentLevel = difficulty;
        notesToSpawn = Mathf.RoundToInt(spawnRateScaling.GetValue(currentLevel));
        waitTime = timeScaling.GetValue(currentLevel);
        noteSpeed = speedScaling.GetValue(currentLevel);
        coolDown = coolDownScaling.GetValue(currentLevel);
        currentGame = StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        WaitForSeconds wait = new WaitForSeconds(coolDown);
        while (true)
        {
            for (int i = 0; i < notesToSpawn; i++)
            {
                int randomPosition = Random.Range(0, spawnPoints.Count);
                MusicNote note = Instantiate(musicNotePrefab, spawnPoints[randomPosition].position, Quaternion.identity)
                    .GetComponent<MusicNote>();
                note.transform.SetParent(this.transform);
                note.Initialize(noteSpeed,noteSprite[randomPosition],this);
                yield return wait;
            }
            yield return wait;
            yield return wait;
        }
    }

    public void StopMiniGame()
    {
        Time.timeScale = 0;
        if (currentGame != null)
        {
            StopCoroutine(currentGame);
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
        Time.timeScale = 1;
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

    public void MusicNoteCollected()
    {
        //Crowd Goes Happy
    }

    public void MusiNoteMissed()
    {
        musicNotesMissed++;
        if (musicNotesMissed >= musicNotesToMiss)
        {
            OnGameLost?.Invoke();
            StopMiniGame();
        }
    }
}
