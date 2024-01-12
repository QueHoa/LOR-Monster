[System.Serializable]
public class RangeFloatValue
{
    public float min, max;

    public RangeFloatValue(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float GetRandom()
    {
        return UnityEngine.Random.Range(min * 1f, max);
    }
    
}
