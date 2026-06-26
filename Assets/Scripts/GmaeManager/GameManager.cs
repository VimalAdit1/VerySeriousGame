using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private string message;
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
        StopAllAudios();
        gameUI.SetLevelsCompleted(levelsCompleted);
        gameUI.ToggleHearts(!isStoryMode);
        
        if (miniGameToSpawn != null)
            GameObject.Destroy(miniGameObj);
        miniGameToSpawn = miniGamesToSpawn[currentMinigameIndex];
        miniGameObj = GameObject.Instantiate(miniGameToSpawn, transform.position, Quaternion.identity).gameObject;
        miniGameObj.transform.position = Vector3.zero;
        currentMiniGame = miniGameObj.GetComponent<MiniGame>();
        Time.timeScale = 1;
        AudioClip miniGameAmb = currentMiniGame.GetMiniGameAmb();
        AudioManager.instance.PlayAmbience(miniGameAmb);
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
        if (message != null)
        {
            gameUI.ShowPreGameScreen(message+ currentMiniGame.GetMiniGameTutorial());
            message = null;
        }
        else
        {
            gameUI.ShowPreGameScreen(message+ currentMiniGame.GetMiniGameTutorial());
        }
        gameUI.SetTimer(timer,currentMiniGame.IsGameOverOnTimeEnd());
    }

    internal void OnGameWon()
    {
        currentMiniGame.StopMiniGame();

        // Audio
        AudioClip winAmb = currentMiniGame.GetGameWonAmb();
        AudioClip winSFX = currentMiniGame.GetGameWonSFX();
        AudioManager.instance.StopAmbience();
        AudioManager.instance.PlayAmbience(winAmb);
        AudioManager.instance.PlaySFX(winSFX);
        Sprite spriteToShow = currentMiniGame.GetEndScreenSprite(true);
        if (isStoryMode)
        {
            StartCoroutine(currentMiniGame.PlayEndCutscene());
        }
        else
        {
            ShowWinScreen(spriteToShow);
        }
    }

    internal void ShowWinScreen(Sprite spriteToShow)
    {
        gameUI.ToggleWinScreen(true);
        gameUI.ToggleRetryScreen(false);
        gameUI.SetWinScreenSprite(spriteToShow);
    }
    
    internal void OnGameLost()
    {
        
        Sprite spriteToShow = currentMiniGame.GetEndScreenSprite(false);
        gameUI.Reset();
        gameUI.SetLooseScreenSprite(spriteToShow);
        currentMiniGame.StopMiniGame();
        if (isStoryMode)
        {
            // Audio
            AudioClip lostAmb = currentMiniGame.GetGameLostAmb();
            AudioClip lostSFX = currentMiniGame.GetGameLostSFX();
            AudioManager.instance.StopAmbience();
            AudioManager.instance.PlayAmbience(lostAmb);
            AudioManager.instance.PlaySFX(lostSFX);

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
       gameUI.HideAllUI();
       SpawnMiniGame();
    }
    internal void OnCutsceneEnd()
    {
        Sprite spriteToShow = currentMiniGame.GetEndScreenSprite(true);
        gameUI.HideAllUI();
        ShowWinScreen(spriteToShow);
    }

    internal void Restart()
    {
        SceneManager.LoadScene(Constants.mainMenuScene);
        /*currentLevel = 0;
        levelsCompleted = 0;
        livesLeft=totalLives;
        gameUI.UpdateLivesLeft(livesLeft);
        gameUI.HideAllUI();
        SpawnRandomGame();*/
    }

    internal void NextLevel()
    {
        gameUI.HideAllUI();
        if (isStoryMode)
        {
            
            currentMinigameIndex++;
            if (currentMinigameIndex >= miniGamesToSpawn.Count)
            {
                isStoryMode = false;
                currentMinigameIndex = 0;
                PlayerPrefs.SetInt(Constants.IsStoryCompleteKey, 1);
                message = "You completed the game! Endless mode now!!!  ";
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
            SpawnRandomGame();
        }
    }

    internal Canvas GetCanvas()
    {
        return canvas;
    }

    public void StopAllAudios()
    {
        AudioManager.instance.StopAmbience();
        AudioManager.instance.StopMusic();
    }

    public GameUI GetGameUI()
    {
        return gameUI;
    }
}
