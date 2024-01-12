using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelFadeAnimation : UnityEngine.MonoBehaviour
{
    Graphic[] graphics;
    float[] alphas;

    [SerializeField]
    private bool useScale = true;
    void Init()
    {
        if (graphics == null)
        {
            graphics = GetComponentsInChildren<Graphic>(true);
            alphas = new float[graphics.Length];
            for (int i = 0; i < graphics.Length; i++)
            {
                alphas[i] = graphics[i].color.a;
            }
        }

    }
    public void Clear()
    {
        Init();
        Transform t = transform;
        t.localScale = Vector3.one;

        Color c = Color.white;
        for (int i = 0; i < graphics.Length; i++)
        {
            c = graphics[i].color;
            c.a = alphas[i];
            graphics[i].color = c;
        }
    }
    private void OnDisable()
    {
        isOpenning = false;
        Clear();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        Clear();
    }
    public void Show()
    {
        gameObject.SetActive(true);
        Open();
    }
    bool isOpenning = false;
    Coroutine openC;
    public void Open()
    {
        if (isOpenning) return;
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        Init();
        Clear();

        isOpenning = true;

        openC = StartCoroutine(DoOpen());
    }
    IEnumerator DoOpen()
    {
        float a = 0;
        Color c = Color.white;
        c.a = 0;
        Transform t = transform;
        Vector3 scale = Vector3.one * 0.95f;
        while (a <= Mathf.PI / 2f)
        {
            float sin = Mathf.Sin(a);
            t.localScale = scale;
            scale.x = scale.y = 0.95f + sin / 20f;
            for (int i = 0; i < graphics.Length; i++)
            {
                c = graphics[i].color;
                c.a = sin * alphas[i];
                graphics[i].color = c;
            }
            a += Mathf.PI / 20;
            yield return null;
        }
        t.localScale = Vector3.one;
        isOpenning = false;
    }
    bool isClosing = false;
    System.Action onClosed;
    public void Close(System.Action onClosed)
    {
        if (!gameObject.activeSelf) return;
        if (isClosing) return;
        isClosing = true;
        Init();
        this.onClosed = onClosed;
        if (isOpenning)
        {
            if (openC != null)
                StopCoroutine(openC);
            isOpenning = false;
        }
        if (gameObject.activeInHierarchy)
            StartCoroutine(DoClose());
        else
        {
            gameObject.SetActive(false);
        }
    }
    IEnumerator DoClose()
    {
        float a = 0;
        Color c = Color.white;
        c.a = 1;
        Transform t = transform;
        Vector3 scale = Vector3.one;
        while (a <= Mathf.PI / 2f)
        {
            float sin = Mathf.Sin(a);

            t.localScale = scale;
            scale.x = scale.y = 1 - sin / 20f;

            for (int i = 0; i < graphics.Length; i++)
            {
                c = graphics[i].color;
                c.a = (1 - sin) * alphas[i];
                graphics[i].color = c;
            }
            a += Mathf.PI / 20;
            yield return null;
        }
        onClosed?.Invoke();
        isClosing = false;
    }
}
