using Cysharp.Threading.Tasks;
using ItemData;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class GotMoneyPanel : UI.Panel
{
    [SerializeField]
    private int cash;
    bool isProcessing;
    public override void PostInit()
    {
    }
    public void SetUp()
    {
        Show();
    }
    public void Get()
    {
        DataManagement.DataManager.Instance.userData.inventory.Cash += cash;
        (Game.Controller.Instance.gameController).hideMonster = false;
        Close();
    }
}
