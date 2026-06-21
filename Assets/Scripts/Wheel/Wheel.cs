using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Wheel : MonoBehaviour
{
    private bool isDragging = false;
    private Camera mainCamera;
    private float startAngleDegrees; 

    private Vector3 mousePosition;

    private float previousAngle;
    private float currentAngle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public Action<float,bool> onSpeedUpdate;

    private float speed;
    private bool isReverse;
    
    List<float> clockwiseAngles = new List<float>();
    List<float> counterClockwiseAngles = new List<float>();
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            previousAngle = currentAngle;
            UpdateWheelRotation();
            if (previousAngle > currentAngle)
            {
                isReverse = false;
            }
            else if(previousAngle < currentAngle)
            {
                isReverse = true;
            }
            else
            {
                return;
            }
            speed = Mathf.Abs(currentAngle - previousAngle);
            onSpeedUpdate?.Invoke(speed,isReverse);
        }
    }

    
    private void UpdateWheelRotation()
    {
       mousePosition = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
       var angleRad = Mathf.Atan2(mousePosition.y-transform.position.y, mousePosition.x-transform.position.x);
       var angleDegrees = angleRad * 180 / Mathf.PI;
       angleDegrees -= startAngleDegrees;
       transform.rotation = Quaternion.Euler(0, 0, angleDegrees);
       currentAngle = angleDegrees;
    }

    public void OnMouseDown()
    {
        isDragging = true;
        mousePosition = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var startAngle = Mathf.Atan2(mousePosition.y-transform.position.y, mousePosition.x-transform.position.x);
        startAngleDegrees = startAngle * 180 / Mathf.PI;
        startAngleDegrees -= transform.eulerAngles.z;
    }

    public void OnMouseUp()
    {
        isDragging = false;
    }
}
