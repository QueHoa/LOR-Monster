using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class IAPPackageDataSO:ScriptableObject
{
    [System.Serializable]
    public struct IAPPackage
    {
        public string id;
        public float price;
        public UnityEngine.Purchasing.ProductType type;
    }
    public bool isAvailable = true;
    public List<IAPPackage> products;



}