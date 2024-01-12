using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoading : UnityEngine.MonoBehaviour
{
    public static LevelLoading Instance;
    Animator ani;

    [SerializeField]
    private Image backGroundImg;
    [SerializeField]
    private Sprite[] bgSprites;

    [SerializeField]
    private Spine.Unity.SkeletonGraphic anim;

    public void Init()
    {
        if (Instance == null)
        {
            Instance = this;
            ani = GetComponent<Animator>();
            DontDestroyOnLoad(transform.parent.gameObject);
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
    }


    System.Action onLoad, onDone;
    string sceneId;
    bool closeOverride;
    AsyncOperationHandle<SceneInstance> sceneLoader;
    public void Active(string sceneId, System.Action onLoad, System.Action onDone, bool closeOverride = false)
    {
        this.closeOverride = closeOverride;
        this.sceneId = sceneId;
        this.onLoad = onLoad;
        this.onDone = onDone;
        gameObject.SetActive(true);
        sceneLoader = Addressables.LoadSceneAsync(sceneId, LoadSceneMode.Single, false);
        isLevelLoaded = false;
        Invoke(nameof(OnLoad), 0.4f);

    }
    async UniTask ActiveScene()
    {
        await sceneLoader;
        await sceneLoader.Result.ActivateAsync();
    }
    bool isLevelLoaded = false;
    private void OnLevelWasLoaded(int level)
    {
        isLevelLoaded = true;
        Invoke(nameof(OnSceneLoaded), 0.25f);
    }
    void OnSceneLoaded()
    {
        onDone?.Invoke();
    }
    public void Active(System.Action onLoad)
    {
        ani.ResetTrigger("Close");
        sceneId = null;
        this.onLoad = onLoad;
        gameObject.SetActive(true);
        CancelInvoke();
        Invoke(nameof(OnLoad), 0.4f);
    }
    public void Close()
    {
        GameUtility.GameUtility.Log("CLOSE");
        closeOverride = false;
        ani.SetTrigger("Close");
        CancelInvoke();
    }

    public void OnLoad()
    {
        GameUtility.GameUtility.Log(" LOADING ONLOAD");

        onLoad?.Invoke();
        onLoad = null;
        if (sceneId == null)
        {
            GameUtility.GameUtility.Log("LOADING onload");
            onLoad?.Invoke();
        }
        else
        {
            ActiveScene();
        }
    }
    public void Deactive()
    {
        gameObject.SetActive(false);
    }
}
