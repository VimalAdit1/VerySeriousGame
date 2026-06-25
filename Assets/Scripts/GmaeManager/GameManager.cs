using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] List<Transform> miniGamesToSpawn;
    [SerializeField] Canvas canvas;
    [SerializeField] GameUI gameUI;
    [SerializeField] int totalLives;
    [SerializeField] int levelsTolevelUp;
    [SerializeField] int maxLevels;
    
    int currentMinigameIndex;
    MiniGame currentMiniGame;
    Transform miniGameToSpawn;
    GameObject miniGameObj;
    int levelsCompleted;
    
    int livesLeft;
    
    int currentLevel;
    private bool isStoryMode;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        livesLeft = totalLives;
        if (PlayerPrefs.GetInt(Constants.IsStoryCompleteKey, 0) == 0)
        {
            StartStoryMode();
        }
        else
        {
            SpawnRandomGame();
        }

        gameUI.UpdateLivesLeft(livesLeft);
    }

    private void StartStoryMode()
    {
        isStoryMode = true;
        currentMinigameIndex = 0;
        SpawnMiniGame();
    }


    internal void StartMiniGame()
    {
        currentMiniGame.StartMiniGame(currentLevel);
        currentMiniGame.OnGameWon+=OnGameWon;
        currentMiniGame.OnGameLost+=OnGameLost;
    }

    
    private void SpawnRandomGame()
    {
        int randomIndex = UnityEngine.Random.Range(0, miniGamesToSpawn.Count);
        if (randomIndex == currentMinigameIndex)
        {
            randomIndex++;
            randomIndex %= miniGamesToSpawn.Count;
        }
        currentMinigameIndex = randomIndex;
        SpawnMiniGame();
    }
    void SpawnMiniGame()
    {
        if(miniGameToSpawn != null)
            GameObject.Destroy(miniGameObj);
        miniGameToSpawn = miniGamesToSpawn[currentMinigameIndex];
        miniGameObj = GameObject.Instantiate(miniGameToSpawn, transform.position, Quaternion.identity).gameObject;
        miniGameObj.transform.position = Vector3.zero;
        currentMiniGame = miniGameObj.GetComponent<MiniGame>();
        Time.timeScale = 1;
        if (isStoryMode)
        {
            StartCoroutine(currentMiniGame.PlayStartCutscene());
        }
        else
        {
            PrepareMiniGame();
        }
    }

    internal void PrepareMiniGame()
    {
        float timer = currentMiniGame.GetMiniGameTime(currentLevel);
        gameUI.Reset();
        gameUI.HideAllUI();
        gameUI.ShowPreGameScreen(currentMiniGame.GetMiniGameTutorial());
        gameUI.SetTimer(timer,currentMiniGame.IsGameOverOnTimeEnd());
    }

    internal void OnGameWon()
    {
        currentMiniGame.StopMiniGame();
        if (isStoryMode)
        {
            StartCoroutine(currentMiniGame.PlayEndCutscene());
        }
        else
        {
            ShowWinScreen();
        }
    }

    internal void ShowWinScreen()
    {
        gameUI.ToggleWinScreen(true);
        gameUI.ToggleRetryScreen(false);
    }

    internal void OnGameLost()
    {
        if (isStoryMode)
        {
            currentMiniGame.StopMiniGame();
            gameUI.ToggleRetryScreen(true);
        }
        else
        {
            livesLeft--;
            gameUI.UpdateLivesLeft(livesLeft);
            if (livesLeft == 0)
            {
                gameUI.ToggleGameOverScreen(true);
            }
            else
            {
                gameUI.ToggleLifeLostScreen(true);
            }
        }
    }
    
    internal void RetryMiniGame()
    {
       GameObject.Destroy(miniGameObj);
       SpawnMiniGame();
    }
    internal void OnCutsceneEnd()
    {
        ShowWinScreen();
    }

    internal void Restart()
    {
        currentLevel = 0;
        levelsCompleted = 0;
        livesLeft=totalLives;
        gameUI.UpdateLivesLeft(livesLeft);
        SpawnRandomGame();
    }

    internal void NextLevel()
    {
        if (isStoryMode)
        {
            currentMinigameIndex++;
            if (currentMinigameIndex >= miniGamesToSpawn.Count)
            {
                isStoryMode = false;
                currentMinigameIndex = 0;
                SpawnRandomGame();
            }
            else
            {
                SpawnMiniGame();
            }
        }
        else
        {
            levelsCompleted++;
            if (levelsCompleted >= levelsTolevelUp)
            {
                currentLevel++;
                currentLevel = Mathf.Min(currentLevel, maxLevels);
                levelsCompleted = 0;
            }
            SpawnMiniGame();
        }
    }

    internal Canvas GetCanvas()
    {
        return canvas;
    }

}
