
using System;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] Wheel wheel;
    [SerializeField] Transform minTransform;
    [SerializeField] Transform maxTransform;
    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float minSpeed = 10f;
    ShipMiniGame miniGame;
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (wheel)
        {
            wheel.onSpeedUpdate += OnWheelUpdate;
        }
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnWheelUpdate(float speed, bool isReverse)
    {
        if (speed < minSpeed)
        {
            speed = minSpeed;
        }
        if (speed > maxSpeed)
        {
            speed = maxSpeed;
        }
        speed = isReverse ? -speed : speed;
        //transform.Translate(Vector3.right * (speed * Time.deltaTime), Space.World);
        rb.MovePosition(transform.position + Vector3.right * (speed * Time.deltaTime));
        if (transform.position.x < minTransform.position.x)
        {
            transform.position = minTransform.position;
        }
        else if (transform.position.x > maxTransform.position.x)
        {
            transform.position = maxTransform.position;
        }
        
    }

    public void SetMiniGameManager(ShipMiniGame shipMiniGame)
    {
        miniGame = shipMiniGame;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.ObstacleTag))
        {
            miniGame.OnGameLost?.Invoke();
        }
    }
}
