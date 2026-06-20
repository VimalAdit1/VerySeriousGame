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
                Debug.Log("Rotating backward");
            }
            else if(previousAngle < currentAngle)
            {
                Debug.Log("Rotating forward");
            }
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
