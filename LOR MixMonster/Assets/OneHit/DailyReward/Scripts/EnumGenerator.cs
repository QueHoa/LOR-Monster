#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class EnumGenerator : MonoBehaviour
{
    [InfoBox("Folder bạn muốn script MyEnum được sinh ra")]
    [FolderPath] public string folderPath;

    public string[] myStrings;

    [Button("Generate MyEnum")]
    public void GenerateEnum()
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogError("Vui lòng kéo và thả thư mục vào trường 'Folder Path' trên Inspector.");
            return;
        }

        var enumBuilder = new System.Text.StringBuilder();
        enumBuilder.AppendLine("public enum MyEnum");
        enumBuilder.AppendLine("{");

        for (int i = 0; i < myStrings.Length; i++)
        {
            var enumValue = myStrings[i].ToUpper();
            enumBuilder.AppendLine($"    {enumValue},");
        }

        enumBuilder.AppendLine("}");

        var enumCode = enumBuilder.ToString();
        Debug.Log(enumCode);

        var path = $"{folderPath}/MyEnum.cs";
        System.IO.File.WriteAllText(path, enumCode);

        AssetDatabase.Refresh();
        Debug.Log("Đã tạo enum thành công!");
    }
}
#endif