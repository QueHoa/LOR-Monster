using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SettingPopup : UI.Panel
{
    public SetButton music, sound, vibration;

    bool isProcessing = false;
    public override void PostInit()
    {
    }
    public void SetUp()
    {
        isProcessing = false;
        if (DataManagement.DataManager.Instance.userData.progressData.music)
        {
            music.icon.transform.position = music.onPos.position;
            music.icon.sprite = music.iconOn;
            music.bg.sprite = music.bgOn;
        }
        else
        {
            music.icon.transform.position = music.offPos.position;
            music.icon.sprite = music.iconOff;
            music.bg.sprite = music.bgOff;
        }
        if (DataManagement.DataManager.Instance.userData.progressData.sound)
        {
            sound.icon.transform.position = sound.onPos.position;
            sound.icon.sprite = sound.iconOn;
            sound.bg.sprite = sound.bgOn;
        }
        else
        {
            sound.icon.transform.position = sound.offPos.position;
            sound.icon.sprite = sound.iconOff;
            sound.bg.sprite = sound.bgOff;
        }
        if (DataManagement.DataManager.Instance.userData.progressData.vibration)
        {
            vibration.icon.transform.position = vibration.onPos.position;
            vibration.icon.sprite = vibration.iconOn;
            vibration.bg.sprite = vibration.bgOn;
        }
        else
        {
            vibration.icon.transform.position = vibration.offPos.position;
            vibration.icon.sprite = vibration.iconOff;
            vibration.bg.sprite = vibration.bgOff;
        }
        Show();
    }
    public void SetMusic()
    {
        music.Move(DataManagement.DataManager.Instance.userData.progressData.music);
        DataManagement.DataManager.Instance.userData.progressData.music = !DataManagement.DataManager.Instance.userData.progressData.music;
        DataManagement.DataManager.Instance.Save();
    }
    public void SetSound()
    {
        sound.Move(DataManagement.DataManager.Instance.userData.progressData.sound);
        DataManagement.DataManager.Instance.userData.progressData.sound = !DataManagement.DataManager.Instance.userData.progressData.sound;
        DataManagement.DataManager.Instance.Save();
    }
    public void SetVibration()
    {
        vibration.Move(DataManagement.DataManager.Instance.userData.progressData.vibration);
        DataManagement.DataManager.Instance.userData.progressData.vibration = !DataManagement.DataManager.Instance.userData.progressData.vibration;
        DataManagement.DataManager.Instance.Save();
    }
    public void BackHome()
    {
        if (DataManagement.DataManager.Instance.userData.progressData.playCount >= Game.Controller.Instance.gameConfig.adConfig.adStart)
        {
            AD.Controller.Instance.ShowInterstitial(() =>
            {
                Back();

            });
        }
        else
        {
            Back();
        }

        void Back()
        {
            if (isProcessing) return;
            isProcessing = true;
            Close();
        }

    }
}
