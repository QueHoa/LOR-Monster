using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameController : MonoBehaviour
{
    protected CancellationTokenSource cancellation;


    public delegate void OnStageStart();
    public static OnStageStart onStageStart;
    public delegate void OnStageEnd();
    public static OnStageEnd onStageEnd;
    public bool isReady = false;
    public bool hideMonster, updateGold = false;
    public bool isDown = false, isSelected = false;

    protected void Start()
    {
        Debug.Log("STARTED: " + gameObject.name);
        Game.Controller.Instance.OnGameLoaded(this);
    }
    public virtual void Destroy() { }
    public virtual void Clear() { }
    public virtual async UniTask InitializeAsync() { }
    public virtual async UniTask SetUp() { }
    public virtual async UniTask SetUpCollection() { }
}
