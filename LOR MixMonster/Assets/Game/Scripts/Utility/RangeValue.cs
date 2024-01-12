[System.Serializable]
public class RangeValue
{
    public int min, max;

    public RangeValue(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    public RangeValue()
    {
    }

    public float GetRandom()
    {
        return UnityEngine.Random.Range(min * 1f, max);
    }
    public int GetRandomInt()
    {
        return UnityEngine.Random.Range(min, max+1);
    }
}
