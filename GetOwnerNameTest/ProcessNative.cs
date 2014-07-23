using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GetOwnerNameTest
{
    static public class ProcessNative
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
    }
}
