namespace CommonUtil.View;

public partial class CSharpDependencyView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(CSharpDependencyView), new PropertyMetadata(""));
    public static readonly DependencyProperty TypeInfosProperty = DependencyProperty.Register("TypeInfos", typeof(ObservableCollection<TypeInfo>), typeof(CSharpDependencyView), new PropertyMetadata());
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(CSharpDependencyView), new PropertyMetadata(true));

    /// <summary>
    /// 输出文本
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }
    public ObservableCollection<TypeInfo> TypeInfos {
        get { return (ObservableCollection<TypeInfo>)GetValue(TypeInfosProperty); }
        private set { SetValue(TypeInfosProperty, value); }
    }
    /// <summary>
    /// 是否扩宽
    /// </summary>
    public bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }

    public CSharpDependencyView() {
        TypeInfos = new();
        InitializeComponent();
        #region 响应式布局
        UIUtils.SetLoadedOnceEventHandler(this, (_, _) => {
            Window window = Window.GetWindow(this);
            double expansionThreshold = (double)Resources["ExpansionThreshold"];
            IsExpanded = window.ActualWidth >= expansionThreshold;
            DependencyPropertyDescriptor
                .FromProperty(Window.ActualWidthProperty, typeof(Window))
                .AddValueChanged(window, (_, _) => {
                    IsExpanded = window.ActualWidth >= expansionThreshold;
                });
        });
        #endregion
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        MessageBox.Success("已复制");
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        OutputText = string.Empty;
        TypeInfos = new();
    }

    /// <summary>
    /// 添加属性
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddPropertyClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        TypeInfos.Add(TypeInfos.Any() ? TypeInfos[^1] with { } : new());
    }

    /// <summary>
    /// 生成代码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GenerateCodeClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        OutputText = CSharpDependencyGenerator.CreateTemplate(TypeInfos);
    }

    /// <summary>
    /// 移除属性
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RemovePropertyClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        TypeInfoListBox.SelectedItems
            .Cast<TypeInfo>()
            .ToList()
            .ForEach(info => {
                TypeInfos.Remove(info);
            });
    }
}
