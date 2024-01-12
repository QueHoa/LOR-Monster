using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Controller : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Clear Save")]
        static  void ClearSave()
        {
            DataManagement.SaveUtility.ClearSave();
            PlayerPrefs.DeleteAll();
        }
      
#endif
        public static Controller Instance;
        public ItemData.ItemDictionarySO itemData;
        public GameController gameController;
        public GameConfig gameConfig;
        public int playCount = 0;
        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

    
        public void OnGameLoaded(GameController gameController)
        {
            this.gameController = gameController;
            gameController.InitializeAsync();
        }
    }
}