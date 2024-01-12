using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace OneHit.Utility
{
    public class PostProcessorBuild : UnityEditor.Build.IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }
        GoogleDriveURL link;
        public void OnPostprocessBuild(BuildReport report)
        {
            link = Resources.Load<GoogleDriveURL>("GoogleDriveURL");
            Debug.Log(link.aabLink);
            Debug.Log(link.apkLink);
            string fileExtension = System.IO.Path.GetExtension(report.summary.outputPath);
            Debug.Log(report.summary.outputPath);
            Debug.Log(fileExtension);
            OpenLink(fileExtension);

            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.LogError("succeed");

            }
            else
            {
                Debug.LogError("Fail");
            }
        }
        void OpenLink(string type)
        {
            if (type == ".aab")
            {
                Application.OpenURL(link.aabLink);
            }
            else if (type == ".apk")
            {
                Application.OpenURL(link.apkLink);
            }
        }

        void UpFileToGoogleDrive()
        {
            //try
            //{
            //    var content = ;
            //    var file = new UnityGoogleDrive.Data.File { Name = "Image.png", Content = content };
            //    file.Parents = new List<string> { "15YbzeVg5urBLTTpFCZWmM4c98haV1SDq" };
            //    GoogleDriveFiles.Create(file).Send();

            //}
            //catch (System.Exception e)
            //{
            //    Debug.Log(e);
            //}
        }
        //void UploadApk()
        //{
        //    var content = File.ReadAllBytes(UploadFilePath);
        //    if (content == null) return;

        //    var file = new UnityGoogleDrive.Data.File() { Name = Path.GetFileName(UploadFilePath), Content = content };
        //    request = GoogleDriveFiles.CreateResumable(file, resumableSessionUri);
        //    request.Send().OnDone += SaveSessionUri;
        //}    
    }

}