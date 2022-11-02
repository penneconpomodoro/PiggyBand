using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PomodoroInputSystemWrapper : MonoBehaviour
{
    public enum KeyCode
    {
        GamepadDpadLeft,
        GamepadDpadRight,
        GamepadDpadDown,
        GamepadDpadUp,
        GamepadButtonSouth
    }
    private static Dictionary<KeyCode, int> keyPressedCounter = new();
    private static Dictionary<KeyCode, int> oldKeyPressedCounter = new();

    // Start is called before the first frame update
    void Start()
    {
        foreach (KeyCode v in Enum.GetValues(typeof(KeyCode)))
        {
            keyPressedCounter.Add(v, 0);
            oldKeyPressedCounter.Add(v, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Gamepad.current != null)
        {
            foreach (KeyCode v in Enum.GetValues(typeof(KeyCode)))
            {
                oldKeyPressedCounter[v] = keyPressedCounter[v];
            }

            if (Gamepad.current.dpad.left.isPressed) { keyPressedCounter[KeyCode.GamepadDpadLeft]++; } else { keyPressedCounter[KeyCode.GamepadDpadLeft] = 0; }
            if (Gamepad.current.dpad.right.isPressed) { keyPressedCounter[KeyCode.GamepadDpadRight]++; } else { keyPressedCounter[KeyCode.GamepadDpadRight] = 0; }
            if (Gamepad.current.dpad.down.isPressed) { keyPressedCounter[KeyCode.GamepadDpadDown]++; } else { keyPressedCounter[KeyCode.GamepadDpadDown] = 0; }
            if (Gamepad.current.dpad.up.isPressed) { keyPressedCounter[KeyCode.GamepadDpadUp]++; } else { keyPressedCounter[KeyCode.GamepadDpadUp] = 0; }
            if (Gamepad.current.buttonSouth.isPressed) { keyPressedCounter[KeyCode.GamepadButtonSouth]++; } else { keyPressedCounter[KeyCode.GamepadButtonSouth] = 0; }
        }
        else
        {
            foreach (KeyCode v in Enum.GetValues(typeof(KeyCode)))
            {
                keyPressedCounter[v] = 0;
                oldKeyPressedCounter[v] = 0;
            }
        }
    }

    public static bool GetKeyDown(KeyCode k)
    {
        return keyPressedCounter[k] > 0 && oldKeyPressedCounter[k] == 0;
    }

    public static bool GetKey(KeyCode k)
    {
        return keyPressedCounter[k] > 0;
    }
    public static bool GetKeyUp(KeyCode k)
    {
        return keyPressedCounter[k] == 0 && oldKeyPressedCounter[k] > 0;
    }
}