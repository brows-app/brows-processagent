using Brows.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Brows.Windows.Controls;

[TestFixture]
[Apartment(ApartmentState.STA)]
internal sealed class ProcessProxyViewTest {
    private const double ViewWidth = 900;

    private static ProcessProxyView Render(IProcessProxy proxy, string dateTimeFormat = null) {
        var view = new ProcessProxyView();
        // A control outside a window resolves its theme style when initialization ends.
        view.BeginInit();
        view.ProcessDateTimeFormat = dateTimeFormat;
        view.ProcessProxy = proxy;
        view.EndInit();
        Layout(view);
        return view;
    }

    private static void Layout(ProcessProxyView view) {
        view.ApplyTemplate();
        view.Measure(new Size(ViewWidth, 200));
        view.Arrange(new Rect(0, 0, ViewWidth, 200));
        view.UpdateLayout();
    }

    private static bool Shown(DependencyObject element, DependencyObject root) {
        for (var current = element; current is not null && !ReferenceEquals(current, root); current = VisualTreeHelper.GetParent(current)) {
            if (current is UIElement ui && ui.Visibility != Visibility.Visible) {
                return false;
            }
        }
        return true;
    }

    private static IEnumerable<TextBlock> TextBlocks(DependencyObject parent) {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is TextBlock text) {
                yield return text;
            }
            foreach (var descendant in TextBlocks(child)) {
                yield return descendant;
            }
        }
    }

    private static List<string> VisibleTexts(ProcessProxyView view) {
        return TextBlocks(view)
            .Where(text => Shown(text, view))
            .Select(text => text.Text.Trim())
            .Where(text => text.Length > 0)
            .ToList();
    }

    private static double LeftOf(ProcessProxyView view, string text) {
        var block = TextBlocks(view).Single(t => t.Text == text);
        return block.TranslatePoint(new Point(), view).X;
    }

    // Where the text visibly ends. A block that stretches to fill its slot draws left-aligned
    // text, so its desired width rather than its actual width is the width of the text.
    private static double RightOf(ProcessProxyView view, TextBlock block) {
        return block.TranslatePoint(new Point(), view).X + block.DesiredSize.Width;
    }

    private static List<TextBlock> VisibleTextBlocks(ProcessProxyView view) {
        return TextBlocks(view)
            .Where(text => Shown(text, view) && text.Text.Trim().Length > 0)
            .ToList();
    }

    // The exit slot's content: the "Exit" label for an exit code, otherwise the state text.
    private static TextBlock ExitSlotStart(ProcessProxyView view) {
        var texts = VisibleTextBlocks(view);
        return texts.SingleOrDefault(text => text.Text == "Exit") ?? texts.Last();
    }

    [Test]
    public void Layout_ExitStatusStartsAtTheSamePositionInEveryState() {
        var proxy = new FakeProcessProxy();
        var view = Render(proxy);
        var starts = new List<(string State, double Left)> {
            ("not started", ExitSlotStart(view).TranslatePoint(new Point(), view).X)
        };

        proxy.Run();
        Layout(view);
        starts.Add(("running", ExitSlotStart(view).TranslatePoint(new Point(), view).X));

        proxy.Cancel();
        Layout(view);
        starts.Add(("stopped", ExitSlotStart(view).TranslatePoint(new Point(), view).X));

        proxy.Exit(3);
        Layout(view);
        starts.Add(("exit 3", ExitSlotStart(view).TranslatePoint(new Point(), view).X));

        var first = starts[0].Left;
        foreach (var (state, left) in starts) {
            Assert.That(left, Is.EqualTo(first).Within(0.5), $"The exit status started at a different position when the state was '{state}'.");
        }
    }

    [TestCase(3)]
    [TestCase(-1073741510)]
    public void Layout_ExitCodeIsAdjacentToItsLabel(int exitCode) {
        AssertExitCodeIsAdjacentToItsLabel(exitCode);
    }

    // Some cultures, such as sv-SE, use U+2212 as the minus sign. The control formats with its
    // own language, so the test must not depend on the thread's culture.
    [Test]
    [SetCulture("sv-SE")]
    public void Layout_ExitCodeIsAdjacentToItsLabelWhenTheCurrentCultureUsesADifferentMinusSign() {
        AssertExitCodeIsAdjacentToItsLabel(-1073741510);
    }

    private static void AssertExitCodeIsAdjacentToItsLabel(int exitCode) {
        var proxy = new FakeProcessProxy();
        proxy.Run();
        proxy.Exit(exitCode);
        var view = Render(proxy);

        var texts = VisibleTextBlocks(view);
        var label = texts.Single(text => text.Text == "Exit");
        // Bindings format with the element's language rather than the thread's culture.
        var value = texts.Single(text => text.Text == exitCode.ToString(view.Language.GetSpecificCulture()));
        // The label's desired width includes its margin, which is the intended space before the value.
        var gap = value.TranslatePoint(new Point(), view).X - RightOf(view, label);

        Assert.That(gap, Is.EqualTo(0).Within(0.5));
    }

    [Test]
    public void Layout_ProcessIDAndStartTimeAreRightAlignedInTheirSlots() {
        var proxy = new FakeProcessProxy();
        var view = Render(proxy, dateTimeFormat: "HH:mm:ss");
        // Before startup, the start time and the process ID each show a placeholder.
        var placeholders = VisibleTextBlocks(view).Where(text => text.Text == "—").ToList();
        var startTimePlaceholderRight = RightOf(view, placeholders[0]);
        var processIDPlaceholderRight = RightOf(view, placeholders[1]);

        proxy.Run(processID: 4624, startTime: new DateTime(2026, 11, 15, 15, 4, 5));
        Layout(view);
        var startTimeRight = RightOf(view, VisibleTextBlocks(view).Single(text => text.Text == "15:04:05"));
        var processIDRight = RightOf(view, VisibleTextBlocks(view).Single(text => text.Text == "4624"));

        using (Assert.EnterMultipleScope()) {
            Assert.That(startTimeRight, Is.EqualTo(startTimePlaceholderRight).Within(0.5));
            Assert.That(processIDRight, Is.EqualTo(processIDPlaceholderRight).Within(0.5));
        }
    }

    [Test]
    public void Template_IsAppliedFromThemeResources() {
        var view = Render(new FakeProcessProxy());

        Assert.That(VisualTreeHelper.GetChildrenCount(view), Is.GreaterThan(0));
    }

    [Test]
    public void Status_ProcessThatNeverStartedShowsNotStarted() {
        var view = Render(new FakeProcessProxy());

        Assert.That(VisibleTexts(view), Does.Contain("Not started"));
    }

    [Test]
    public void Status_StartingProcessShowsStarting() {
        var view = Render(new FakeProcessProxy { Starting = true });

        var texts = VisibleTexts(view);
        using (Assert.EnterMultipleScope()) {
            Assert.That(texts, Does.Contain("Starting"));
            Assert.That(texts, Does.Not.Contain("Not started"));
        }
    }

    [Test]
    public void Status_RunningProcessShowsRunning() {
        var proxy = new FakeProcessProxy();
        proxy.Run();
        var view = Render(proxy);

        var texts = VisibleTexts(view);
        using (Assert.EnterMultipleScope()) {
            Assert.That(texts, Does.Contain("Running"));
            Assert.That(texts, Does.Not.Contain("Not started"));
        }
    }

    [Test]
    public void Status_ExitCodeWinsOverRunning() {
        var proxy = new FakeProcessProxy();
        proxy.Run();
        proxy.ExitCode = 3;
        var view = Render(proxy);

        var texts = VisibleTexts(view);
        using (Assert.EnterMultipleScope()) {
            Assert.That(texts, Does.Contain("Exit"));
            Assert.That(texts, Does.Contain("3"));
            Assert.That(texts, Does.Not.Contain("Running"));
        }
    }

    [Test]
    public void Status_CanceledAfterStartDoesNotShowNotStartedOrRunning() {
        var proxy = new FakeProcessProxy();
        proxy.Run(processID: 4624);
        proxy.Cancel();
        var view = Render(proxy);

        var texts = VisibleTexts(view);
        using (Assert.EnterMultipleScope()) {
            Assert.That(texts, Does.Not.Contain("Not started"));
            Assert.That(texts, Does.Not.Contain("Running"));
            Assert.That(texts, Does.Contain("Stopped"));
            Assert.That(texts, Does.Contain("4624"));
        }
    }

    [Test]
    public void Status_ProcessCanceledWhileViewIsDisplayedUpdatesTheStatus() {
        var proxy = new FakeProcessProxy();
        var view = Render(proxy);
        proxy.Run();
        Layout(view);
        Assert.That(VisibleTexts(view), Does.Contain("Running"));

        proxy.Cancel();
        Layout(view);

        var texts = VisibleTexts(view);
        using (Assert.EnterMultipleScope()) {
            Assert.That(texts, Does.Contain("Stopped"));
            Assert.That(texts, Does.Not.Contain("Running"));
        }
    }

    [Test]
    public void StartTime_AlreadyStartedProxyUsesTheConfiguredFormatWhenFirstDisplayed() {
        var proxy = new FakeProcessProxy();
        proxy.Run(startTime: new DateTime(2026, 11, 15, 15, 4, 5));
        var view = Render(proxy, dateTimeFormat: "HH:mm:ss");

        Assert.That(VisibleTexts(view), Does.Contain("15:04:05"));
    }

    [Test]
    public void StartTime_ChangingTheFormatUpdatesTheDisplayedTime() {
        var proxy = new FakeProcessProxy();
        proxy.Run(startTime: new DateTime(2026, 11, 15, 15, 4, 5));
        var view = Render(proxy, dateTimeFormat: "HH:mm:ss");

        view.ProcessDateTimeFormat = "hh:mm";
        Layout(view);

        var texts = VisibleTexts(view);
        using (Assert.EnterMultipleScope()) {
            Assert.That(texts, Does.Contain("03:04"));
            Assert.That(texts, Does.Not.Contain("15:04:05"));
        }
    }

    [Test]
    public void StartTime_ProxyThatStartsAfterTheViewUsesTheConfiguredFormat() {
        var proxy = new FakeProcessProxy();
        var view = Render(proxy, dateTimeFormat: "HH:mm:ss");

        proxy.Run(startTime: new DateTime(2026, 11, 15, 15, 4, 5));
        Layout(view);

        Assert.That(VisibleTexts(view), Does.Contain("15:04:05"));
    }

    [Test]
    public void StartTime_WithoutAFormatUsesTheGeneralFormat() {
        var proxy = new FakeProcessProxy();
        var startTime = new DateTime(2026, 11, 15, 15, 4, 5);
        proxy.Run(startTime: startTime);
        var view = Render(proxy);

        Assert.That(
            VisibleTexts(view),
            Does.Contain(startTime.ToString("G", CultureInfo.GetCultureInfo("en-US"))));
    }

    [Test]
    public void Layout_StatusItemsDoNotMoveAsTheProcessStartsAndExits() {
        var proxy = new FakeProcessProxy();
        var view = Render(proxy, dateTimeFormat: "HH:mm:ss");
        var positions = new List<(string State, double Left)> {
            ("not started", LeftOf(view, "Started"))
        };

        proxy.Run(processID: 4624);
        Layout(view);
        positions.Add(("running", LeftOf(view, "Started")));

        proxy.Exit(0);
        Layout(view);
        positions.Add(("exit 0", LeftOf(view, "Started")));

        // Typical exit codes fit in the space reserved for the widest status text. A much
        // longer code, such as an NTSTATUS value, widens the exit slot once.
        proxy.Exit(255);
        Layout(view);
        positions.Add(("exit 255", LeftOf(view, "Started")));

        var first = positions[0].Left;
        foreach (var (state, left) in positions) {
            Assert.That(left, Is.EqualTo(first).Within(0.5), $"The status items moved when the state was '{state}'.");
        }
    }

    [TestCase("D")]
    [TestCase("MMMM")]
    [TestCase("dddd")]
    [TestCase("O")]
    [TestCase("F")]
    public void Layout_StatusItemsDoNotMoveForFormatsWhoseWidthVaries(string format) {
        // The widest month and weekday names in en-US, as a local time as `ProcessProxy` records it.
        var startTime = new DateTime(2026, 9, 30, 15, 4, 5, System.DateTimeKind.Local);
        var proxy = new FakeProcessProxy();
        var view = Render(proxy, dateTimeFormat: format);
        var before = LeftOf(view, "Started");

        proxy.Run(startTime: startTime);
        Layout(view);

        Assert.That(LeftOf(view, "Started"), Is.EqualTo(before).Within(0.5));
    }

    [Test]
    public void Layout_StatusItemsDoNotMoveWhenAProcessIsStoppedAfterItStarted() {
        var proxy = new FakeProcessProxy();
        var view = Render(proxy);
        proxy.Run();
        Layout(view);
        var running = LeftOf(view, "Started");

        proxy.Cancel();
        Layout(view);

        Assert.That(LeftOf(view, "Started"), Is.EqualTo(running).Within(0.5));
    }
}
