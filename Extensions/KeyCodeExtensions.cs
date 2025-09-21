using UnityEngine;

namespace iiMenu.Extensions
{
    public static class KeyCodeExtensions
    {
        public static string Key(this KeyCode key)
        {
            if (key >= KeyCode.A && key <= KeyCode.Z)
                return key.ToString(); // Returns "A", "B", etc.

            if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
                return ((char)('0' + (key - KeyCode.Alpha0))).ToString();

            if (key == KeyCode.Space)
                return " ";

            if (key == KeyCode.Return || key == KeyCode.KeypadEnter)
                return "\n";

            if (key == KeyCode.Tab)
                return "\t";

            switch (key)
            {
                case KeyCode.Comma: return ",";
                case KeyCode.Period: return ".";
                case KeyCode.Slash: return "/";
                case KeyCode.Backslash: return "\\";
                case KeyCode.Minus: return "-";
                case KeyCode.Equals: return "=";
                case KeyCode.Semicolon: return ";";
                case KeyCode.Quote: return "'";
                case KeyCode.LeftBracket: return "[";
                case KeyCode.RightBracket: return "]";
                default: return ""; // Unknown key
            }
        }
    }
}
