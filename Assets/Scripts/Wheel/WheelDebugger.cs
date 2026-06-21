using TMPro;
using UnityEngine;

public class WheelDebugger : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI debugText;
    [SerializeField] bool debug;
    [SerializeField] Wheel wheelToDebug;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (debug && wheelToDebug != null)
        {
            wheelToDebug.onSpeedUpdate += OnWheelUpdate;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnWheelUpdate(float speed, bool isReverse)
    {
        if (debugText)
        {
            speed = Mathf.Round(speed);
            debugText.SetText("Current Speed "+speed.ToString()+"Rotation direction"+isReverse.ToString());
            Debug.Log(debugText.text);
        }
    }
}
