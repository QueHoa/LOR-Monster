using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Pool
{
    public interface SpawnerBase<T>
    {

        UniTask<T> GetAsync(string id);
        void ClearAll();
        void Destroy();
    }

}