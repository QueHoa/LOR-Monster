using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Pool
{
    [System.Serializable]
    public class ObjectCollection
    {
        public List<PoolObject> pool = new List<PoolObject>();
        public List<PoolObject> inUsePool = new List<PoolObject>();
        public AsyncOperationHandle<Object> loadRequest;
        bool isReady = false;
        bool isLoading = false;
        public ObjectCollection(string objectId)
        {
            loadRequest = Addressables.LoadAssetAsync<Object>(objectId);
        }

        public async UniTask Load()
        {
            if (isLoading)
            {
                await UniTask.WaitUntil(() => !isLoading);
                return;
            }
            isLoading = true;
            await loadRequest.Task;
            isReady = true;
            isLoading = false;
        }

        public void Add(PoolObject obj)
        {
            pool.Add(obj);
            obj.onReleased = Remove;
        }
        public void Remove(PoolObject obj)
        {
            pool.Add(obj);
            inUsePool.Remove(obj);
        }
        public PoolObject Get()
        {
            if (pool.Count == 0)
            {
                GameObject obj = ((GameObject)GameObject.Instantiate(loadRequest.Result));
                obj.name = loadRequest.Result.name + "_" + (pool.Count + inUsePool.Count);
                Add(obj.AddComponent<PoolObject>());
            }
            PoolObject readyObj = pool[0];
            readyObj.IsAvailable = false;

            pool.Remove(readyObj);
            inUsePool.Add(readyObj);

            return readyObj;
        }

        public void ClearAll()
        {
            foreach (PoolObject PoolObject in inUsePool)
            {
                PoolObject.gameObject.SetActive(false);
            }
        }
    }

}