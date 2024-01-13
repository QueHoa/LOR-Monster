using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEngine.Rendering.DebugUI;

public class PhotoButton : GameUtility.Pooling.PoolComponent
{
    public Image bg;
    public GameObject collecting;

    CollectionPanel collectionPanel;
    string filePath;
    int value;
    int musicThemeIndex = 0;
    public void SetUp(CollectionPanel collectionPanel)
    {
        this.collectionPanel = collectionPanel;
        value = int.Parse(gameObject.name);
        filePath = Application.persistentDataPath + "/" + (value + 1).ToString() + ".jpg"; ;
        if (File.Exists(filePath))
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D loadedTexture = new Texture2D(2, 2);
            loadedTexture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));
            bg.sprite = sprite;
        }
        gameObject.SetActive(true);
    }
    public async UniTask<GameObject> SetItem(ItemData.Item item, CollectionPanel collectionPanel)
    {
        switch (item.category)
        {
            case ItemData.Category.Head:
                collectionPanel.iconItem[0].sprite = await item.GetIconAsync();
                return collectionPanel.iconItem[0].gameObject;
            case ItemData.Category.Accessory:
                collectionPanel.iconItem[1].sprite = await item.GetIconAsync();
                return collectionPanel.iconItem[1].gameObject;
            case ItemData.Category.Eye:
                collectionPanel.iconItem[2].sprite = await item.GetIconAsync();
                return collectionPanel.iconItem[2].gameObject;
            case ItemData.Category.Mouth:
                collectionPanel.iconItem[3].sprite = await item.GetIconAsync();
                return collectionPanel.iconItem[3].gameObject;
            case ItemData.Category.Body:
                collectionPanel.iconItem[4].sprite = await item.GetIconAsync();
                return collectionPanel.iconItem[4].gameObject;

        }
        return null;
    }
    public async void Choose()
    {
        StartCoroutine(collectionPanel.Effect());
        collectionPanel.viewText.text = GameUtility.GameUtility.ShortenNumber(DataManagement.DataManager.Instance.userData.progressData.collectionDatas[value].view);
        collectionPanel.likeText.text = GameUtility.GameUtility.ShortenNumber(DataManagement.DataManager.Instance.userData.progressData.collectionDatas[value].like);
        foreach (ItemData.Item item in DataManagement.DataManager.Instance.userData.progressData.collectionDatas[value].collectionItem)
        {
            await SetItem(item, collectionPanel);
            ((MakeOverGameController)Game.Controller.Instance.gameController).monster.SetItem(item);
        }
        musicThemeIndex = DataManagement.DataManager.Instance.userData.progressData.playCount == 0 ? 4 : UnityEngine.Random.Range(0, Sound.Controller.Instance.soundData.finalThemes.Length);
        ((MakeOverGameController)Game.Controller.Instance.gameController).monster.Dance(musicThemeIndex % Sound.Controller.Instance.soundData.finalThemes.Length);
        collectionPanel.ChangeCollect(value + 1);
    }
}
