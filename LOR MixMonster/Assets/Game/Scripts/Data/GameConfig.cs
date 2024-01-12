using System;
using UnityEngine;
using UnityEditor;
using Unity.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

[CreateAssetMenu (menuName ="GameConfig/Config")]
public partial class GameConfig:ScriptableObject
{
    public bool editMode = true;
  
    public bool debugMode=false;
    public bool skipAd=false;
  
    public AdConfig adConfig;
    public int bundleAdRequire=10;

    //
    public int gameType = 0;
    public int newItemMax=1;

    [Serializable]
    public class Vector
    {
        public float x, y, z;

        public Vector()
        {
        }

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3 Vector3()
        {
            return new Vector3(x, y, z);
        }
    }
 
    [System.Serializable]
    public class CustomColor
    {
        public float r, g, b,a;

        public CustomColor()
        {
        }

        public Color ToColor()
        {
            return new Color(r, g, b, a);
        }
    }
  

    [Serializable]
    public class AdConfig
    {
        public int adStart =2, interAdCoolDown = 45;
        public bool openAd=true;
        public int adBetweenMakeOver=2;
    }
}
[System.Serializable]
public class RewardData
{
    public ItemData.Category category; 
    public ObscuredString itemId;
    public string icon;
    public string title;
    public ObscuredInt total;
    public int weight = 10;

    public RewardData()
    {
    }
}