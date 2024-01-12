using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectSpawner : SpawnerBase<GameObject>
{
    public static ObjectSpawner Instance;
    void Start()
    {
        Instance = this;
    }
    public override async UniTask<GameObject> GetAsync(int index)
    {
        Init();
        if (status[index] != 0)
        {
            await UniTask.WaitUntil(() => (status[index] == 2));
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
                return stack[index][i];
            }
        }

        GameObject o = Instantiate(ops[index].Result, transform);

        stack[index].Add(o);
        o.SetActive(false);
        return o;

    }


    public override void SetActive(GameObject t, bool active)
    {
        t.SetActive(active);
    }
    public override bool IsActive(GameObject t)
    {
        return t.activeSelf;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void DestroySpawn(GameObject obj)
    {
        Destroy(obj);
    }
}
