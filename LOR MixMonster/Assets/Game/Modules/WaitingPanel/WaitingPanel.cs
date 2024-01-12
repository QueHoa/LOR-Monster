using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPanel : UI.Panel
{
    private static UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> op;
    public static void Create(System.Action<UI.Panel> onDone)
    {
        if (Instance != null)
        {
            onDone?.Invoke(Instance);
            return;
        }
        UnityEngine.AddressableAssets.Addressables.InstantiateAsync("WaitingPanel", UI.PanelManager.Instance.transform).Completed += (op) =>
        {
            WaitingPanel.op = op;
            Instance = op.Result.GetComponent<WaitingPanel>();
            onDone?.Invoke(Instance);
        };
    }
    private void OnDisable()
    {
        if (op.IsValid())
        {
            UnityEngine.AddressableAssets.Addressables.ReleaseInstance(op);
            Instance = null;
        }
    }
    public static WaitingPanel Instance;
    public override void PostInit()
    {
        Instance = this;
    }

    [SerializeField]
    private GameObject closeBtn;
    [SerializeField]
    private Image backGroundImg;
   

    public void SetUp(float backGroundAlpha = 0.6f)
    {
        Color c = backGroundImg.color;
        c.a = backGroundAlpha;
        backGroundImg.color = c;
        Show();

        closeBtn.SetActive(false);
        Invoke(nameof(ShowClose), 2);
    }
    public void ShowClose()
    {
        closeBtn.SetActive(true);
    }

    public override void Hide()
    {
        Deactive();
    }
}
