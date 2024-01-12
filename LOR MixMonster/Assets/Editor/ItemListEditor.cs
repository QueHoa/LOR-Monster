using ItemData;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ItemData.ItemDictionarySO))]
public class ItemListEditor : Editor
{
    string json;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ItemDictionarySO target = (ItemDictionarySO)this.target;
      
        if (GUILayout.Button("Populate"))
        {
            EditorUtility.SetDirty(target);
            
        }
        
       
    }
}
