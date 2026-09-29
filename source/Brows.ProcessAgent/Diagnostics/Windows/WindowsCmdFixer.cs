using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Brows.Diagnostics.Windows;

/// <summary>
/// Routes eligible missing-command process starts through the Windows command processor.
/// </summary>
/// <remarks>
/// This fixer handles Windows process starts that fail with Win32 error code 2 and use a
/// bare executable name other than <c>cmd.exe</c>. It replaces the executable name with
/// <c>cmd.exe</c> and prefixes the existing arguments with <c>/C</c>. Other failures and
/// executable names are left unchanged. A canceled token is observed before the start
/// information or error is inspected.
/// </remarks>
public sealed class WindowsCmdFixer : IProcessStartInfoFixer {
    async Task<bool> IProcessStartInfoFixer.Fix(ProcessStartInfo startInfo, Exception error, CancellationToken token) {
        if (token.IsCancellationRequested) {
            token.ThrowIfCancellationRequested();
        }
#if !NETFRAMEWORK
#if !NET
        if (Environment.OSVersion.Platform != PlatformID.Win32NT){
            return false;
        }
#else
        if (OperatingSystem.IsWindows() != true) {
            return false;
        }
#endif
#endif
        if (startInfo is null) {
            return false;
        }
        if (error is not Win32Exception win32) {
            return false;
        }
        if (win32.NativeErrorCode != 2) {
            return false;
        }
        var fileName = startInfo.FileName;
        if (fileName == "cmd.exe") {
            return false;
        }
        var fileNameContainsSep = fileName.Contains(Path.DirectorySeparatorChar) ||
                                  fileName.Contains(Path.AltDirectorySeparatorChar);
        if (fileNameContainsSep) {
            return false;
        }
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = $"/C {fileName} {startInfo.Arguments}";
        return true;
    }
}
