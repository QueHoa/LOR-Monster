using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPooler : MonoBehaviour
{
    public static CoinPooler instance;

    public RectTransform[] pooledCoin;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public RectTransform[] GetPoolCoin()
    {
        return pooledCoin;
    }
}
