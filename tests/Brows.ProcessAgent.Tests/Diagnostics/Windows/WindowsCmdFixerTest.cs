using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Brows.Diagnostics.Windows;

[TestFixture]
internal sealed class WindowsCmdFixerTest {
    private static readonly IProcessStartInfoFixer Fixer = new WindowsCmdFixer();

    private static bool IsWindows {
#if NETFRAMEWORK
        get => true;
#else
        get => OperatingSystem.IsWindows();
#endif
    }

    [Test]
    public async Task Fix_RewritesMissingBareExecutableThroughCommandProcessor() {
        const string fileName = "missing-tool.exe";
        const string arguments = "--flag \"two words\"";
        var startInfo = new ProcessStartInfo(fileName, arguments);

        var result = await Fixer.Fix(startInfo, new Win32Exception(2), CancellationToken.None);

        using (Assert.EnterMultipleScope()) {
            Assert.That(result, Is.EqualTo(IsWindows));
            Assert.That(startInfo.FileName, Is.EqualTo(IsWindows ? "cmd.exe" : fileName));
            Assert.That(startInfo.Arguments,
                Is.EqualTo(IsWindows ? $"/C {fileName} {arguments}" : arguments));
        }
    }

    [TestCase("cmd.exe")]
    [TestCase(@"relative\tool.exe")]
    [TestCase("relative/tool.exe")]
    public async Task Fix_DoesNotRewriteCommandProcessorOrPathExecutable(string fileName) {
        const string arguments = "--flag";
        var startInfo = new ProcessStartInfo(fileName, arguments);

        var result = await Fixer.Fix(startInfo, new Win32Exception(2), CancellationToken.None);

        using (Assert.EnterMultipleScope()) {
            Assert.That(result, Is.False);
            Assert.That(startInfo.FileName, Is.EqualTo(fileName));
            Assert.That(startInfo.Arguments, Is.EqualTo(arguments));
        }
    }

    [TestCase(1)]
    [TestCase(5)]
    public async Task Fix_DoesNotRewriteForOtherWin32Errors(int errorCode) {
        const string fileName = "missing-tool.exe";
        const string arguments = "--flag";
        var startInfo = new ProcessStartInfo(fileName, arguments);

        var result = await Fixer.Fix(startInfo, new Win32Exception(errorCode), CancellationToken.None);

        using (Assert.EnterMultipleScope()) {
            Assert.That(result, Is.False);
            Assert.That(startInfo.FileName, Is.EqualTo(fileName));
            Assert.That(startInfo.Arguments, Is.EqualTo(arguments));
        }
    }

    [Test]
    public async Task Fix_DoesNotRewriteForOtherErrorsOrNullStartInfo() {
        var startInfo = new ProcessStartInfo("missing-tool.exe", "--flag");

        var otherErrorResult = await Fixer.Fix(
            startInfo,
            new InvalidOperationException(),
            CancellationToken.None);
        var nullStartInfoResult = await Fixer.Fix(
            null,
            new Win32Exception(2),
            CancellationToken.None);

        using (Assert.EnterMultipleScope()) {
            Assert.That(otherErrorResult, Is.False);
            Assert.That(nullStartInfoResult, Is.False);
            Assert.That(startInfo.FileName, Is.EqualTo("missing-tool.exe"));
            Assert.That(startInfo.Arguments, Is.EqualTo("--flag"));
        }
    }

    [Test]
    public void Fix_ThrowsWhenCancellationIsRequested() {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var startInfo = new ProcessStartInfo("missing-tool.exe", "--flag");

        Assert.ThrowsAsync<OperationCanceledException>(
            async () => await Fixer.Fix(startInfo, new Win32Exception(2), cancellation.Token));

        using (Assert.EnterMultipleScope()) {
            Assert.That(startInfo.FileName, Is.EqualTo("missing-tool.exe"));
            Assert.That(startInfo.Arguments, Is.EqualTo("--flag"));
        }
    }
}
