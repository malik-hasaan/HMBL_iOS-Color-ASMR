
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

namespace Gadsme
{
    [Preserve]
    public class GadsmeInputBackend : IGadsmeInputBackend
    {
#if ENABLE_INPUT_SYSTEM
        bool pointerPressed = false;
        bool pointerJustPressed = false;
        bool pointerJustReleased = false;
        Vector2 pointerPosition = Vector2.zero;
#endif

        public GadsmeInputBackend()
        {
#if ENABLE_INPUT_SYSTEM
            GadsmeDebug.Log("Init Gadsme input (Input System)");
#else
            GadsmeDebug.Log("Init Gadsme input (Legacy Input Manager)");
#endif
        }

        public void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (Touchscreen.current != null)
            {
                var pointerWasPressed = pointerPressed;
                pointerPressed = Touchscreen.current.primaryTouch.press.isPressed;
                pointerJustPressed = pointerPressed && !pointerWasPressed;
                pointerJustReleased = !pointerPressed && pointerWasPressed;
                if (pointerPressed || pointerWasPressed)
                {
                    pointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                }
                else if (Mouse.current != null)
                {
                    pointerPosition = Mouse.current.position.ReadValue();
                }
            }
            else if (Mouse.current != null)
            {
                pointerPressed = Mouse.current.leftButton.isPressed;
                pointerJustPressed = Mouse.current.leftButton.wasPressedThisFrame;
                pointerJustReleased = Mouse.current.leftButton.wasReleasedThisFrame;
                pointerPosition = Mouse.current.position.ReadValue();
            }
            else
            {
                pointerPressed = false;
                pointerJustPressed = false;
                pointerJustReleased = false;
            }
#endif
        }

        public bool PointerJustPressed()
        {
#if ENABLE_INPUT_SYSTEM
            return pointerJustPressed;
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

        public bool PointerJustReleased()
        {
#if ENABLE_INPUT_SYSTEM
            return pointerJustReleased;
#else
            return Input.GetMouseButtonUp(0);
#endif
        }

        public Vector2 PointerPosition()
        {
#if ENABLE_INPUT_SYSTEM
            return pointerPosition;
#else
            return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#endif
        }

        public GameObject CreateEventSystem()
        {
#if ENABLE_INPUT_SYSTEM
            return new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
#else
            return new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
#endif
        }
    }
}