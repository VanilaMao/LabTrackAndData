using System;
using System.Diagnostics;
using System.IO;
using System.Security;

namespace Lab.Common.Utilities
{
    /// <summary>
    /// Provides methods to show and hide the console.  Only available in debug mode.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        public static bool HasConsole => NativeMethods.GetConsoleWindow() != IntPtr.Zero;

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        [Conditional("DEBUG")]
        public static void Show()
        {
            if (HasConsole) return;
            NativeMethods.AllocConsole();
            InvalidateOutAndError();
        }

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        [Conditional("DEBUG")]
        public static void Hide()
        {
            if (!HasConsole) return;
            SetOutAndErrorNull();
            NativeMethods.FreeConsole();
        }

        /// <summary>
        /// Toggles the console.
        /// </summary>
        [Conditional("DEBUG")]
        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private static void InvalidateOutAndError()
        {
            var type = typeof(Console);

            var _out = type.GetField("_out",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            var error = type.GetField("_error",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            var initializeStdOutError = type.GetMethod("InitializeStdOutError",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Debug.Assert(_out != null);
            Debug.Assert(error != null);

            Debug.Assert(initializeStdOutError != null);

            _out.SetValue(null, null);
            error.SetValue(null, null);
            initializeStdOutError.Invoke(null, new object[] { true });
        }

        private static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
}
