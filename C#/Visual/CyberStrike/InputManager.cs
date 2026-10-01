using System.Collections.Generic;
using System.Windows.Forms;

namespace CyberStrike;

public static class InputManager
{
    private static readonly Dictionary<Keys, bool> _keys = new();

    public static void SetKeyState(Keys key, bool isPressed)
    {
        _keys[key] = isPressed;
    }

    public static bool IsKeyPressed(Keys key)
    {
        return _keys.TryGetValue(key, out bool pressed) && pressed;
    }

    public static void Clear()
    {
        _keys.Clear();
    }
}
