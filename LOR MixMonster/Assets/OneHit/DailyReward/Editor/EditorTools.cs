using UnityEngine;
using UnityEditor;

public class EditorTools {

     public static bool DrawHeader(string text) {
          string key = text;

          // check extended/collapsed
          bool isExtended = EditorPrefs.GetBool(key, true);

          GUILayout.Space(3f);
          if (!isExtended) {
               GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
          }
          GUILayout.BeginHorizontal();
          GUI.changed = false;

          text = "<b><size=12><color=yellow>" + text + "</color></size></b>";
          text = (isExtended ? "\u25BC " : "\u25BA ") + text;

          if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
               isExtended = !isExtended;
          }

          if (GUI.changed) {
               EditorPrefs.SetBool(key, isExtended);
               isExtended = EditorPrefs.GetBool(text, true);
          }

          GUILayout.Space(2f);
          GUILayout.EndHorizontal();
          GUI.backgroundColor = Color.white;
          if (!isExtended) {
               GUILayout.Space(3f);
          }
          return isExtended;
     }

     public static void BeginContents() {
          GUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
          GUILayout.Space(5f);
          GUILayout.BeginVertical();
          GUILayout.Space(2f);
     }

     public static void EndContents() {
          GUILayout.Space(3f);
          GUILayout.EndVertical();
          EditorGUILayout.EndHorizontal();
          GUILayout.Space(3f);
     }
}