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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    bool acceptInput = false;
    Coroutine timerCoroutine;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (acceptInput)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HidePreGameScreen();
                acceptInput = false;
                GameManager.instance.StartMiniGame();
            }
        }
    }

    
    internal void HideAllUI()
    {
        HidePreGameScreen();
        ToggleGameOverScreen(false);
        ToggleWinScreen(false);
    }

    internal void HidePreGameScreen()
    {
        preGameScreen.SetActive(false);
    }

    internal void ToggleGameOverScreen(bool value)
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
            GameManager.instance.GameLost();
        }
        else
        {
            GameManager.instance.GameWon();
        }
    }

    public void RetryLevel()
    {
        GameManager.instance.RetryMiniGame();
    }

    public void Reset()
    {
       HideAllUI();
       if (timerCoroutine != null)
       {
           StopCoroutine(timerCoroutine);
       }
    }
}
