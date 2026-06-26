using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject preGameScreen;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject gameLostScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private GameObject lifeLostScreen;
    [SerializeField] private TextMeshProUGUI lifeLostText;
    [SerializeField] private Image[] livesImage;
    [SerializeField] private Image[] winScreenSprites;
    [SerializeField] private Image[] loseScreenSprites;
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private GameObject countDownUI;
    [SerializeField] private GameObject heartsUI;
    [SerializeField] private GameObject levelsUI;
    [SerializeField] private TextMeshProUGUI levelsText;
    private int livesLeft;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    bool acceptInput = false;
    Coroutine timerCoroutine;
    void Start()
    {
        countDownUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!acceptInput) return;
        if (Input.GetMouseButtonDown(0))
        {
            HidePreGameScreen();
            acceptInput = false;
            GameManager.instance.StartMiniGame();
        }
    }

    
    internal void HideAllUI()
    {
        HidePreGameScreen();
        ToggleRetryScreen(false);
        ToggleWinScreen(false);
        ToggleGameOverScreen(false);
        ToggleLifeLostScreen(false);
        countDownUI.SetActive(false);
    }

    internal void HidePreGameScreen()
    {
        preGameScreen.SetActive(false);
    }

    internal void ToggleRetryScreen(bool value)
    {
        gameLostScreen.SetActive(value);
    }
    internal void ShowPreGameScreen(string message)
    {
        preGameScreen.SetActive(true);
        tutorialText.text = message;
        acceptInput = true;
    }

    internal void ToggleWinScreen(bool value)
    {
        winScreen.SetActive(value);
    }

    internal void SetTimer(float timer,bool isGameOverOnTimeEnd)
    {
        timerCoroutine = StartCoroutine(CountDownTimer(timer,isGameOverOnTimeEnd));
    }

    private IEnumerator CountDownTimer(float timer,bool isGameOverOnTimeEnd)
    {
        float elapsedTime = 0f;
        
        yield return new WaitUntil(() => acceptInput == false);
        while (elapsedTime <= timer)
        {
            elapsedTime += Time.deltaTime;
            fillImage.fillAmount = 1 - (elapsedTime / timer);
            yield return null;
        }

        if (isGameOverOnTimeEnd)
        {
            GameManager.instance.OnGameLost();
        }
        else
        {
            GameManager.instance.OnGameWon();
        }
    }

    public void RetryLevel()
    {
        GameManager.instance.RetryMiniGame();
    }

    public void NextLevel()
    {
        GameManager.instance.NextLevel();
    }
    public void RestartLevel()
    {
        GameManager.instance.Restart();
    }

    public void Reset()
    {
       HideAllUI();
       if (timerCoroutine != null)
       {
           StopCoroutine(timerCoroutine);
       }
    }

    public void UpdateLivesLeft(int value)
    {
        livesLeft = value;
        foreach (var image in livesImage)
        {
            image.enabled = false;
        }
        for (int i = 0; i < livesLeft; i++)
        {
            livesImage[i].enabled = true;
        }
    }

    public void ToggleGameOverScreen(bool value)
    {
        gameOverScreen.SetActive(value);
        gameOverText.SetText("You completed "+levelsText.text+" levels ");
    }

    public void ToggleLifeLostScreen(bool value)
    {
        lifeLostScreen.SetActive(value);
        if (livesLeft > 1)
        {
            lifeLostText.SetText("You have "+livesLeft+" chances left.");
        }
        else
        {
            lifeLostText.SetText("You have only one chance left.");
        }
    }

    public void SetWinScreenSprite(Sprite spriteToShow)
    {
        foreach (var sprite in winScreenSprites)
        {
            sprite.sprite = spriteToShow;
        }
    }

    public void SetLooseScreenSprite(Sprite spriteToShow)
    {
        foreach (var sprite in loseScreenSprites)
        {
            sprite.sprite = spriteToShow;
        }
    }

    public IEnumerator StartCountDown()
    {
        countDownUI.SetActive(true);
        Time.timeScale = 0f;
        int countDown = 3;
        while (countDown>0)
        {
            countDownText.SetText(countDown.ToString());
            countDown--;
            yield return new WaitForSecondsRealtime(1);
        }
        countDownText.SetText("Go");
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1f;
        countDownUI.SetActive(false);
    }

    public void ToggleHearts(bool value)
    {
        heartsUI.SetActive(value);
        levelsUI.SetActive(value);
    }

    public void SetLevelsCompleted(int levelsCompleted)
    {
        levelsCompleted++;
        levelsText.SetText(levelsCompleted.ToString());
    }
}
