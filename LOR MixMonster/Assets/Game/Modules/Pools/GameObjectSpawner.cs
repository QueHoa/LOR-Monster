using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public interface IAssetLoader<T>
{
    T LoadAsync(string id);
    void Release(T t);
}
public class AddressableAssetLoader : IAssetLoader<AsyncOperationHandle>
{
    public AddressableAssetLoader()
    {

    }
    public AsyncOperationHandle LoadAsync(string id)
    {
        return Addressables.LoadAssetAsync<Object>(id);
    }

    public void Release(AsyncOperationHandle op)
    {
        Addressables.Release(op);
    }
}
namespace Game.Pool
{

    public class GameObjectSpawner : MonoBehaviour, SpawnerBase<PoolObject>
    {
        IAssetLoader<AsyncOperationHandle> loader;
        public static GameObjectSpawner Instance;
        void Start()
        {
            Instance = this;
            loader = new AddressableAssetLoader();
        }

        protected Dictionary<string, ObjectCollection> collections = new Dictionary<string, ObjectCollection>();
        public async UniTask<PoolObject> GetAsync(string id)
        {
            ObjectCollection collection;
            if (!this.collections.ContainsKey(id))
            {
                collection = new ObjectCollection(id);
                this.collections.Add(id, collection);

            }
            else
            {
                collection = this.collections[id];
            }
            await collection.Load();

            return collection.Get();
        }
        public async UniTask Get(string id, System.Action<PoolObject> onLoaded)
        {
            ObjectCollection collection;
            if (!this.collections.ContainsKey(id))
            {
                collection = new ObjectCollection(id);
                this.collections.Add(id, collection);
            }
            else
            {
                collection = this.collections[id];
            }
            await collection.Load();
            onLoaded?.Invoke(collection.Get());
        }

        public void Destroy()
        {
            foreach (var collection in collections.Values)
            {
                collection.ClearAll();
                loader.Release(collection.loadRequest);
            }
        }
        public void ClearAll()
        {
            foreach (var collection in collections.Values)
            {
                collection.ClearAll();
            }
        }
    }

}