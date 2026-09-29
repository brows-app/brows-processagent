using Brows.Diagnostics;
using System.Windows;

namespace Brows.Windows.Controls;

public sealed partial class ProcessProxyView {
    private static DependencyProperty RegisterDependencyProperty(
        string name,
        Type propertyType) => DependencyProperty.Register(
            name,
            propertyType,
            typeof(ProcessProxyView),
            new PropertyMetadata(defaultValue: null));

    public static readonly DependencyProperty ProcessProxyProperty =
        RegisterDependencyProperty(nameof(ProcessProxy), typeof(IProcessProxy));

    public IProcessProxy ProcessProxy {
        get => GetValue(ProcessProxyProperty) as IProcessProxy;
        set => SetValue(ProcessProxyProperty, value);
    }

    public static readonly DependencyProperty ProcessBorderStyleProperty =
        RegisterDependencyProperty(nameof(ProcessBorderStyle), typeof(Style));

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

    public static readonly DependencyProperty ProcessStandardErrorLineStyleProperty =
        RegisterDependencyProperty(nameof(ProcessStandardErrorLineStyle), typeof(Style));

    public Style ProcessStandardErrorLineStyle {
        get => GetValue(ProcessStandardErrorLineStyleProperty) as Style;
        set => SetValue(ProcessStandardErrorLineStyleProperty, value);
    }

    public static readonly DependencyProperty ProcessStandardOutputLineStyleProperty =
        RegisterDependencyProperty(nameof(ProcessStandardOutputLineStyle), typeof(Style));

    public Style ProcessStandardOutputLineStyle {
        get => GetValue(ProcessStandardOutputLineStyleProperty) as Style;
        set => SetValue(ProcessStandardOutputLineStyleProperty, value);
    }

    public static readonly DependencyProperty ProcessDateTimeFormatProperty =
        RegisterDependencyProperty(nameof(ProcessDateTimeFormat), typeof(string));

    public string ProcessDateTimeFormat{
        get => GetValue(ProcessDateTimeFormatProperty) as string;
        set => SetValue(ProcessDateTimeFormatProperty, value);
    }

    public ProcessProxyView() {
        InitializeComponent();
    }
}
