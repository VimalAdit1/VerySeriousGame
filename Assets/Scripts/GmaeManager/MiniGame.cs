using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
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
 
 public IEnumerator PlayStartCutscene();
 public IEnumerator PlayEndCutscene();

    // Audio Events
    public AudioClip GetMiniGameAmb();
    public AudioClip GetGameWonAmb();
    public AudioClip GetGameWonSFX();
    public AudioClip GetGameLostAmb();
    public AudioClip GetGameLostSFX();
}
