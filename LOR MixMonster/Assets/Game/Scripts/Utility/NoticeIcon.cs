using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UIHandler;
using UnityEngine;
using UnityEngine.UI;

public class NoticeIcon : MonoBehaviour
{
    public delegate void OnRecheck();
    public static OnRecheck onRecheck;

    public static void RecheckState()
    {
        onRecheck?.Invoke();
    }
    Transform _transform;
    [SerializeField]
    private Button mainButton;
    [Flags]
    public enum NoticeType 
    {
        None=0,
        CheckUpgradeAvailable = 1<<1,
        CheckCollectionMachineGun = 1<<2,
        CheckCollectionGun = 1<<3,
        CheckFreeReward = 1<<4
    }
    public NoticeType  noticeType;
    private List<INoticeHandler>  handler=new List<INoticeHandler>();
    private async UniTask Awake()
    {
        _transform = transform;
        ImageButtonAnimation otherButton=null;
        if (mainButton == null)
        {
            mainButton = GetComponentInParent<Button>();
            if (mainButton == null)
            {
                otherButton = GetComponentInParent<ImageButtonAnimation>();
            }
        }
    
        //if ((noticeType & NoticeType.CheckUpgradeAvailable) != 0)
        //    handler.Add(new NoticeCheckUpgrade());
        //if ((noticeType & NoticeType.CheckCollectionGun) != 0)
        //    handler.Add(new NoticeCheckCollectNewSupportGun());
        //if ((noticeType & NoticeType.CheckCollectionMachineGun) != 0)
        //    handler.Add(new NoticeCheckCollectNewMachineGun());
        //if ((noticeType & NoticeType.CheckFreeReward) != 0)
        //    handler.Add(new NoticeCheckCollectNewFreeGift());

        Deactive();
        await UniTask.DelayFrame(10);
        bool check = false;
        for(int i = 0; i < handler.Count; i++)
        {
            if (handler[i].CheckState())
            {
                check = true;
                break;
            }
        }

        if (!check)
        {
            Deactive();
        }
        else
        {
            Active();
            if (mainButton != null)
            {
                mainButton.onClick.AddListener(Deactive);
            }
            else
            {
                otherButton.AddListenner(Deactive);

            }
        }
        NoticeIcon.onRecheck -= Check;
        NoticeIcon.onRecheck += Check;
     
    }
   

    void Check()
    {
        try
        {
            for (int i = 0; i < handler.Count; i++)
            {
                if (handler[i].CheckState())
                {
                    Active();
                    return;
                }
            }
            Deactive();
        }
        catch
        {
            NoticeIcon.onRecheck -= Check;
        }
    }

    Vector3 scale;
    float a = 0;
    private void Update()
    {
        scale.x = scale.y = 1+Mathf.Sin(a)/12f;
        _transform.localScale = scale;
        a += Mathf.PI / 25;
    }
    public void Active()
    {
        gameObject.SetActive(true);
        a = 0;
    }
    public void Deactive()
    {
        gameObject.SetActive(false);
    }
}


public interface INoticeHandler
{
    bool CheckState();
}



//public class NoticeCheckCollectNewFreeGift : INoticeHandler
//{
//    public bool CheckState()
//    {

//        return DataManagement.DataManager.Instance.userData.progressData.comboRewardTrack<Game.Controller.Instance.gameConfig.comboRewardData.Length;
//    }
//}

