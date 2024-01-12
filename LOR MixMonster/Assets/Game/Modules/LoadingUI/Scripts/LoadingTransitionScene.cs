using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingTransitionScene : UnityEngine.MonoBehaviour
{
    void Start()
    {
        GetComponentInChildren<LevelLoading>(true).Init();
        GetComponentInChildren<FadeScreen>(true).Init();
    }

}
