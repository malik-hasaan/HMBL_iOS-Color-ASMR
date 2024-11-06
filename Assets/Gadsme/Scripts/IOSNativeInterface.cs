using System;
using System.Runtime.InteropServices;

#if UNITY_IOS && !UNITY_EDITOR
namespace Gadsme.Core.Native
{
    class IOSNativeInterface: IIOSNativeInterface
    {
        [DllImport("__Internal")]
        public static extern IntPtr GDSPluginGetUpdateTextureCallback();

        [DllImport("__Internal")]
        public static extern void GDSPluginSetMessageCallback(IntPtr nativeMessageCallback);

        [DllImport("__Internal")]
        public static extern IntPtr GDSWebTexture_new(int width, int height);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_setGadsmeBridgeJavascript(string gadsmeBridgeJavascript);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_setGadsmeSDKVersion(string sdkVersion);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_setMediaBlockingRuleEnabled(bool enabled);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_setRemoteDebugEnabled(bool enabled);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_setRemoteDebugBoolConfig(string name, bool value);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_setRemoteDebugStringConfig(string name, string value);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_setRemoteDebugDoubleConfig(string name, double value);

        [DllImport("__Internal")]
        public static extern int GDSWebTexture_getInstanceId(IntPtr webViewTexturePtr);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_setUserAgent(IntPtr webViewTexturePtr, string userAgent);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_loadUrl(IntPtr webViewTexturePtr, string url);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_loadHtml(IntPtr webViewTexturePtr, string html);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_setViewable(IntPtr webViewTexturePtr, bool viewable);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_render(IntPtr webViewTexturePtr);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_click(IntPtr webViewTexturePtr, int posX, int posY);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_setSkAdNetworkData(IntPtr webViewTexturePtr, string skAdNetworkDataString);

        [DllImport("__Internal")]
        public static extern void GDSWebTexture_destroy(IntPtr webViewTexturePtr);

        [DllImport("__Internal")]
        public static extern void GDSAdPresenter_presentURL(string url, string adNetworkData);

        [DllImport("__Internal")]
        public static extern int GDSAdvertisingIdentifier_request(bool allowConsentDialog);

        [DllImport("__Internal")]
        public static extern IntPtr GDSVendorIdentifier_getIdentifier();

        [DllImport("__Internal")]
        public static extern IntPtr GDSSKAdNetwork_getAdNetworkIds();

        [DllImport("__Internal")]
        public static extern int GDSUserAgent_request();

        [DllImport("__Internal")]
        public static extern IntPtr GDSTCF_getConsentString();

        [DllImport("__Internal")]
        public static extern float GDSAudio_requestAudioVolume();

        public IntPtr PluginGetUpdateTextureCallback() {
            return GDSPluginGetUpdateTextureCallback();
        }

        public void PluginSetMessageCallback(IntPtr nativeMessageCallback) {
            GDSPluginSetMessageCallback(nativeMessageCallback);
        }

        public IntPtr WebTexture_new(int width, int height) {
            return GDSWebTexture_new(width, height);
        }

        public void WebTexture_setGadsmeBridgeJavascript(string gadsmeBridgeJavascript) {
            GDSWebTexture_setGadsmeBridgeJavascript(gadsmeBridgeJavascript);
        }

        public void WebTexture_setGadsmeSDKVersion(string sdkVersion) {
            GDSWebTexture_setGadsmeSDKVersion(sdkVersion);
        }

        public void WebTexture_setMediaBlockingRuleEnabled(bool enabled) {
            GDSWebTexture_setMediaBlockingRuleEnabled(enabled);
        }

        public void WebTexture_setRemoteDebugEnabled(bool enabled) {
            GDSWebTexture_setRemoteDebugEnabled(enabled);
        }

        public void WebTexture_setRemoteDebugBoolConfig(string name, bool value) {
            GDSWebTexture_setRemoteDebugBoolConfig(name, value);
        }

        public void WebTexture_setRemoteDebugStringConfig(string name, string value) {
            GDSWebTexture_setRemoteDebugStringConfig(name, value);
        }

        public void WebTexture_setRemoteDebugDoubleConfig(string name, double value) {
            GDSWebTexture_setRemoteDebugDoubleConfig(name, value);
        }

        public int WebTexture_getInstanceId(IntPtr webViewTexturePtr) {
            return GDSWebTexture_getInstanceId(webViewTexturePtr);
        }

        public void WebTexture_setUserAgent(IntPtr webViewTexturePtr, string userAgent) {
            GDSWebTexture_setUserAgent(webViewTexturePtr, userAgent);
        }

        public void WebTexture_loadUrl(IntPtr webViewTexturePtr, string url) {
            GDSWebTexture_loadUrl(webViewTexturePtr, url);
        }

        public void WebTexture_loadHtml(IntPtr webViewTexturePtr, string html) {
            GDSWebTexture_loadHtml(webViewTexturePtr, html);
        }

        public void WebTexture_setViewable(IntPtr webViewTexturePtr, bool viewable) {
            GDSWebTexture_setViewable(webViewTexturePtr, viewable);
        }

        public void WebTexture_render(IntPtr webViewTexturePtr) {
            GDSWebTexture_render(webViewTexturePtr);
        }

        public void WebTexture_click(IntPtr webViewTexturePtr, int posX, int posY) {
            GDSWebTexture_click(webViewTexturePtr, posX, posY);
        }

        public void WebTexture_setSkAdNetworkData(IntPtr webViewTexturePtr, string skAdNetworkDataString) {
            GDSWebTexture_setSkAdNetworkData(webViewTexturePtr, skAdNetworkDataString);
        }

        public void WebTexture_destroy(IntPtr webViewTexturePtr) {
            GDSWebTexture_destroy(webViewTexturePtr);
        }

        public void AdPresenter_presentURL(string url, string adNetworkData) {
            GDSAdPresenter_presentURL(url, adNetworkData);
        }

        public int AdvertisingIdentifier_request(bool allowConsentDialog) {
            return GDSAdvertisingIdentifier_request(allowConsentDialog);
        }

        public IntPtr VendorIdentifier_getIdentifier() {
            return GDSVendorIdentifier_getIdentifier();
        }

        public IntPtr SKAdNetwork_getAdNetworkIds() {
            return GDSSKAdNetwork_getAdNetworkIds();
        }

        public int UserAgent_request() {
            return GDSUserAgent_request();
        }

        public IntPtr TCF_getConsentString() {
            return GDSTCF_getConsentString();
        }

        public float Audio_requestAudioVolume() {
            return GDSAudio_requestAudioVolume();
        }
    }
}
#endif