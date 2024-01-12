using Firebase;
using Firebase.Crashlytics;
using UnityEngine;

public class FirebaseCrashlyticsInit : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

        FirebaseManager.onInit -= OnInit;
        FirebaseManager.onInit += OnInit;
    }
    void OnInit()
    {
        Crashlytics.ReportUncaughtExceptionsAsFatal = false;
        FirebaseManager.onInit -= OnInit;
        Debug.Log("INIT crashlystic");
    }

}