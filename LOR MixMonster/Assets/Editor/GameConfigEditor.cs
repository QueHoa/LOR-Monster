using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(GameConfig))]
public class GameConfigEditor : Editor
{
    string json;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameConfig target = (GameConfig)this.target;
      
        if (GUILayout.Button("Create json"))
        {
            json=Newtonsoft.Json.JsonConvert.SerializeObject(target,new ObscuredValueConverter());
        }
        if (!string.IsNullOrEmpty(json))
        {
            GUILayout.TextArea(json, GUILayout.Height(100));
        }

       
    }
}
