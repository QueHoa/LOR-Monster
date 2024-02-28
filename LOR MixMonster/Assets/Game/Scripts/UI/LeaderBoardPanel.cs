using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class LeaderBoardPanel : Panel
{
    [SerializeField]
    private GameObject loadingPanel; 
    [SerializeField]
    private GameObject[] board;
    [SerializeField]
    private Button[] btn;
    bool isProcessing;
    public override void PostInit()
    {
    }
    public void SetUp()
    {
        isProcessing = false;
        if ((Game.Controller.Instance.gameController).isView)
        {
            (Game.Controller.Instance.gameController).isView = false;
            board[1].SetActive(false);
            board[0].SetActive(true);
            btn[1].interactable = true;
            btn[0].interactable = false;
            
        }
        else
        {
            (Game.Controller.Instance.gameController).isView = true;
            board[0].SetActive(false);
            board[1].SetActive(true);
            btn[0].interactable = true;
            btn[1].interactable = false;
            
        }
        Show();
    }
    public void chooseBoard(bool isView)
    {
        loadingPanel.SetActive(true);
        if (isView)
        {
            (Game.Controller.Instance.gameController).isView = false;
            board[1].SetActive(false);
            board[0].SetActive(true);
            btn[1].interactable = true;
            btn[0].interactable = false;
        }
        else
        {
            (Game.Controller.Instance.gameController).isView = true;
            board[0].SetActive(false);
            board[1].SetActive(true);
            btn[0].interactable = true;
            btn[1].interactable = false;
        }
        loadingPanel.SetActive(false);
    }
    public void Back()
    {
        if (DataManagement.DataManager.Instance.userData.progressData.playCount >= Game.Controller.Instance.gameConfig.adConfig.adStart)
        {
            AD.Controller.Instance.ShowInterstitial(() =>
            {
                BackHome();

            });
        }
        else
        {
            BackHome();
        }


        void BackHome()
        {
            if (isProcessing) return;
            isProcessing = true;
            LevelLoading.Instance.Active(() =>
            {
                Close();
                ((StageGameController)Game.Controller.Instance.gameController).SetUp();
                //LevelLoading.Instance.Close();
            });
        }
    }
}
