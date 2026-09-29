using Brows.Diagnostics;
using System.Windows;

namespace Brows.Windows.Controls;

/// <summary>
/// Displays a process's output and status in a WPF control.
/// </summary>
public sealed partial class ProcessProxyView {
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
    public string ProcessDateTimeFormat{
        get => GetValue(ProcessDateTimeFormatProperty) as string;
        set => SetValue(ProcessDateTimeFormatProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessProxyView"/> class.
    /// </summary>
    public ProcessProxyView() {
        InitializeComponent();
    }
}
