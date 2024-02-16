using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterButton : MonoBehaviour
{
    public EBooster boosterType;
    [SerializeField]
    private TMPro.TextMeshProUGUI totalText, freeText;
    [SerializeField]
    private Image coolDownImg;
    [SerializeField]
    private GameObject adIcon, iconFree, effect;

    [SerializeField]
    private AudioClip readySFX;

    private void OnEnable()
    {
        adIcon.SetActive(DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)boosterType].Amount == 0);

        UpdateView(DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)boosterType]);
        DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)boosterType].onUpdate += UpdateView;
        //GetComponent<Button>().interactable = true;

        if (booster != null)
        {
            SetUp(booster);
        }

    }
    private void OnDisable()
    {
        DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)boosterType].onUpdate -= UpdateView;
    }
    private void OnDestroy()
    {
        DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)boosterType].onUpdate -= UpdateView;
    }
    public void UpdateView(DataManagement.BoosterData boosterData)
    {

        totalText.text = $"{boosterData.Amount}/{1}";
    }
    public void SetCoolDown(float time, System.Action onDone)
    {
        //GetComponent<Button>().interactable = false;
        coolDownImg.enabled = true;
        if(effect != null)
        {
            effect.SetActive(true);
        }
        StartCoroutine(DoCoolDown(time, onDone));
    }
    IEnumerator DoCoolDown(float time, System.Action onDone)
    {
        float t = 0;
        while (t <= time)
        {
            t += Time.deltaTime;
            coolDownImg.fillAmount = 1 - t / time;
            yield return null;
        }
        coolDownImg.enabled = false;
        if (effect != null)
        {
            effect.SetActive(false);
        }
        GetComponent<Button>().interactable = true;

        Sound.Controller.Instance.PlayOneShot(readySFX);
        onDone?.Invoke();
    }
    IBooster booster;

    public void SetUp(IBooster booster)
    {
        this.booster = booster;
        booster.AddOnBoosterActive(OnBoosterActive);
        booster.AddOnBoosterRefilled(OnBoosterRefilled);
        freeText.text = string.Empty;
        iconFree.SetActive(true);
        if (booster.IsActive())
        {
            SetCoolDown(booster.GetDuration() - booster.GetCurrentCoolDown(), () => { adIcon.SetActive(true); });
            adIcon.SetActive(false);
        }
        else
        {
            coolDownImg.enabled = false;
            if (effect != null)
            {
                effect.SetActive(false);
            }
        }
    }

    private void OnBoosterRefilled()
    {
        freeText.text = string.Empty;
        adIcon.SetActive(false);
        iconFree.SetActive(true);
    }

    void OnBoosterActive()
    {
        SetCoolDown(booster.GetDuration(), () => { adIcon.SetActive(true); });
        adIcon.SetActive(false);

    }

    float time = 0;
    private void Update()
    {
        if (Time.time - time > 1 && DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)boosterType].Amount < 1 &&
           System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)boosterType].lastUseFree)).TotalSeconds < Sheet.SheetDataManager.Instance.gameData.stageConfig.boosterRecoverTime)
        {
            var span = System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)boosterType].lastUseFree));
            span = TimeSpan.FromSeconds(Sheet.SheetDataManager.Instance.gameData.stageConfig.boosterRecoverTime).Subtract(span);
            iconFree.SetActive(false);
            freeText.text = $"Free {span.Minutes}:{span.Seconds}";
            time = Time.time;
        }
    }
}
