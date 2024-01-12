using UnityEngine.AddressableAssets;
namespace GameUtility
{
    public class AssetLoader
    {
        public static void LoadAsset<T>(string address, System.Action<T> onLoaded)
        {
            Addressables.LoadAsset<T>(address).Completed += (op) =>
            {
                T result = op.Result;
                onLoaded?.Invoke(result);
                Addressables.Release(op);
            };
        }
    }
}
