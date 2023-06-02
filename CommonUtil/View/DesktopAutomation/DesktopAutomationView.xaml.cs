using WindowsInput.Events;

namespace CommonUtil.View;

public partial class DesktopAutomationView : ResponsivePage {
    private class AutomationDialogItem {
        public Type DialogType { get; }
        public DesktopAutomationDialog? Dialog { get; set; }

        public AutomationDialogItem(Type dialogType) {
            DialogType = dialogType;
        }
    }

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty AutomationItemsProperty = DependencyProperty.Register("AutomationItems", typeof(IList<AutomationItem>), typeof(DesktopAutomationView), new PropertyMetadata());
    public static readonly DependencyProperty AutomationStepsProperty = DependencyProperty.Register("AutomationSteps", typeof(ExtendedObservableCollection<AutomationStep>), typeof(DesktopAutomationView), new PropertyMetadata());
    public static readonly DependencyProperty StepsIntervalTimeProperty = DependencyProperty.Register("StepsIntervalTime", typeof(double), typeof(DesktopAutomationView), new PropertyMetadata(100.0));
    public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register("IsRunning", typeof(bool), typeof(DesktopAutomationView), new PropertyMetadata(false));
    public static readonly DependencyProperty CurrentMousePositionProperty = DependencyProperty.Register("CurrentMousePosition", typeof(Point), typeof(DesktopAutomationView), new PropertyMetadata());
    private static readonly IReadOnlyDictionary<uint, AutomationDialogItem> AutomationItemDialogDict = new Dictionary<uint, AutomationDialogItem>() {
        {1, new (typeof(InputTextDialog)) },
        {2, new (typeof(PressKeyDialog)) },
        {3, new (typeof(PressKeyShortcutDialog)) },
        {4, new (typeof(MouseClickDialog)) },
        {5, new (typeof(MouseDoubleClickDialog)) },
        {6, new (typeof(MouseMoveDialog)) },
        {7, new (typeof(MouseScrollDialog)) },
        {8, new (typeof(WaitDialog)) },
    };

    /// <summary>
    /// 当前鼠标位置
    /// </summary>
    public Point CurrentMousePosition {
        get { return (Point)GetValue(CurrentMousePositionProperty); }
        set { SetValue(CurrentMousePositionProperty, value); }
    }
    /// <summary>
    /// 是否正在运行
    /// </summary>
    public bool IsRunning {
        get { return (bool)GetValue(IsRunningProperty); }
        set { SetValue(IsRunningProperty, value); }
    }
    /// <summary>
    /// 步骤运行间隔时间 (ms)
    /// </summary>
    public double StepsIntervalTime {
        get { return (double)GetValue(StepsIntervalTimeProperty); }
        set { SetValue(StepsIntervalTimeProperty, value); }
    }
    /// <summary>
    /// 执行步骤
    /// </summary>
    public ExtendedObservableCollection<AutomationStep> AutomationSteps {
        get { return (ExtendedObservableCollection<AutomationStep>)GetValue(AutomationStepsProperty); }
        set { SetValue(AutomationStepsProperty, value); }
    }
    /// <summary>
    /// 菜单
    /// </summary>
    public IList<AutomationItem> AutomationItems {
        get { return (IList<AutomationItem>)GetValue(AutomationItemsProperty); }
        private set { SetValue(AutomationItemsProperty, value); }
    }
    /// <summary>
    /// 更新当前鼠标位置定时器
    /// </summary>
    private readonly DispatcherTimer UpdateCurrentMousePositionTimer = new() {
        Interval = TimeSpan.FromMilliseconds(250),
    };

    public DesktopAutomationView() {
        AutomationSteps = new();
        // Place before InitializeComponent();
        InitAutomationItems();
        InitializeComponent();
        UpdateCurrentMousePositionTimer.Tick += (_, _) => {
            var newPosition = DesktopAutomation.CurrentMousePosition;
            if (newPosition != CurrentMousePosition) {
                CurrentMousePosition = newPosition;
            }
        };
        UpdateCurrentMousePositionTimer.Start();
    }

    protected override void IsExpandedPropertyChangedHandler(ResponsiveLayout self, DependencyPropertyChangedEventArgs e) {
        if (e.NewValue is true) {
            GridPanel.RowDefinitions.Clear();
            GridPanel.ColumnDefinitions.Add(new() {
                Width = new(1, GridUnitType.Star)
            });
            GridPanel.ColumnDefinitions.Add(new() {
                Width = new(2, GridUnitType.Star)
            });
        } else {
            GridPanel.ColumnDefinitions.Clear();
            GridPanel.RowDefinitions.Add(new() {
                Height = new(1, GridUnitType.Star)
            });
            GridPanel.RowDefinitions.Add(new() {
                Height = new(1, GridUnitType.Star)
            });
        }
    }

    /// <summary>
    /// 初始化 AutomationItems
    /// </summary>
    private void InitAutomationItems() {
        AutomationItems = new List<AutomationItem> {
            new AutomationItem("键盘", "\ue629",isFolder: true) {
                Children = new List<AutomationItem> {
                    new AutomationItem("键入文本", "\ue627", id: 1),
                    new AutomationItem("按下键盘", "\ue62a", id: 2),
                    new AutomationItem("输入快捷键", "\ue62a", id: 3),
                }
            },
            new AutomationItem("鼠标", "\ue651",isFolder: true) {
                Children = new List<AutomationItem> {
                    new AutomationItem("单击鼠标", "\ue645", id:4),
                    new AutomationItem("双击鼠标", "\ue60e", id:5),
                    new AutomationItem("移动鼠标", "\ue648", id:6),
                    new AutomationItem("滚动鼠标", "\ue628", id:7),
                }
            },
            new AutomationItem("等待", "\ue642", id:8),
        };
    }

    /// <summary>
    /// 双击鼠标
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MenuMouseDoubleClickHandler(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        if (e.ChangedButton != MouseButton.Left) {
            return;
        }
        if (e.OriginalSource is not FrameworkElement element
            || element.DataContext is not AutomationItem automationItem
            || automationItem.IsFolder
        ) {
            return;
        }
        var dialog = EnsureDialogIsCreated(AutomationItemDialogDict[automationItem.Id]);
        // 确定
        if (await dialog.ShowAsync() == ModernWpf.Controls.ContentDialogResult.Primary) {
            AutomationSteps.Add(new(automationItem.Id, dialog.AutomationMethod) {
                Parameters = dialog.Parameters,
                Icon = automationItem.Icon,
                DescriptionHeader = dialog.DescriptionHeader,
                DescriptionValue = dialog.DescriptionValue,
            });
        }
    }

    /// <summary>
    /// 双击步骤
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AutomationStepsListBoxMouseDoubleClickHandler(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        if (e.OriginalSource is FrameworkElement element && element.DataContext is AutomationStep step) {
            ModifyAutomationStep(step);
        }
    }

    /// <summary>
    /// 修改 AutomationStep
    /// </summary>
    /// <param name="step"></param>
    private async void ModifyAutomationStep(AutomationStep step) {
        var dialog = EnsureDialogIsCreated(AutomationItemDialogDict[step.AutomationItemId]);
        dialog.ParseParameters(step.Parameters);
        // 确定
        if (await dialog.ShowAsync() == ModernWpf.Controls.ContentDialogResult.Primary) {
            step.Parameters = dialog.Parameters;
            step.DescriptionValue = dialog.DescriptionValue;
        }
    }

    /// <summary>
    /// 开始运行
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void StartClickHandler(object sender, RoutedEventArgs e) {
        // Empty list
        if (!AutomationSteps.Any()) {
            return;
        }
        // Already started
        if (IsRunning) {
            return;
        }
        IsRunning = true;
        var builder = BuildSteps();
        // Start
        var isSuccessful = await TaskUtils.TryAsync(() => DesktopAutomation.RunAsync(builder));
        IsRunning = false;
        // Failed
        if (!isSuccessful) {
            MessageBoxUtils.Error("执行失败");
        }
    }

    /// <summary>
    /// 构建步骤
    /// </summary>
    /// <returns></returns>
    private EventBuilder BuildSteps() {
        var builder = DesktopAutomation.NewEventBuilder;
        // Build steps
        if (StepsIntervalTime == 0) {
            foreach (var step in AutomationSteps) {
                step.AutomationMethod.DynamicInvoke(builder, step.Parameters);
            }
        } else {
            for (int i = 0; i < AutomationSteps.Count - 1; i++) {
                _BuildEventStep(builder, AutomationSteps[i]);
                DesktopAutomation.Wait(builder, (uint)StepsIntervalTime);
            }
            // The last
            _BuildEventStep(builder, AutomationSteps[^1]);
        }
        DesktopAutomation.CancelOnEscape(builder);
        return builder;

        static void _BuildEventStep(EventBuilder builder, AutomationStep step) {
            // Create arguments object
            var paramList = new object[1 + step.Parameters.Length];
            paramList[0] = builder;
            step.Parameters.CopyTo(paramList, 1);
            step.AutomationMethod.DynamicInvoke(paramList);
        }
    }

    /// <summary>
    /// 删除步骤
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteExecutedHandler(object sender, ExecutedRoutedEventArgs e) {
        e.Handled = true;
        if (sender is ListBox listBox) {
            AutomationSteps.RemoveList(
                listBox.SelectedItems.Cast<AutomationStep>().ToArray()
            );
        }
    }

    /// <summary>
    /// CanExecute
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>Set CanExecute to true</remarks>
    private void CanExecuteHandler(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

    /// <summary>
    /// 修改步骤
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ModifyAutomationStepClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element && element.DataContext is AutomationStep step) {
            ModifyAutomationStep(step);
        }
    }

    /// <summary>
    /// 确保 Dialog 已创建
    /// </summary>
    /// <param name="dialogItem"></param>
    /// <returns></returns>
    private DesktopAutomationDialog EnsureDialogIsCreated(AutomationDialogItem dialogItem) {
        dialogItem.Dialog ??= (DesktopAutomationDialog)Activator.CreateInstance(dialogItem.DialogType)!;
        return dialogItem.Dialog;
    }
}
