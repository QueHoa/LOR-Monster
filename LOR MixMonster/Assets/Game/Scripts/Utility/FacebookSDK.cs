using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacebookSDK : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!FB.IsInitialized)
        {
            FB.Init();
        }
    }

}
