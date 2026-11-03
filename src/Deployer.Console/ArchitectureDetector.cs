using System;
using System.Runtime.InteropServices;

namespace Deployer.Console
{
    /// <summary>
    /// v4 Core Architecture detector for AMD Shell, ARM, and CoreOS-style deployments.
    /// Supports Windows-to-Lumia applying and multi-arch shell.
    /// </summary>
    public static class ArchitectureDetector
    {
        public static string GetArchitecture()
        {
            var arch = RuntimeInformation.OSArchitecture;
            var processArch = RuntimeInformation.ProcessArchitecture;
            
            return $"OS: {arch}, Process: {processArch}, Is64Bit: {Environment.Is64BitProcess}";
        }

        public static bool IsAmd64() => RuntimeInformation.ProcessArchitecture == Architecture.X64;
        
        public static bool IsArm64() => RuntimeInformation.ProcessArchitecture == Architecture.Arm64;

        public static string GetShellMode()
        {
            if (IsAmd64()) return "AMD64 Shell";
            if (IsArm64()) return "ARM64 Shell";
            return "Generic Shell";
        }

        public static void LogArchitectureInfo()
        {
            Console.WriteLine($"[v4] Architecture: {GetArchitecture()}");
            Console.WriteLine($"[v4] Shell Mode: {GetShellMode()}");
            Console.WriteLine("[v4] CoreOS architecture support: modular (ready for CoreOS/Fedora CoreOS image apply)");
            Console.WriteLine("[v4] Windows-to-Lumia apply: enhanced partitioning + driver injection available");
        }
    }
}
