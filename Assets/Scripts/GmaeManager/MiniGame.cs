using System;
using UnityEngine;

public interface MiniGame
{
 public Action OnGameWon { get; set;}
 public Action OnGameLost { get;set;}
 public void StartMiniGame(int difficulty);
 
 public void StopMiniGame();
 public string GetMiniGameTutorial();
 public float GetMiniGameTime(int difficulty);
 
 public bool IsGameOverOnTimeEnd();
}
