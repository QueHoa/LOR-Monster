using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtility
{
    public class ScreenCapture
    {
        public static Texture2D Capture()
        {
            return UnityEngine.ScreenCapture.CaptureScreenshotAsTexture(1);
        }
    }
}