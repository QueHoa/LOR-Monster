using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpawnerBase<T> : MonoBehaviour
{
    [SerializeField]
    protected AssetReferenceGameObject[] prefabRefs;
    protected List<List<T>> stack ;
    protected AsyncOperationHandle<GameObject>[] ops;
    protected int[] status;
    [SerializeField]
    protected AssetLabelReference label;
    protected CancellationTokenSource cancellation;
    public void Init()
    {
        if (ops == null)
        {
            ops = new AsyncOperationHandle<GameObject>[prefabRefs.Length];
            status = new int[prefabRefs.Length];
            stack = new List<List<T>>();
            for (int j = 0; j < prefabRefs.Length; j++)
            {
                stack.Add(new List<T>());
            }
        }
    }


    private void OnEnable()
    {
        cancellation = new CancellationTokenSource();
    }
    private void OnDisable()
    {
        if (cancellation != null)
        {
            cancellation.Cancel();
        }
    }
    public virtual async UniTask<T> GetAsync(int index)
    {
        Init();
        if (status[index] != 0)
        {
            await UniTask.WaitUntil(() => (status[index] == 2),cancellationToken:cancellation.Token);
        }
        if (!ops[index].IsValid() && status[index] == 0)
        {
            status[index] = 1;
            ops[index] = Addressables.LoadAssetAsync<GameObject>(prefabRefs[index]);
            await ops[index];
            status[index] = 2;

        }
        for (int i = 0; i < stack[index].Count; i++)
        {
            if (!IsActive(stack[index][i]))
            {
                OnGetCompleted(stack[index][i]);
                return stack[index][i];
            }
        }

        GameObject o = Instantiate(ops[index].Result, transform);
        o.gameObject.name = ops[index].Result.gameObject.name;
        T t = o.GetComponent<T>();
        stack[index].Add(t);
        o.SetActive(false);
        OnGetCompleted(t);

        return t;

    }
    public virtual void OnGetCompleted(T item)
    {

    }
    public async UniTaskVoid Get(int index, System.Action<T> result)
    {
        T res = await GetAsync(index);
        result?.Invoke(res);
    }
    protected virtual void OnDestroy()
    {
        DestroyAll();
        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
        }
    }
    public virtual void ClearAll()
    {
        if (ops == null) return;

        for (int i = 0; stack != null && i < stack.Count; i++)
        {
            for (int j = 0; j < stack[i].Count; j++)
            {
                try
                {
                    SetActive(stack[i][j], false);
                }catch(System.Exception e)
                {
                    GameUtility.GameUtility.LogError(e);
                }
            }
        }
        //foreach(AsyncOperationHandle op in ops)
        //{
        //    if(op.IsValid())
        //    Addressables.Release(op);
        //}
    }

    public void DestroyAll()
    {
        if (ops != null)
        {
            foreach (AsyncOperationHandle op in ops)
            {
                if (op.IsValid())
                    Addressables.Release(op);
            }
            ops = null;
        }
        if (stack != null)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                for (int j = 0; j < stack[i].Count; j++)
                {
                    DestroySpawn(stack[i][j]);
                    stack[i][j] = default;
                }
            }
            stack.Clear();
            stack = null;
        }
    }
    public virtual void DestroySpawn(T obj)
    {

    }
    public virtual bool IsActive(T t) { return false; }
    public virtual void SetActive(T t, bool active) { }

}
