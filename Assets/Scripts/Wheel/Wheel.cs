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
    private Vector3 localPosition;
    
    [SerializeField]private Transform shadowTransform;

    [Space(5), Header("Wheel Audio")]
    public AudioClip wheelTurnAudio;

    private float speed;
    private bool isReverse;
    
    List<float> clockwiseAngles = new List<float>();
    List<float> counterClockwiseAngles = new List<float>();
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDragging)
        {
            AudioManager.instance.PlaySFX(wheelTurnAudio);
            previousAngle = currentAngle;
            UpdateWheelRotation();
            SnapMouse();
            if (previousAngle > currentAngle)
            {
                isReverse = false;
            }
            else if(previousAngle < currentAngle)
            {
                isReverse = true;
            }
            speed = Mathf.Abs(CalculateSpeedFromAngles());

            onSpeedUpdate?.Invoke(speed,isReverse);
        }

        if (shadowTransform)
        {
            shadowTransform.rotation = transform.rotation;
        }
    }

    private void SnapMouse()
    {
        if (Mouse.current.delta.ReadValue().sqrMagnitude > 0.1f)
        {
            Vector3 worldPos = transform.TransformPoint(localPosition);
            Mouse.current.WarpCursorPosition(mainCamera.WorldToScreenPoint(worldPos));
        }
        
    }

    private float CalculateSpeedFromAngles()
    {
        float angleDifference = Normalize(currentAngle) - Normalize(previousAngle);
        angleDifference = Mathf.Abs(angleDifference);
        angleDifference = (angleDifference+180) % 360 - 180;
        return angleDifference;
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
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 1); 
        
        Gizmos.DrawSphere(transform.TransformPoint(localPosition), 0.1f);

    }

    public void OnMouseDown()
    {
        isDragging = true;
        mousePosition = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
        localPosition = transform.InverseTransformPoint(mousePosition);
        var startAngle = Mathf.Atan2(mousePosition.y-transform.position.y, mousePosition.x-transform.position.x);
        startAngleDegrees = startAngle * 180 / Mathf.PI;
        startAngleDegrees -= transform.eulerAngles.z;
    }

    public void OnMouseUp()
    {
        isDragging = false;
    }
    public float Normalize(float angle)
    {
        return ((angle % 360f) + 360f) % 360f;
    }

    public float GetAngle()
    {
        return Normalize(currentAngle);
    }
}
