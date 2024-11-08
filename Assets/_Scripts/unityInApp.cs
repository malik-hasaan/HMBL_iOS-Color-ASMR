﻿using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class unityInApp : MonoBehaviour, IStoreListener
{

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.

    public static string kProductIDSubscription = "subscription";

    // Apple App Store-specific product identifier for the subscription product.
    private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

    // Google Play Store-specific product identifier subscription product.
    private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";
    public static unityInApp instance;
    void Start()
    {
        instance = this;
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.

        for (int i = 0; i < AdsIds.instance.InAppIds.Length; i++)
        {
            builder.AddProduct(AdsIds.instance.InAppIds[i].Id, AdsIds.instance.InAppIds[i].Type);
        }

        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        // must only be referenced here. 
        builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
            { kProductNameAppleSubscription, AppleAppStore.Name },
            { kProductNameGooglePlaySubscription, GooglePlay.Name },
        });

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        try
        {
            UnityPurchasing.Initialize(this, builder);
        }
        catch
        {
        }
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    //	public void BuyConsumable(string key)
    //	{
    //		// Buy the consumable product using its general identifier. Expect a response either 
    //		// through ProcessPurchase or OnPurchaseFailed asynchronously.
    //		BuyProductID(key);
    //	}
    //
    //
    //	public void BuyNonConsumable()
    //	{
    //		// Buy the non-consumable product using its general identifier. Expect a response either 
    //		// through ProcessPurchase or OnPurchaseFailed asynchronously.
    //		BuyProductID(kProductIDNonConsumable);
    //	}
    //
    //
    //	public void BuySubscription()
    //	{
    //		// Buy the subscription product using its the general identifier. Expect a response either 
    //		// through ProcessPurchase or OnPurchaseFailed asynchronously.
    //		// Notice how we use the general product identifier in spite of this ID being mapped to
    //		// custom store-specific identifiers above.
    //		BuyProductID(kProductIDSubscription);
    //	}

    int inAppValue;
    public void BuyProductID(string productId, int value)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);
            inAppValue = value;
            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public string[] localizedPrices;

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        try
        {
            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;

            // Fetch and store the localized prices
            for (int i = 0; i < m_StoreController.products.all.Length; i++)
            {
                Product product = m_StoreController.products.all[i];
                if (product != null)
                {
                    localizedPrices[i] = product.metadata.localizedPriceString;
                    Debug.Log($"Product ID: {product.definition.id}, Localized Price: {localizedPrices[i]}");
                }
            }
        }
        catch { }
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    private string m_LastTransationID;
    private string m_LastReceipt;
    private int coinsToAdd = 0;
    private string currentPurchaseID = null;
    List<string> productIdsForRestore;
    private CrossPlatformValidator validator;

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {


        bool validPurchase = true;
        m_LastTransationID = e.purchasedProduct.transactionID;
        m_LastReceipt = e.purchasedProduct.receipt;

        //
        //#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
        //validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
        //try
        //{
        //    var result = validator.Validate(e.purchasedProduct.receipt);

        //    foreach (IPurchaseReceipt productReceipt in result)
        //    {

        //        Debug.Log(productReceipt.productID);
        //        Debug.Log(productReceipt.purchaseDate);
        //        Debug.Log(productReceipt.transactionID);
        //        productIdsForRestore.Add(productReceipt.productID);
        //    }

        //    for (int i = 0; i < productIdsForRestore.Count; i++)
        //    {
        //        for (int j = 0; j < AdsIds.instance.InAppIds.Length; j++)
        //        {
        //            if (AdsIds.instance.InAppIds[j].Id == productIdsForRestore[i])
        //            {
        //                if (AdsIds.instance.InAppIds[j].Type == ProductType.NonConsumable)
        //                {

        //                    AdsIds.instance.AfterPurchased(j);
        //                }
        //                break;
        //            }

        //        }
        //    }
        //}

        //        catch (IAPSecurityException)
        //        {
        //            Debug.Log("Invalid receipt, not unlocking content");
        //            //  validPurchase = false;
        //        }
        //#endif



        if (validPurchase) // remove this sign in IF condition !
        {
            Debug.Log("Purchase is valid");
            if (String.Equals(e.purchasedProduct.definition.id, AdsIds.instance.InAppIds[inAppValue].Id, StringComparison.Ordinal))
            {
                Debug.Log("Product Type : " + e.purchasedProduct.definition.type);
                AdsIds.instance.AfterPurchased(inAppValue);

            }
        }
        return PurchaseProcessingResult.Complete;





    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        //throw new NotImplementedException();
    }
}