using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeScreen : UnityEngine.MonoBehaviour
{
    public static FadeScreen Instance;
    Animator ani;
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

    System.Action onLoad;

    float delayTime = 0.8f;
    public void Active(System.Action onLoad)
    {
        this.onLoad = onLoad;
        gameObject.SetActive(true);
        Invoke(nameof(OnLoad), 0.3f);
    }
    public void Close()
    {
        ani.SetTrigger("Close");
    }
    public void OnLoad()
    {
        onLoad?.Invoke();
    }
    public void Deactive()
    {
        gameObject.SetActive(false);
    }
}
