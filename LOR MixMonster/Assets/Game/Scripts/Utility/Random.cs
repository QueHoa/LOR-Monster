namespace GameUtility
{
    public static class Random
    {
        public static float Range(RangeValue rangeValue)
        {
            return UnityEngine.Random.Range(rangeValue.min, rangeValue.max + 1);
        }
       
    }
}