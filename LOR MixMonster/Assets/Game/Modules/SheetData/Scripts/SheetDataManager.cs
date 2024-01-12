using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.IO;
using System;
using System;
using System.Linq;

namespace Sheet
{
    public static class UriExtensions
    {
        public static Uri Append(this Uri uri, params string[] paths)
        {
            return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/'))));
        }
    }
    public class SheetDataManager : MonoBehaviour
    {

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Sheet/Fetch new data")]
        static async void FetchNewData()
        {
            SheetDataManager sheetDataManager = GameObject.FindObjectOfType<SheetDataManager>();
            string[] loadedData = null;
            GameUtility.GameUtility.Log("UPDATE GOOGLE SHEET");
            List<UniTask<string>> tasks = new List<UniTask<string>>();
          
            tasks.Add(GetData(sheetDataManager.gameData.localizationSheetUrl));
            tasks.Add(GetData(sheetDataManager.gameData.itemSheetUrl));
           
            //đợi tất cả data load về
            loadedData = await UniTask.WhenAll(tasks);
            sheetDataManager.updatePackage.ApplyData(loadedData);
            sheetDataManager.defaultUpdatePackage.ApplyData(loadedData);

            sheetDataManager.ApplySheetData();
            GameUtility.GameUtility.Log("DONE FEtCHING DATA");
        }
        public void ApplySheetData()
        {
            List<GSheetData[]> sheetData = new List<GSheetData[]>();
            GameConfigPackageSO configData = updatePackage.Clone();
            string[] loadedData  = configData.UnloadPackage();

            for (int i = 0; i < loadedData.Length; i++)
            {
                sheetData.Add(JsonConvert.DeserializeObject<GSheetData[]>(loadedData[i]));
            }
            localizationData.ApplyData(ConvertSheetToList(sheetData[0][0].GoogleSheetData));

            Debug.Log("INIT DATA: item");
            gameData.itemData.ApplyData(sheetData[1]);
            gameData.rewardBarConfig.ApplyData(ConvertSheetToList(sheetData[1][7].GoogleSheetData));
            gameData.rewardGold.ApplyData(ConvertSheetToList(sheetData[1][9].GoogleSheetData));

        }
#endif

        public static SheetDataManager Instance;
        public GameDataConfigSO gameData;
        public GameConfigPackageSO updatePackage;
        public GameConfigPackageSO defaultUpdatePackage;

        public LocalizationCollectionSO localizationData { get { return gameData.localizationData; } set { gameData.localizationData = value; } }

        List<GSheetData[]> sheetData = new List<GSheetData[]>();
        public bool isReady = false;
        [SerializeField]
        private RuntimeEnvironment environment = RuntimeEnvironment.Development;
        private enum RuntimeEnvironment
        {
            Development, Production, Editor
        }
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SetUp().Forget();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async UniTaskVoid SetUp()
        {
            string[] loadedData = null;
            GameConfigPackageSO configData = null;
            configData = defaultUpdatePackage.Clone();
            loadedData = configData.UnloadPackage();

            sheetData.Clear();
            for (int i = 0; i < loadedData.Length; i++)
            {
                sheetData.Add(JsonConvert.DeserializeObject<GSheetData[]>(loadedData[i]));
            }
            gameData = gameData.Clone();

            InitData();
            isReady = true;
            sheetData.Clear();
            //await UniTask.WaitUntil(() => RemoteConfigHandler.Instance!=null &&RemoteConfigHandler.Instance.isReady);
            //string[] loadedData = null;
            //GameConfigPackageSO configData = null;
            //if (environment == RuntimeEnvironment.Development)
            //{
            //    configData = defaultUpdatePackage.Clone();
            //}
            //else if (environment == RuntimeEnvironment.Production)
            //{
            //    configData = updatePackage.IsValid()? updatePackage.Clone():defaultUpdatePackage.Clone();
            //}
            //loadedData = configData.UnloadPackage();

            //sheetData.Clear();
            //for (int i = 0; i < loadedData.Length; i++)
            //{
            //    sheetData.Add(JsonConvert.DeserializeObject<GSheetData[]>(loadedData[i]));
            //}
            //gameData = gameData.Clone();

            //InitData();
            //isReady = true;
            //sheetData.Clear();
        }


        public void InitData()
        {

            //try
            //{
            //    Debug.Log("INIT DATA: Localize");
            //    localizationData = localizationData.Clone();
            //    localizationData.ApplyData(ConvertSheetToList(sheetData[0][0].GoogleSheetData));
            //    Debug.Log("=>FINISH DATA: Localize");

            //}
            //catch (System.Exception e)
            //{
            //    GameUtility.GameUtility.LogError(e);
            //}


        }
        static async UniTask<string> GetData(string url)
        {
#if UNITY_EDITOR
            GameUtility.GameUtility.Log(url);
#endif
            UnityWebRequest webrequest = UnityWebRequest.Get(url);
            webrequest.timeout = 400;
            UnityWebRequestAsyncOperation sendWebRequestAsyncOperation = webrequest.SendWebRequest();
            await sendWebRequestAsyncOperation;
            return webrequest.downloadHandler.text;
        }
    
        public static List<RowData> ConvertSheetToList(object[,] data)
        {
            List<RowData> list = new List<RowData>();
            for (int i = 0; i < data.GetLength(0); i++)
            {
                RowData row = new RowData();
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    string value = data[i, j].ToString();
                    row.list.Add(value);
                }
                list.Add(row);
            }
            return list;
        }
       
    }
    [System.Serializable]
    public class RowData
    {
        public List<string> list;

        public RowData()
        {
            list = new List<string>();
        }
    }
}