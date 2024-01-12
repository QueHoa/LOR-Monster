using IAP;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Purchasing;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Purchasing;

[CustomEditor(typeof(PurchaseButton))]
[CanEditMultipleObjects]
public class PurchaseButtonEditor : Editor
{
    private static readonly string[] excludedFields = new string[] { "m_Script" };
    private static readonly string[] restoreButtonExcludedFields = new string[] { "m_Script", "consumePurchase", "onPurchaseComplete", "onPurchaseFailed", "titleText", "descriptionText", "priceText" };
    private const string kNoProduct = "<None>";

    private List<string> m_ValidIDs = new List<string>();
    private SerializedProperty m_ProductIDProperty;
    IAPPackageDataSO catalog;

    /// <summary>
    /// Event trigger when IAPButton is enabled in the scene.
    /// </summary>
    public void OnEnable()
    {
        m_ProductIDProperty = serializedObject.FindProperty("productId");
    }

    /// <summary>
    /// Event trigger when trying to draw the IAPButton in the inspector.
    /// </summary>
    public override void OnInspectorGUI()
    {
        PurchaseButton button = (PurchaseButton)target;

        serializedObject.Update();

        if (button.buttonType == PurchaseButton.ButtonType.Purchase)
        {
            EditorGUILayout.LabelField(new GUIContent("Product ID:", "Select a product from the IAP catalog."));


            m_ValidIDs.Clear();
            m_ValidIDs.Add(kNoProduct);

            catalog = Resources.Load<IAPPackageDataSO>("IAP_Android");



            foreach (var product in catalog.products)
            {
                m_ValidIDs.Add(product.id);
            }

            int currentIndex = string.IsNullOrEmpty(button.productId) ? 0 : m_ValidIDs.IndexOf(button.productId);
            int newIndex = EditorGUILayout.Popup(currentIndex, m_ValidIDs.ToArray());
            if (newIndex > 0 && newIndex < m_ValidIDs.Count)
            {
                m_ProductIDProperty.stringValue = m_ValidIDs[newIndex];
            }
            else
            {
                m_ProductIDProperty.stringValue = string.Empty;
            }

            if (GUILayout.Button("IAP Catalog..."))
            {
                ProductCatalogEditor.ShowWindow();
            }
        }

        DrawPropertiesExcluding(serializedObject, button.buttonType == PurchaseButton.ButtonType.Restore ? restoreButtonExcludedFields : excludedFields);

        serializedObject.ApplyModifiedProperties();
    }
}