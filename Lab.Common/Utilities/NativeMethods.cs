using System;
using System.Runtime.InteropServices;

namespace Lab.Common.Utilities
{
    internal static class NativeMethods
    {
        private const string Kernel32DllName = "kernel32.dll";

        [DllImport(Kernel32DllName)]
        internal static extern bool AllocConsole();

        [DllImport(Kernel32DllName)]
        internal static extern bool FreeConsole();

        [DllImport(Kernel32DllName)]
        internal static extern IntPtr GetConsoleWindow();
    }
}
