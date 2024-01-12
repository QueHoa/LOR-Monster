using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/GameConfigPackageSO")]
public class GameConfigPackageSO : ScriptableObject
{
    public string[] configDatas;

    public GameConfigPackageSO Clone()
    {
        GameConfigPackageSO instance = CreateInstance<GameConfigPackageSO>();
        instance.configDatas = (string[])configDatas.Clone();
        return instance;
    }
    public bool IsValid()
    {
        if (configDatas == null || configDatas.Length == 0) return false;
        for(int i = 0; i < configDatas.Length; i++)
        {
            if (string.IsNullOrEmpty(configDatas[i]))
            {
                return false;
            }
        }
        return true;
    }

    public void ApplyData(string[] loadedData)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        configDatas = (string[])loadedData.Clone();
        //GameUtility.GameUtility.Log("APPLY DATA " + name);
        for (int i = 0; i < loadedData.Length; i++)
        {
            //GameUtility.GameUtility.Log(i+" "+configDatas[i]);
            configDatas[i] = new Zitga.Core.Toolkit.Compression.CompressService().Compress(Zitga.Core.Toolkit.Compression.CompressType.GZIP, configDatas[i]);
            //GameUtility.GameUtility.Log(i+" :convert:"+configDatas[i]);
        }
    }
    public string [] UnloadPackage()
    {
        for (int i = 0; i < configDatas.Length; i++)
        {
            configDatas[i] = new Zitga.Core.Toolkit.Compression.CompressService().Decompress(Zitga.Core.Toolkit.Compression.CompressType.GZIP, configDatas[i]);
        }

        return configDatas;
    }

}
