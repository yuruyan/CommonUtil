namespace CommonUtil.View;

public partial class CodeColorizationView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyPropertyKey LanguagesPropertyKey = DependencyProperty.RegisterReadOnly("Languages", typeof(ExtendedObservableCollection<string>), typeof(CodeColorizationView), new PropertyMetadata());
    public static readonly DependencyProperty LanguagesProperty = LanguagesPropertyKey.DependencyProperty;

    /// <summary>
    /// 语言
    /// </summary>
    public ExtendedObservableCollection<string> Languages => (ExtendedObservableCollection<string>)GetValue(LanguagesProperty);

    public CodeColorizationView() {
        SetValue(LanguagesPropertyKey, new ExtendedObservableCollection<string>());
        // 后台加载
        Dispatcher.BeginInvoke(() => {
            var languages = CodeColorization.Languages.ToArray();
            Array.Sort(languages);
            Languages.AddRange(languages);
        });
        InitializeComponent();
        LanguageComboBox.SelectedValue = "C#";
        TextEditor.Options.ConvertTabsToSpaces = true;
        // 设置 SyntaxHighlighting
        ThemeManager.Current.ThemeChanged += (_, mode) => SetCurrentSyntaxHighlighting(mode);
    }

    /// <summary>
    /// 设置当前 SyntaxHighlighting
    /// </summary>
    /// <param name="themeMode"></param>
    private void SetCurrentSyntaxHighlighting(ThemeMode themeMode) {
        if (LanguageComboBox.SelectedValue is object value) {
            TextEditor.SyntaxHighlighting = CodeColorization.GetHighlighting(
                value.ToString()!,
                themeMode
            );
        }
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        TextEditor.SelectAll();
        TextEditor.Copy();
        MessageBoxUtils.Success("已复制");
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        TextEditor.Text = string.Empty;
    }

    /// <summary>
    /// 选择语言
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LanguageComboBoxSelectionChangedHandler(object sender, SelectionChangedEventArgs e) {
        e.Handled = true;
        SetCurrentSyntaxHighlighting(ThemeManager.Current.CurrentMode);
    }
}
