using System.Collections.Generic;
using UnityEngine;

public class PlayHead : MonoBehaviour
{
    [SerializeField] Wheel wheel;
    [SerializeField] private List<Transform> lanePoints;
    [SerializeField] private float speedToSwitch;
    DJMiniGame miniGame;
    int currentLane = 0;
    private float accumulatedSpeed;
    void Start()
    {
        MoveToLane(0);
        if (wheel)
        {
            wheel.onSpeedUpdate += OnWheelUpdate;
        }
    }

    void Update()
    {
        if (accumulatedSpeed > 0)
        {
            accumulatedSpeed -= Time.deltaTime * 10;
        }
        else if (accumulatedSpeed < 0)
        {
            accumulatedSpeed += Time.deltaTime * 10;
        }
    }

    private void OnWheelUpdate(float speed, bool isReverse)
    {
        speed = isReverse ? -speed : speed;
        accumulatedSpeed += speed;
        if (accumulatedSpeed >= speedToSwitch)
        {
            currentLane++;
            if (currentLane >= lanePoints.Count)
            {
                currentLane = lanePoints.Count-1;
            }
            MoveToLane(currentLane);
        }
        else if (Mathf.Abs(accumulatedSpeed) >= speedToSwitch)
        {
            currentLane--;
            if (currentLane <0)
            {
                currentLane = 0;
            }
            MoveToLane(currentLane);
        }
    }

    private void MoveToLane(int i)
    {
        accumulatedSpeed = 0;
        if (i >= lanePoints.Count || i < 0)
        {
            i = 0;
        }
        transform.position = new Vector3(transform.position.x, lanePoints[i].position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.MusicNoteTag))
        {
            //Play Audio
            //SpawnFX
            Destroy(other.gameObject);
            //miniGame.MusicNoteCollected();
        }
    }
}
