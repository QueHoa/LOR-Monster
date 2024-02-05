using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace IAP
{
    public class Controller : IDetailedStoreListener, IStoreListener
    {
        public static Controller Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Controller();
                }
                return instance;
            }

            set
            {
                if (instance == null)
                {
                    instance = value;
                }
            }

        }
        public Dictionary<string, float> productPrices = new Dictionary<string, float>();
        private static Controller instance;

        public delegate void OnPurchased(Product product);
        public static OnPurchased onPurchased;

        private IStoreController controller;
        private IExtensionProvider extensions;
        ConfigurationBuilder builder;
        List<PurchaseButton> buttons = new List<PurchaseButton>();

        IAPPackageDataSO catalog;
        public static bool isReady = false;
        bool isInit = false;
        public Controller()
        {
        }
        public void InitProduct()
        {
            GameUtility.GameUtility.Log("INIT PRODUCT IAP");
            if (isInit) return;
            isInit = true;
            StandardPurchasingModule module = StandardPurchasingModule.Instance();


            builder = ConfigurationBuilder.Instance(module);
#if UNITY_ANDROID
            catalog = Resources.Load<IAPPackageDataSO>("IAP_Android");
#elif UNITY_IOS
            catalog = Resources.Load<IAPPackageDataSO>("IAP_IOS");
#endif
            foreach (IAPPackageDataSO.IAPPackage product in catalog.products)
            {
                builder.AddProduct(product.id, product.type);
                productPrices.Add(product.id, product.price);
            }

            UnityPurchasing.Initialize(this, builder);
            GameUtility.GameUtility.Log("INIT PRODUCT IAP finish");

        }



        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.controller = controller;
            this.extensions = extensions;

            isReady = true;
            GameUtility.GameUtility.Log("ON IAP INITIALIZED");
        }
        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            GameUtility.GameUtility.Log("ON IAP OnInitializeFailed " + error + " " + message);

        }
        public void AddButton(PurchaseButton button)
        {
            if (!buttons.Contains(button))
            {
                buttons.Add(button);
            }
        }
        public void RemoveButton(PurchaseButton button)
        {
            buttons.Remove(button);
        }
        public Product GetProduct(string productID)
        {
            if (controller != null && controller.products != null && !string.IsNullOrEmpty(productID))
            {
                return controller.products.WithID(productID);
            }
            GameUtility.GameUtility.LogError("CodelessIAPStoreListener attempted to get unknown product " + productID);
            return null;
        }

        public T GetStoreConfiguration<T>() where T : IStoreConfiguration
        {
            return builder.Configure<T>();
        }

        public T GetStoreExtensions<T>() where T : IStoreExtension
        {
            return extensions.GetExtension<T>();
        }
        public bool HasProductInCatalog(string productID)
        {
            foreach (var product in catalog.products)
            {
                if (product.id == productID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool InitiatePurchase(string productID)
        {
            if (!catalog.isAvailable)
            {
                foreach (var button in buttons)
                {
                    if (button.productId == productID)
                    {
                        button.OnPurchaseFailed(null, PurchaseFailureReason.PurchasingUnavailable);
                        break;
                    }
                }

                return false;
            }
            if (controller == null)
            {
                GameUtility.GameUtility.LogError("Purchase failed because Purchasing was not initialized correctly");

                foreach (var button in buttons)
                {
                    if (button.productId == productID)
                    {
                        button.OnPurchaseFailed(null, PurchaseFailureReason.PurchasingUnavailable);
                        break;
                    }
                }
                return false;
            }

            controller.InitiatePurchase(productID);
            return true;
        }




        /// <summary>
        /// Implementation of <typeparamref name="UnityEngine.Purchasing.IStoreListener.OnInitializeFailed"/> which
        /// logs the failure reason.
        /// </summary>
        /// <param name="error">Reported in the app log</param>
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            GameUtility.GameUtility.LogError(string.Format("Purchasing failed to initialize. Reason: {0}", error.ToString()));
        }

        /// <summary>
        /// Implementation of <typeparamref name="UnityEngine.Purchasing.IStoreListener.ProcessPurchase"/> which forwards
        /// this successful purchase event to any appropriate registered <typeparamref name="IAPButton"/>s and
        /// <typeparamref name="IAPListener"/>s. Logs an error if there are no appropriate registered handlers.
        /// </summary>
        /// <param name="e">Data for this purchase</param>
        /// <returns>Any indication of whether this purchase has been completed by any of my appropriate registered
        /// <typeparamref name="IAPButton"/>s or <typeparamref name="IAPListener"/>s</returns>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            PurchaseProcessingResult result;

            // if any receiver consumed this purchase we return the status
            bool consumePurchase = false;
            bool resultProcessed = false;
            GameUtility.GameUtility.Log("PROCESS PURCHASE");
            PurchaseButton[] buttons = this.buttons.ToArray();
            foreach (var button in buttons)
            {
                GameUtility.GameUtility.Log("PROCESS PURCHASE " + button.gameObject.name + " " + button.productId + " " + e.purchasedProduct.definition.id);
                if (button.productId.Equals(e.purchasedProduct.definition.id))
                {
                    result = button.ProcessPurchase(e);

                    if (result == PurchaseProcessingResult.Complete)
                    {

                        consumePurchase = true;
                    }

                    resultProcessed = true;
                }
            }

            // we expect at least one receiver to get this message
            if (!resultProcessed)
            {

                GameUtility.GameUtility.LogError("Purchase not correctly processed for product \"" +
                                 e.purchasedProduct.definition.id +
                                 "\". Add an active IAPButton to process this purchase, or add an IAPListener to receive any unhandled purchase events.");

            }
            else
            {
                //onPurchased?.Invoke(e.purchasedProduct);
                var product = e.purchasedProduct;
                Debug.LogWarning("purchase thanh cong, ban event len firebase");
                FirebaseAnalysticController.Instance.LogIAP(product, productPrices[product.definition.id]);
            }
            return PurchaseProcessingResult.Complete;
            //return (consumePurchase) ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            OnPurchaseFailed(product, failureDescription.reason);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            bool resultProcessed = false;

            foreach (var button in buttons)
            {
                if (button.productId == product.definition.id)
                {
                    button.OnPurchaseFailed(product, failureReason);

                    resultProcessed = true;
                }
            }

            // we expect at least one receiver to get this message
            if (!resultProcessed)
            {

                GameUtility.GameUtility.LogError("Failed purchase not correctly handled for product \"" + product.definition.id +
                                  "\". Add an active IAPButton to handle this failure, or add an IAPListener to receive any unhandled purchase failures.");
            }
        }
    }
}