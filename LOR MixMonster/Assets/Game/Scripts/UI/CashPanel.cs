using CodeStage.AntiCheat.ObscuredTypes;
using DataManagement;
using GameUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashPanel : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI cashText;
    private void OnEnable()
    {
        DataManagement.DataManager.Instance.userData.inventory.onCashUpdated += OnUpdate;
        OnUpdate(null, DataManager.Instance.userData.inventory.Cash);
    }
    private void OnDisable()
    {
        DataManagement.DataManager.Instance.userData.inventory.onCashUpdated -= OnUpdate;
    }
    private void OnDestroy()
    {
        DataManagement.DataManager.Instance.userData.inventory.onCashUpdated -= OnUpdate;
    }
    private void OnUpdate(Inventory inventory, ObscuredInt cash)
    {
        cashText.text = $"${GameUtility.GameUtility.ShortenNumber(cash)}";
    }
}
