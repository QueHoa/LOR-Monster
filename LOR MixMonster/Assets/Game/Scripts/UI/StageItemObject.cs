using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageItemObject : MonoBehaviour
{
    SpriteRenderer[] srs;
    Color[] colors;
    [SerializeField]
    private AnimationCurve scaleCurve;
    [SerializeField]
    private Sprite[] customSprites;
    [SerializeField]
    private SpriteRenderer mainSR;
    private void OnEnable()
    {
        srs = GetComponentsInChildren<SpriteRenderer>();
        colors = new Color[srs.Length];
        for (int i = 0; i < srs.Length; i++)
        {
            colors[i] = srs[i].color;
        }
    }
    public void SetUp(int stageIndex)
    {
        Debug.Log("SET UP " + stageIndex + " " + gameObject.name);
        if (customSprites != null && customSprites.Length != 0)
        {
            mainSR.sprite = customSprites[stageIndex];
        }
    }
    public async UniTask Show()
    {
        gameObject.SetActive(true);
        float a = 0;
        Vector3 scale = Vector3.zero;
        while (a <= Mathf.PI / 2f)
        {
            scale.x = scale.y = scaleCurve.Evaluate(a / (Mathf.PI / 2f));
            transform.localScale = scale;
            for (int i = 0; i < srs.Length; i++)
            {
                Color c = colors[i];
                c.a = Mathf.Sin(a);
                srs[i].color = c;
            }

            if (a >= Mathf.PI / 2f) break;
            a += Mathf.PI * Time.deltaTime * 3;
            await UniTask.Yield();
        }
    }
    public async UniTask Hide()
    {
        float a = Mathf.PI / 2f;
        Vector3 scale = Vector3.zero;
        while (a >= 0)
        {
            scale.x = scale.y = scaleCurve.Evaluate(a / (Mathf.PI / 2f));
            transform.localScale = scale;
            for (int i = 0; i < srs.Length; i++)
            {
                Color c = colors[i];
                c.a = Mathf.Sin(a);
                srs[i].color = c;
            }

            if (a <= 0) break;
            a -= Mathf.PI * Time.deltaTime * 3;
            await UniTask.Yield();

        }
        gameObject.SetActive(false);
    }
}
