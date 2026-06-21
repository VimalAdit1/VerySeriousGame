using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyScaling", menuName = "Scriptable Objects/DifficultyScaling")]
public class DifficultyScaling : ScriptableObject
{
  [SerializeField]private float minValue;
  [SerializeField]private float maxValue;
  [SerializeField]private float[] customValues;
  [SerializeField]private bool flipMinAndMax;
  public float GetValue(int level)
  {
    if (customValues.Length > level && level >= 0)
    {
      float value = customValues[level];
      if (value < minValue)
      {
        value = minValue;
      }
      else if (value > maxValue)
      {
        value = maxValue;
      }
      return value;
    }
    else if (customValues.Length < level)
    {
      return flipMinAndMax?minValue:maxValue;
    }
    else
    {
      return flipMinAndMax?maxValue:minValue;
    }
  }
}
