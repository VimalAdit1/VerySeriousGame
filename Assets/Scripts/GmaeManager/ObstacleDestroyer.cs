using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObstacleDestroyer : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.ObstacleTag))
        {
            Object.Destroy(other.gameObject);
        }
    }
}
