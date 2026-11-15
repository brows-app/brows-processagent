using Brows.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Brows.Windows.Controls;

/// <summary>
/// Displays a process's output and status in a WPF control.
/// </summary>
public sealed class ProcessProxyView : Control {
    static ProcessProxyView() {
        DefaultStyleKeyProperty.OverrideMetadata(
            forType: typeof(ProcessProxyView),
            typeMetadata: new FrameworkPropertyMetadata(typeof(ProcessProxyView)));
    }

    private static DependencyProperty RegisterDependencyProperty(
        string name,
        Type propertyType) => DependencyProperty.Register(
            name,
            propertyType,
            typeof(ProcessProxyView),
            new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="ProcessProxy"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProcessProxyProperty =
        RegisterDependencyProperty(nameof(ProcessProxy), typeof(IProcessProxy));

    /// <summary>
    /// Gets or sets the process proxy whose output and status are displayed.
    /// </summary>
    public IProcessProxy ProcessProxy {
        get => GetValue(ProcessProxyProperty) as IProcessProxy;
        set => SetValue(ProcessProxyProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ProcessBorderStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProcessBorderStyleProperty =
        RegisterDependencyProperty(nameof(ProcessBorderStyle), typeof(Style));

    /// <summary>
    /// Gets or sets the style applied to the border around process information.
    /// </summary>
    public Style ProcessBorderStyle {
        get => GetValue(ProcessBorderStyleProperty) as Style;
        set => SetValue(ProcessBorderStyleProperty, value);
    }

    /// <summary>Identifies the <see cref="ProcessLineItemsControlTemplate"/> dependency property.</summary>
    public static readonly DependencyProperty ProcessLineItemsControlTemplateProperty =
        RegisterDependencyProperty(nameof(ProcessLineItemsControlTemplate), typeof(DataTemplate));

    /// <summary>
    /// Gets or sets the template used to create the control that displays the process line items.
    /// The template's data context is the line-item collection.
    /// </summary>
    public DataTemplate ProcessLineItemsControlTemplate {
        get => GetValue(ProcessLineItemsControlTemplateProperty) as DataTemplate;
        set => SetValue(ProcessLineItemsControlTemplateProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ProcessStandardErrorLineStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProcessStandardErrorLineStyleProperty =
        RegisterDependencyProperty(nameof(ProcessStandardErrorLineStyle), typeof(Style));

    /// <summary>
    /// Gets or sets the style applied to standard error lines.
    /// </summary>
    public Style ProcessStandardErrorLineStyle {
        get => GetValue(ProcessStandardErrorLineStyleProperty) as Style;
        set => SetValue(ProcessStandardErrorLineStyleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ProcessStandardOutputLineStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProcessStandardOutputLineStyleProperty =
        RegisterDependencyProperty(nameof(ProcessStandardOutputLineStyle), typeof(Style));

    /// <summary>
    /// Gets or sets the style applied to standard output lines.
    /// </summary>
    public Style ProcessStandardOutputLineStyle {
        get => GetValue(ProcessStandardOutputLineStyleProperty) as Style;
        set => SetValue(ProcessStandardOutputLineStyleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ProcessDateTimeFormat"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProcessDateTimeFormatProperty =
        RegisterDependencyProperty(nameof(ProcessDateTimeFormat), typeof(string));

    /// <summary>
    /// Gets or sets the format string used to display the process start time.
    /// </summary>
    /// <value>
    /// A date-time format string, or null to use the general (<c>G</c>) format.
    /// </value>
    public string ProcessDateTimeFormat {
        get => GetValue(ProcessDateTimeFormatProperty) as string;
        set => SetValue(ProcessDateTimeFormatProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ProcessCommandTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProcessCommandTemplateProperty =
        RegisterDependencyProperty(nameof(ProcessCommandTemplate), typeof(DataTemplate));

    /// <summary>
    /// Gets or sets the template used to display the process command in the status area.
    /// </summary>
    /// <value>
    /// A template whose data context is the <see cref="IProcessProxy"/>, or null to display the
    /// file name and arguments, trimmed to the available width.
    /// </value>
    public DataTemplate ProcessCommandTemplate {
        get => GetValue(ProcessCommandTemplateProperty) as DataTemplate;
        set => SetValue(ProcessCommandTemplateProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ProcessStartTimeTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProcessStartTimeTemplateProperty =
        RegisterDependencyProperty(nameof(ProcessStartTimeTemplate), typeof(DataTemplate));

    /// <summary>
    /// Gets or sets the template used to display the process start time in the status area.
    /// </summary>
    /// <value>
    /// A template whose data context is the <see cref="IProcessProxy"/>, or null to display the
    /// start time formatted with <see cref="ProcessDateTimeFormat"/>.
    /// </value>
    public DataTemplate ProcessStartTimeTemplate {
        get => GetValue(ProcessStartTimeTemplateProperty) as DataTemplate;
        set => SetValue(ProcessStartTimeTemplateProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ProcessIDTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProcessIDTemplateProperty =
        RegisterDependencyProperty(nameof(ProcessIDTemplate), typeof(DataTemplate));

    /// <summary>
    /// Gets or sets the template used to display the process identifier in the status area.
    /// </summary>
    /// <value>
    /// A template whose data context is the <see cref="IProcessProxy"/>, or null to use the default display.
    /// </value>
    public DataTemplate ProcessIDTemplate {
        get => GetValue(ProcessIDTemplateProperty) as DataTemplate;
        set => SetValue(ProcessIDTemplateProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ProcessExitCodeTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProcessExitCodeTemplateProperty =
        RegisterDependencyProperty(nameof(ProcessExitCodeTemplate), typeof(DataTemplate));

    /// <summary>
    /// Gets or sets the template used to display the process exit code in the status area.
    /// </summary>
    /// <value>
    /// A template whose data context is the <see cref="IProcessProxy"/>, or null to display the
    /// exit code, or the process state while no exit code is available.
    /// </value>
    /// <remarks>
    /// The exit code is null until the process exits. To keep the status area from shifting,
    /// give a custom template a stable width while the exit code is null.
    /// </remarks>
    public DataTemplate ProcessExitCodeTemplate {
        get => GetValue(ProcessExitCodeTemplateProperty) as DataTemplate;
        set => SetValue(ProcessExitCodeTemplateProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ProcessStatusSeparatorTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProcessStatusSeparatorTemplateProperty =
        RegisterDependencyProperty(nameof(ProcessStatusSeparatorTemplate), typeof(DataTemplate));

    /// <summary>
    /// Gets or sets the template used to separate items in the status area.
    /// </summary>
    /// <value>
    /// A template whose data context is the <see cref="IProcessProxy"/>, or null to display a
    /// thin vertical rule.
    /// </value>
    public DataTemplate ProcessStatusSeparatorTemplate {
        get => GetValue(ProcessStatusSeparatorTemplateProperty) as DataTemplate;
        set => SetValue(ProcessStatusSeparatorTemplateProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessProxyView"/> class.
    /// </summary>
    public ProcessProxyView() {
    }
}
