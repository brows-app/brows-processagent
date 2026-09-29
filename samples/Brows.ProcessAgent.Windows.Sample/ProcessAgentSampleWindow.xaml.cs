using Brows.Diagnostics;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace Brows;

public partial class ProcessAgentSampleWindow : Window {
    private CancellationTokenSource _runCancellation;
    private bool _closeWhenRunCompletes;

    public ProcessAgentSampleWindow() {
        InitializeComponent();
        WorkingDirectoryTextBox.Text = Environment.CurrentDirectory;
        FileNameTextBox.Text = "cmd.exe";
        ArgumentsTextBox.Text = "/c echo ProcessAgent standard output & echo ProcessAgent standard error 1>&2";
    }

    private async void RunButton_Click(object sender, RoutedEventArgs e) {
        if (_runCancellation is not null) {
            return;
        }

        var fileName = FileNameTextBox.Text.Trim();
        if (fileName.Length == 0) {
            StatusTextBlock.Text = "Enter an executable.";
            return;
        }

        var workingDirectory = WorkingDirectoryTextBox.Text.Trim();
        var arguments = ArgumentsTextBox.Text;
        var cancellation = new CancellationTokenSource();
        _runCancellation = cancellation;

        RunButton.IsEnabled = false;
        CancelButton.IsEnabled = true;
        ProcessProxyView.ProcessProxy = null;
        StatusTextBlock.Text = "Starting process...";

        try {
            var agent = new ProcessAgent {
                FileName = fileName,
                Arguments = string.IsNullOrWhiteSpace(arguments) ? null : arguments,
                WorkingDirectory = workingDirectory.Length == 0 ? null : workingDirectory,
                OnProxyCreated = proxy => ProcessProxyView.ProcessProxy = proxy,
            };
            var proxy = await agent.Start(cancellation.Token);
            StatusTextBlock.Text = $"Process exited with code {proxy.ExitCode?.ToString() ?? "unknown"}.";
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested) {
            StatusTextBlock.Text = "Process canceled.";
        }
        catch (Exception exception) {
            StatusTextBlock.Text = $"Process failed: {exception.Message}";
        }
        finally {
            _runCancellation = null;
            cancellation.Dispose();
            RunButton.IsEnabled = true;
            CancelButton.IsEnabled = false;

            if (_closeWhenRunCompletes) {
                _closeWhenRunCompletes = false;
                Close();
            }
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) {
        _runCancellation?.Cancel();
        CancelButton.IsEnabled = false;
        StatusTextBlock.Text = "Canceling process...";
    }

    private void ProcessAgentSampleWindow_Closing(object sender, CancelEventArgs e) {
        if (_runCancellation is not null) {
            e.Cancel = true;
            _closeWhenRunCompletes = true;
            _runCancellation.Cancel();
        }
    }
}
