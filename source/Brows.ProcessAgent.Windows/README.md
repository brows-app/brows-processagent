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

The default template displays completed and current output lines, distinguishing standard output from standard error. Below the output, a status area shows the process command, start time, process ID, and exit code. While the process has no exit code, the status area shows whether it is starting, running, stopped (it started but ended without an exit code, such as when it was canceled), or not started. Each status item reserves the width of its widest value (a formatted start time, a seven-digit process ID, and the longest state text, "Not started"), so the items don't shift as the process starts and exits. Exit codes of up to about six digits fit in that space; a longer code, such as an NTSTATUS value like -1073741510, widens the exit slot once when the process exits. The start time's reserve is the widest result of `ProcessDateTimeFormat` across all months and weekdays, including the UTC offset of a local time. It is exact for numeric formats and an approximation, based on character count, for formats with month or weekday names in a proportional font. For strict control, or for a custom status template, give the item a fixed width yourself.

Customize the output with `ProcessLineItemsControlTemplate`, `ProcessStandardOutputLineStyle`, and `ProcessStandardErrorLineStyle`. Customize the status area with `ProcessBorderStyle`, `ProcessDateTimeFormat`, and the status templates `ProcessCommandTemplate`, `ProcessStartTimeTemplate`, `ProcessIDTemplate`, `ProcessExitCodeTemplate`, and `ProcessStatusSeparatorTemplate`. Each status template's data context is the `IProcessProxy`:

```xml
<brows:ProcessProxyView ProcessProxy="{Binding ProcessProxy}">
  <brows:ProcessProxyView.ProcessExitCodeTemplate>
    <DataTemplate>
      <TextBlock MinWidth="60" Text="{Binding ExitCode, TargetNullValue=…}" />
    </DataTemplate>
  </brows:ProcessProxyView.ProcessExitCodeTemplate>
</brows:ProcessProxyView>
```

## Style the control

The default template is defined in the package's theme resources, so `ProcessProxyView` is styled like any other lookless WPF control. To reuse the package styles in your own resource dictionary, merge `/Brows.ProcessAgent.Windows;component/Resources/Styles.xaml` and base your style on the implicit `ProcessProxyView` style:

```xml
<ResourceDictionary.MergedDictionaries>
  <ResourceDictionary Source="/Brows.ProcessAgent.Windows;component/Resources/Styles.xaml"/>
</ResourceDictionary.MergedDictionaries>

<Style x:Key="ConsoleProcessStyle" TargetType="brows:ProcessProxyView"
       BasedOn="{StaticResource {x:Type brows:ProcessProxyView}}">
  <Setter Property="Background" Value="Black"/>
  <Setter Property="Foreground" Value="White"/>
  <Setter Property="ProcessDateTimeFormat" Value="HH:mm:ss"/>
</Style>
```

Set `Template` to replace the default template entirely.
