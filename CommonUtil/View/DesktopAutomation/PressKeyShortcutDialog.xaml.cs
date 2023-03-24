using ModernWpf.Controls;
using WindowsInput.Events;

namespace CommonUtil.View;

public partial class PressKeyShortcutDialog : DesktopAutomationDialog {
    public class KeyStrokeItem : DependencyObject {
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register("Code", typeof(string), typeof(KeyStrokeItem), new PropertyMetadata(KeyCode.None.ToString()));

        public string Code {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }
    }

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty KeyCodeListProperty = DependencyProperty.Register("KeyCodeList", typeof(ExtendedObservableCollection<KeyStrokeItem>), typeof(PressKeyShortcutDialog), new PropertyMetadata());
    internal static readonly DependencyPropertyKey KeyCodeNameListPropertyKey = DependencyProperty.RegisterReadOnly("KeyCodeNameList", typeof(string[]), typeof(PressKeyShortcutDialog), new PropertyMetadata());
    public static readonly DependencyProperty KeyCodeNameListProperty = KeyCodeNameListPropertyKey.DependencyProperty;

    public string[] KeyCodeNameList => (string[])GetValue(KeyCodeNameListProperty);
    /// <summary>
    /// 按键集合
    /// </summary>
    public ExtendedObservableCollection<KeyStrokeItem> KeyCodeList {
        get { return (ExtendedObservableCollection<KeyStrokeItem>)GetValue(KeyCodeListProperty); }
        set { SetValue(KeyCodeListProperty, value); }
    }

    public PressKeyShortcutDialog() {
        SetValue(KeyCodeNameListPropertyKey, Enum.GetNames<KeyCode>());
        KeyCodeList = new();
        AutomationMethod = DesktopAutomation.PressKeyShortcut;
        Title = DescriptionHeader = "按下快捷键";
        InitializeComponent();
    }

    /// <summary>
    /// CanExecute
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>Set CanExecute to true</remarks>
    private void CanExecuteHandler(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

    /// <summary>
    /// 删除步骤
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteExecutedHandler(object sender, ExecutedRoutedEventArgs e) {
        e.Handled = true;
        if (sender is ListBox listBox) {
            KeyCodeList.RemoveList(listBox.SelectedItems.Cast<KeyStrokeItem>().ToArray());
        }
    }

    /// <summary>
    /// Set values
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="e"></param>
    private void ClosingHandler(ContentDialog dialog, ContentDialogClosingEventArgs e) {
        _ = dialog;
        _ = e;
        Parameters = new object[] {
            KeyCodeList.Select(
                item => Enum.Parse<KeyCode>(item.Code)
            ).ToArray(),
        };
        DescriptionValue = string.Join('+', (KeyCode[])Parameters[0]);
    }

    public override void ParseParameters(object[] parameters) {
        KeyCodeList = new(((KeyCode[])parameters[0]).Select(
            code => new KeyStrokeItem() { Code = code.ToString(), }
        ));
    }

    /// <summary>
    /// 添加按键
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void AddKeyStrokeClickHandler(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
        _ = sender;
        _ = args;
        args.Cancel = true;
        KeyCodeList.Add(new());
    }

    /// <summary>
    /// 设置 ItemsSource
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void KeyCodeListItemLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is ComboBox comboBox) {
            comboBox.ItemsSource = KeyCodeNameList;
        }
    }
}
