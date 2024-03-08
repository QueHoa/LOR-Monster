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
    /*[SerializeField]
    private GameObject[] board;
    [SerializeField]
    private Button[] btn;*/
    bool isProcessing;
    public override void PostInit()
    {
    }
    public void SetUp()
    {
        isProcessing = false;
        /*if (DataManagement.DataManager.Instance.userData.progressData.isView)
        {
            board[0].SetActive(false);
            board[1].SetActive(true);
            btn[0].interactable = true;
            btn[1].interactable = false;
            
        }
        else
        {
            board[1].SetActive(false);
            board[0].SetActive(true);
            btn[1].interactable = true;
            btn[0].interactable = false;
            
        }*/
        Show();
    }
    /*public void chooseBoard(bool isView)
    {
        loadingPanel.SetActive(true);
        if (isView)
        {
            DataManagement.DataManager.Instance.userData.progressData.isView = false;
            board[1].SetActive(false);
            board[0].SetActive(true);
            btn[1].interactable = true;
            btn[0].interactable = false;
        }
        else
        {
            DataManagement.DataManager.Instance.userData.progressData.isView = true;
            board[0].SetActive(false);
            board[1].SetActive(true);
            btn[0].interactable = true;
            btn[1].interactable = false;
        }
        loadingPanel.SetActive(false);
    }*/
    public void Back()
    {
        if (isProcessing) return;
        isProcessing = true;
        LevelLoading.Instance.Active(() =>
        {
            Close();
            AD.Controller.Instance.ShowInterstitial();
            UI.PanelManager.Create(typeof(HomePanel), (panel, op) =>
            {
                ((StageGameController)Game.Controller.Instance.gameController).homePanel = panel as HomePanel;
                ((HomePanel)panel).SetUp();
                ((StageGameController)Game.Controller.Instance.gameController).ShowCurrentStageMonster();
                ((StageGameController)Game.Controller.Instance.gameController).RestoreStageView();
            });
            LevelLoading.Instance.Close();
        });
    }
}
