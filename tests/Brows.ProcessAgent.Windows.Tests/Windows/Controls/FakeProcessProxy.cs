using Brows.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;

namespace Brows.Windows.Controls;

/// <summary>
/// A settable <see cref="IProcessProxy"/> that raises property-change notifications.
/// </summary>
internal sealed class FakeProcessProxy : IProcessProxy, INotifyPropertyChanged {
    private void Change<T>(ref T field, T value, [CallerMemberName] string name = null) {
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public bool Running { get; set => Change(ref field, value); }
    public int? ProcessID { get; set => Change(ref field, value); }
    public int? ExitCode { get; set => Change(ref field, value); }
    public DateTime? ExitTime { get; set => Change(ref field, value); }
    public DateTime? StartTime { get; set => Change(ref field, value); }
    public bool Started { get; set => Change(ref field, value); }
    public bool Starting { get; set => Change(ref field, value); }
    public string FileName { get; set => Change(ref field, value); } = "tool.exe";
    public string Arguments { get; set => Change(ref field, value); } = "--flag";
    public string WorkingDirectory => null;
    public string Domain => null;
    public bool? LoadUserProfile => null;
    public string UserName => null;
    public SecureString Password => null;
    public string PasswordInClearText => null;
    public string Verb => null;
    public IDictionary<string, string> Environment => null;
    public IProcessStream Stream => null;
    public void Kill() { }

    /// <summary>
    /// Sets the state of a process that is running.
    /// </summary>
    public void Run(int processID = 4624, DateTime? startTime = null) {
        ProcessID = processID;
        StartTime = startTime ?? new DateTime(2026, 11, 15, 15, 4, 5);
        Started = true;
        Running = true;
    }

    /// <summary>
    /// Sets the state of a process that has exited with the given code.
    /// </summary>
    public void Exit(int exitCode) {
        ExitCode = exitCode;
        ExitTime = new DateTime(2026, 11, 15, 15, 5, 5);
        Running = false;
    }

    /// <summary>
    /// Sets the state of a process that was canceled after it started: it is
    /// no longer running and has no exit code.
    /// </summary>
    public void Cancel() {
        Running = false;
    }
}
