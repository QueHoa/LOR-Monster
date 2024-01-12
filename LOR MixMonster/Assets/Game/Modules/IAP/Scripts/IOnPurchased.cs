using UnityEngine.Purchasing;

public interface IOnPurchased
{

    void OnPurchaseCompleted(Product product);
    void OnPurchaseFailed(Product product, PurchaseFailureReason reason);
}