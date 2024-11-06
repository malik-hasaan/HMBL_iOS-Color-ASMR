using System;
using System.Runtime.InteropServices;

#if (UNITY_STANDALONE_OSX && !UNITY_EDITOR) || UNITY_EDITOR_OSX
namespace Gadsme.Core.Native
{
    class MacNativeInterface : IMacNativeInterface
    {
        [DllImport("GadsmeMacPlugin")]
        public static extern IntPtr GDSPluginGetUpdateTextureCallback();

        [DllImport("GadsmeMacPlugin")]
        public static extern void GDSPluginSetMessageCallback(IntPtr nativeMessageCallback);

        [DllImport("GadsmeMacPlugin")]
        public static extern IntPtr GDSWebTexture_new(int width, int height);

        [DllImport("GadsmeMacPlugin")]
        public static extern int GDSWebTexture_getInstanceId(IntPtr webViewTexturePtr);

        [DllImport("GadsmeMacPlugin")]
        public static extern void GDSWebTexture_setUserAgent(IntPtr webViewTexturePtr, string userAgent);

        [DllImport("GadsmeMacPlugin")]
        public static extern void GDSWebTexture_loadUrl(IntPtr webViewTexturePtr, string url);

        [DllImport("GadsmeMacPlugin")]
        public static extern void GDSWebTexture_loadHtml(IntPtr webViewTexturePtr, string html);

        [DllImport("GadsmeMacPlugin")]
        public static extern void GDSWebTexture_render(IntPtr webViewTexturePtr);

        [DllImport("GadsmeMacPlugin")]
        public static extern void GDSWebTexture_click(IntPtr webViewTexturePtr, int posX, int posY);

        [DllImport("GadsmeMacPlugin")]
        public static extern void GDSWebTexture_destroy(IntPtr webViewTexturePtr);

        [DllImport("GadsmeMacPlugin")]
        public static extern int GDSAdvertisingIdentifier_request();

        [DllImport("GadsmeMacPlugin")]
        public static extern int GDSUserAgent_request();

        public IntPtr PluginGetUpdateTextureCallback()
        {
            return GDSPluginGetUpdateTextureCallback();
        }

        public void PluginSetMessageCallback(IntPtr nativeMessageCallback)
        {
            GDSPluginSetMessageCallback(nativeMessageCallback);
        }

        public IntPtr WebTexture_new(int width, int height)
        {
            return GDSWebTexture_new(width, height);
        }

        public int WebTexture_getInstanceId(IntPtr webViewTexturePtr)
        {
            return GDSWebTexture_getInstanceId(webViewTexturePtr);
        }

        public void WebTexture_setUserAgent(IntPtr webViewTexturePtr, string userAgent)
        {
            GDSWebTexture_setUserAgent(webViewTexturePtr, userAgent);
        }

        public void WebTexture_loadUrl(IntPtr webViewTexturePtr, string url)
        {
            GDSWebTexture_loadUrl(webViewTexturePtr, url);
        }

        public void WebTexture_loadHtml(IntPtr webViewTexturePtr, string html)
        {
            GDSWebTexture_loadHtml(webViewTexturePtr, html);
        }

        public void WebTexture_setViewable(IntPtr webViewTexturePtr, bool viewable)
        {
            // TODO?
        }

        public void WebTexture_render(IntPtr webViewTexturePtr)
        {
            GDSWebTexture_render(webViewTexturePtr);
        }

        public void WebTexture_click(IntPtr webViewTexturePtr, int posX, int posY)
        {
            GDSWebTexture_click(webViewTexturePtr, posX, posY);
        }

        public void WebTexture_destroy(IntPtr webViewTexturePtr)
        {
            GDSWebTexture_destroy(webViewTexturePtr);
        }

        public int AdvertisingIdentifier_request()
        {
            return GDSAdvertisingIdentifier_request();
        }

        public int UserAgent_request()
        {
            return GDSUserAgent_request();
        }
    }
}
#endif