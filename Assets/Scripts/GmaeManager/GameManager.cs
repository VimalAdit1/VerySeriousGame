using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] Transform miniGameToSpawn;
    MiniGame currentMiniGame;
    GameObject miniGameObj;
    [SerializeField] GameUI gameUI;
    int currentLevel;
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


    internal void StartMiniGame()
    {
        currentMiniGame.StartMiniGame(currentLevel);
        currentMiniGame.OnGameWon+=OnGameWon;
        currentMiniGame.OnGameLost+=OnGameLost;
    }

    void SpawnMiniGame()
    {
        miniGameObj = GameObject.Instantiate(miniGameToSpawn, transform.position, Quaternion.identity).gameObject;
        currentMiniGame = miniGameObj.GetComponent<MiniGame>();
        float timer = currentMiniGame.GetMiniGameTime(currentLevel);
        Time.timeScale = 1;
        //StartMiniGame();
        gameUI.Reset();
        gameUI.HideAllUI();
        gameUI.ShowPreGameScreen(currentMiniGame.GetMiniGameTutorial());
        gameUI.SetTimer(timer,currentMiniGame.IsGameOverOnTimeEnd());
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnMiniGame();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGameWon()
    {
        currentMiniGame.StopMiniGame();
        gameUI.ToggleWinScreen(true);
        gameUI.ToggleGameOverScreen(false);
    }

    void OnGameLost()
    {
        currentMiniGame.StopMiniGame();
        gameUI.ToggleGameOverScreen(true);
    }

    internal void GameWon()
    {
        OnGameWon();
    }

    public void RetryMiniGame()
    {
       GameObject.Destroy(miniGameObj);
       SpawnMiniGame();
    }

    internal void GameLost()
    {
        OnGameLost();
    }
}
