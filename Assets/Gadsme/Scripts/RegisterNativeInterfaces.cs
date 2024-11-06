using UnityEngine;
using Gadsme.Core.Native;

namespace Gadsme
{
    class RegisterNativeInterface
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void registerInterface()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        GadsmeSDK.RegisterAndroidInterface(new AndroidNativeInterface());
#elif UNITY_IOS && !UNITY_EDITOR
        GadsmeSDK.RegisterIOSInterface(new IOSNativeInterface());
#elif (UNITY_STANDALONE_OSX && !UNITY_EDITOR) || UNITY_EDITOR_OSX
            GadsmeSDK.RegisterMacInterface(new MacNativeInterface());
#endif
        }
    }
}