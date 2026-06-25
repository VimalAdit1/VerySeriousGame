
using System;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] Wheel wheel;
    [SerializeField] Transform minTransform;
    [SerializeField] Transform maxTransform;
    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float minSpeed = 10f;
    
    [SerializeField] SpriteRenderer shipSpriteRenderer;
    [SerializeField] Sprite leftSprite;
    [SerializeField] Sprite rightSprite;
    [SerializeField] Sprite defaultSprite;
    ShipMiniGame miniGame;
    Rigidbody2D rb;
    
    bool isSteering;
    bool isMovingRight;

    public List<AudioClip> shipCrashSFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (wheel)
        {
            wheel.onSpeedUpdate += OnWheelUpdate;
        }
        rb = GetComponent<Rigidbody2D>();
    }

    private void UpdateSprite()
    {
        if (!isSteering)
        {
            shipSpriteRenderer.sprite = defaultSprite;
        }
        else
        {
            if (isMovingRight)
            {
                shipSpriteRenderer.sprite = rightSprite;
            }
            else
            {
                shipSpriteRenderer.sprite = leftSprite;
            }
        }
    }

    void OnWheelUpdate(float speed, bool isReverse)
    {
        if (speed < minSpeed && speed>0)
        {
            speed = minSpeed;
        }
        if (speed > maxSpeed)
        {
            speed = maxSpeed;
        }
        isMovingRight = !isReverse;
        isSteering = speed!=0;
        speed = isReverse ? -speed : speed;
        transform.Translate(Vector3.right * (speed * Time.deltaTime));
        //rb.MovePosition(transform.position + Vector3.right * (speed * Time.deltaTime));
        if (transform.position.x < minTransform.position.x)
        {
            transform.position = minTransform.position;
        }
        else if (transform.position.x > maxTransform.position.x)
        {
            transform.position = maxTransform.position;
        }
        UpdateSprite();
        
    }

    public void SetMiniGameManager(ShipMiniGame shipMiniGame)
    {
        miniGame = shipMiniGame;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.ObstacleTag))
        {
            // Chooses one of the random Audios of Ship Crashing and plays it
            AudioClip randomClip = shipCrashSFX[UnityEngine.Random.Range(0, shipCrashSFX.Count)];
            AudioManager.instance.PlaySFX(randomClip);

            miniGame.OnGameLost?.Invoke();
        }
    }
}
