using CommonUITools.Route;
using CommonUtil.Core.Model;
using CommonUtil.Route;

namespace CommonUtil.View;

public partial class RandomGeneratorView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(RandomGeneratorView), new PropertyMetadata(""));
    public static readonly DependencyProperty GenerateCountProperty = DependencyProperty.Register("GenerateCount", typeof(double), typeof(RandomGeneratorView), new PropertyMetadata(16.0));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RandomGeneratorView), new PropertyMetadata(true));

    /// <summary>
    /// 输出
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }
    /// <summary>
    /// 生成个数
    /// </summary>
    public double GenerateCount {
        get { return (double)GetValue(GenerateCountProperty); }
        set { SetValue(GenerateCountProperty, value); }
    }
    /// <summary>
    /// 是否扩宽
    /// </summary>
    public bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }

    private readonly Type[] Routers = {
        typeof(RandomStringGeneratorView),
        typeof(RandomNumberGeneratorView),
        typeof(RandomGeneratorWithRegexView),
        typeof(RandomGeneratorWithDataSourceView),
        typeof(RandomChineseNameGeneratorView),
        typeof(RandomChineseFamilyNameGeneratorView),
        typeof(RandomChineseAncientNameGeneratorView),
        typeof(RandomJapaneseNameGeneratorView),
        typeof(RandomJapaneseFamilyNameGeneratorView),
        typeof(RandomEnglishNameGeneratorView),
        typeof(RandomEnglishChineseNameGeneratorView),
        typeof(RandomChineseWordGeneratorView),
        typeof(RandomEnglishWordGeneratorView),
        typeof(RandomEmailAddressGeneratorView),
        typeof(RandomGuidGeneratorView),
        typeof(RandomIPV4AddressGeneratorView),
        typeof(RandomIPV6AddressGeneratorView),
        typeof(RandomMACAddressGeneratorView),
        typeof(RandomDateTimeGeneratorView),
    };
    private readonly RouterService RouterService;

    public RandomGeneratorView() {
        InitializeComponent();
        RouterService = new(ContentFrame, Routers);
        NavigationUtils.EnableNavigation(
            NavigationView,
            RouterService,
            ContentFrame
        );
        #region 响应式布局
        NavigationUtils.EnableNavigationPanelResponsive(NavigationView);
        DependencyPropertyDescriptor
            .FromProperty(IsExpandedProperty, this.GetType())
            .AddValueChanged(this, (_, _) => {
                if (IsExpanded) {
                    SecondRowDefinition.Height = new(0);
                    SecondColumnDefinition.Width = new(1, GridUnitType.Star);
                    Grid.SetColumn(OutputTextBox, 1);
                    Grid.SetRow(OutputTextBox, 0);
                } else {
                    SecondRowDefinition.Height = new(1, GridUnitType.Star);
                    SecondColumnDefinition.Width = new(0);
                    Grid.SetColumn(OutputTextBox, 0);
                    Grid.SetRow(OutputTextBox, 1);
                }
            });
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
    /// 复制
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        CommonUITools.Widget.MessageBox.Success("已复制");
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GenerateClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (RouterService.GetInstance(ContentFrame.CurrentSourcePageType) is IGenerable<uint, IEnumerable<string>> generator) {
            OutputText = string.Join('\n', generator.Generate((uint)GenerateCount));
        }
    }
}
