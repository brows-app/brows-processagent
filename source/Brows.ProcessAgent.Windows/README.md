# Brows.ProcessAgent.Windows

A WPF control for displaying a process's standard output and standard error using `IProcessProxy` from `Brows.ProcessAgent`.

Install the package with `dotnet add package Brows.ProcessAgent.Windows`.

This package targets `net10.0-windows` and includes WPF support.

## Display process output

Add the Brows XAML namespace and bind `ProcessProxy` to an `IProcessProxy` created by `ProcessAgent`:

```xml
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:brows="http://schemas.brows.app/winfx/2026/xaml/presentation">
  <brows:ProcessProxyView ProcessProxy="{Binding ProcessProxy}" />
</Window>
```

The default template displays completed and current output lines, distinguishing standard output from standard error. It also shows the process command, start time, process ID, and exit code. Customize the line-item template, output styles, border style, and date-time format with the control's corresponding properties.
