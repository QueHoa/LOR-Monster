using Cysharp.Threading.Tasks;
using ItemData;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class SharePanel : UI.Panel
{
    [SerializeField]
    private UnityEngine.UI.Image screenshot;
    bool isProcessing;

    public override void PostInit()
    {
    }
    public void SetUp(Sprite screenshot)
    {
        isProcessing = false;
        this.screenshot.sprite = screenshot;
        Texture2D rawImageTexture = screenshot.texture;
        byte[] bytes = rawImageTexture.EncodeToJPG(50); // Chuyển texture thành dãy byte JPG
        string filePath = UnityEngine.Application.persistentDataPath + "/" + "home" + ".jpg";
        File.WriteAllBytes(filePath, bytes);
        Show();
    }
    public void Share()
    {
        if (isProcessing) return;
        isProcessing = true;

        Sprite sprite = screenshot.sprite;

        if (sprite == null)
        {
            Debug.LogError("Không thể chuyển đổi hình ảnh thành Texture2D: Sprite không tồn tại.");
            return;
        }
        Texture2D text = GetTextureFromSprite(sprite);
        if (UnityEngine.Application.isMobilePlatform)
        {
            NativeShare nativeShare = new NativeShare();
            nativeShare.AddFile(text, "home.jpg");
            nativeShare.Share();
        }
        isProcessing = false;
    }

    private Texture2D GetTextureFromSprite(Sprite sprite)
    {
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);

        texture.SetPixels(sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height));
        texture.Apply();

        return texture;
    }
    public override void Close()
    {
        base.Close();
        (Game.Controller.Instance.gameController).hideMonster = false;
    }
}
