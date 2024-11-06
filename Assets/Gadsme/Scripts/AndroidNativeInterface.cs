using System;
using System.Runtime.InteropServices;

#if UNITY_ANDROID && !UNITY_EDITOR
namespace Gadsme.Core.Native
{
    class AndroidNativeInterface: IAndroidNativeInterface
    {
        [DllImport("GadsmeAndroidPlugin")]
        public static extern IntPtr GDSPluginGetUpdateTextureCallback();

        [DllImport("GadsmeAndroidPlugin")]
        public static extern void GDSPluginSetMessageCallback(IntPtr nativeMessageCallback);
        
        public IntPtr PluginGetUpdateTextureCallback()
        {
            return GDSPluginGetUpdateTextureCallback();
        }

        public void PluginSetMessageCallback(IntPtr nativeMessageCallback)
        {
            GDSPluginSetMessageCallback(nativeMessageCallback);
        }
    }
}
#endif