﻿using ModernWpf.Controls;

namespace CommonUtil.View;

public partial class InputTextDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(InputTextDialog), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 输入文本
    /// </summary>
    public string InputText {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
    }

    public InputTextDialog() {
        AutomationMethod = DesktopAutomation.InputText;
        Title = DescriptionHeader = "输入文本";
        InitializeComponent();
        this.EnableAutoResize(
            (double)FindResource("ContentDialogMinWidth"),
            (double)FindResource("ContentDialogMaxWidth"),
            3
        );
    }

    /// <summary>
    /// Set values
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="e"></param>
    private void ClosingHandler(ContentDialog dialog, ContentDialogClosingEventArgs e) {
        _ = dialog;
        _ = e;
        Parameters = new object[] { InputText };
        DescriptionValue = InputText;
    }

    public override void ParseParameters(object[] parameters) {
        InputText = (string?)parameters[0] ?? string.Empty;
    }
}
