using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
   


    public delegate void OnStageStart();
    public static OnStageStart onStageStart;
    public delegate void OnStageEnd();
    public static OnStageEnd onStageEnd;
    public bool isReady = false;

    public virtual void Destroy() { }
    public virtual void Clear() { }
    public virtual async UniTask InitializeAsync() { }
    public virtual async UniTask SetUp() { }
    public virtual async UniTask SetUpCollection() { }
}
