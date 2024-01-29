using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace DataManagement
{
    public class DataManager 
    {
        public static DataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataManager();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }
        private static DataManager instance;

        public UserData userData;
     

        public DataManager()
        {
            GameUtility.GameUtility.Log("NEWE DATA MANAGER ");
            userData = SaveUtility.Instance.LoadFile<UserData>();
            if (userData == null)
            {
                userData = new UserData();
                userData.Init();
            }
            if (userData.stageListData == null)
            {
                userData.stageListData = new UserStageData();
            }

            if (userData.stageListData.boosters == null || userData.stageListData.boosters.Count == 0)
            {
                userData.stageListData.boosters = new List<BoosterData>();
                userData.stageListData.boosters.Add(new BoosterData(EBooster.InstantMoney, 1));
                userData.stageListData.boosters.Add(new BoosterData(EBooster.SpeedBoost, 1));
                userData.stageListData.boosters.Add(new BoosterData(EBooster.AutoClick, 1));
            }
            if (userData.progressData.IsNewDay())
            {
                userData.progressData.CheckTotalSessionOfDay();
                userData.progressData.comboRewardTrack = 0;
            }
            Save();

        }
        public bool IsReady()
        {
            GameUtility.GameUtility.Log("IS READY " + (userData == null));
            return userData != null;
        }
        public void Save()
        {
            try
            {
                SaveUtility.Instance.Save<UserData>(userData);
            }
            catch(System.Exception e)
            {
                if (e.GetType() == typeof(IOException))
                {
                    UI.PanelManager.Create(typeof(MessagePanel), (panel, op) =>
                    {
                        ((MessagePanel)panel).SetUp("Your device storage is full. Free up some space so the game can be save");
                    });
                }
            }
        }
    }

    public class SaveUtility
    {
        public static SaveUtility Instance
        {
            get {
                if (instance == null)
                {
                    instance = new SaveUtility();
                }
                return instance; }
            set { instance = value; }
        }
        private static SaveUtility instance;
        FileStream dataStream;
        // Key for reading and writing encrypted data.
        private byte[] savedKey;
        private byte[] secretKey = { 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15 };
        private static string saveFile = "virus1";

        public SaveUtility()
        {
            if (PlayerPrefs.HasKey("key"))
            {
                savedKey = System.Convert.FromBase64String(PlayerPrefs.GetString("key"));
            }
            else
            {
                savedKey = secretKey;
            }
            saveFile = Path.Combine(Application.persistentDataPath, saveFile);
        }
        public static void ClearSave()
        {
            string saveFile = Path.Combine(Application.persistentDataPath, SaveUtility.saveFile);
            if (File.Exists(saveFile))
            {
                File.Delete(saveFile);
            }
        }
        public void UpdateKey()
        {
            savedKey = secretKey;
        }
        public bool Save<T>(T data)
        {
            if (File.Exists(saveFile))
            {
                File.Delete(saveFile);
            }
            // Create new AES instance.
            Aes iAes = Aes.Create();

            // Update the internal key.
            //savedKey = iAes.Key;


            // Create a FileStream for creating files.
            dataStream = new FileStream(saveFile, FileMode.Create);

            // Save the new generated IV.
            byte[] inputIV = iAes.IV;

            // Write the IV to the FileStream unencrypted.
            dataStream.Write(inputIV, 0, inputIV.Length);
            //GameUtility.GameUtility.Log("save " + savedKey);
            //for (int i = 0; i < savedKey.Length; i++)
            //{
            //    GameUtility.GameUtility.Log(savedKey[i]);
            //}
            // Create CryptoStream, wrapping FileStream.
            CryptoStream iStream = new CryptoStream(
                    dataStream,
                    iAes.CreateEncryptor(savedKey, iAes.IV),
                    CryptoStreamMode.Write);

            // Create StreamWriter, wrapping CryptoStream.
            StreamWriter sWriter = new StreamWriter(iStream);

            // Serialize the object into JSON and save string.
            string jsonString = JsonConvert.SerializeObject(data, new ObscuredValueConverter());

            // Write to the innermost stream (which will encrypt).
            sWriter.Write(jsonString);

            // Close StreamWriter.
            sWriter.Close();

            // Close CryptoStream.
            iStream.Close();

            // Close FileStream.
            dataStream.Close();
            //PlayerPrefs.SetString("key", System.Convert.ToBase64String(savedKey));

            return true;
        }

        public T LoadFile<T>()
        {
            // Does the file exist?
            if (File.Exists(saveFile))
            {
                try
                {
                    // Create FileStream for opening files.
                    dataStream = new FileStream(saveFile, FileMode.Open);

                    // Create new AES instance.
                    Aes oAes = Aes.Create();

                    // Create an array of correct size based on AES IV.
                    byte[] outputIV = new byte[oAes.IV.Length];

                    // Read the IV from the file.
                    dataStream.Read(outputIV, 0, outputIV.Length);

                    // Create CryptoStream, wrapping FileStream
                    //GameUtility.GameUtility.Log("load " + savedKey);
                    //for(int i = 0; i < savedKey.Length; i++)
                    //{
                    //    GameUtility.GameUtility.Log(savedKey[i]);
                    //}
                    CryptoStream oStream = new CryptoStream(
                           dataStream,
                           oAes.CreateDecryptor(savedKey, outputIV),
                           CryptoStreamMode.Read);

                    // Create a StreamReader, wrapping CryptoStream
                    StreamReader reader = new StreamReader(oStream);

                    // Read the entire file into a String value.
                    string text = reader.ReadToEnd();
                    // Always close a stream after usage.
                    reader.Close();
                    dataStream.Close();

                    // Deserialize the JSON data 
                    //  into a pattern matching the GameData class.
                    T data = JsonConvert.DeserializeObject<T>(text, new ObscuredValueConverter());
                    return data;
                }
                catch(System.Exception e)
                {
                    dataStream.Close();
                    GameUtility.GameUtility.LogError(e);
                    return default(T);
                }
                }
            return default(T);
        }
    }
}

