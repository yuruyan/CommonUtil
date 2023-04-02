namespace CommonUtil.View;

public partial class RandomGeneratorView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(RandomGeneratorView), new PropertyMetadata(""));
    public static readonly DependencyProperty GenerateCountProperty = DependencyProperty.Register("GenerateCount", typeof(double), typeof(RandomGeneratorView), new PropertyMetadata(16.0));

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

    private static readonly Type[] Routers = {
        typeof(RandomStringGeneratorView),
        typeof(RandomNumberGeneratorView),
        typeof(RandomGeneratorWithRegexView),
        typeof(RandomGeneratorWithDataSourceView),
        typeof(RandomChoiceGeneratorView),
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
    }

    protected override void IsExpandedPropertyChangedHandler(ResponsivePage self, DependencyPropertyChangedEventArgs e) {
        if (e.NewValue is true) {
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
    }

    /// <summary>
    /// 复制
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        MessageBoxUtils.Success("已复制");
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

    private void ViewLoadedHandler(object sender, RoutedEventArgs e) {
        NavigationUtils.EnableNavigation(
            NavigationView,
            RouterService,
            ContentFrame
        );
        NavigationUtils.EnableNavigationPanelResponsive(NavigationView);
    }

    private void ViewUnloadedHandler(object sender, RoutedEventArgs e) {
        NavigationUtils.DisableNavigation(NavigationView);
        NavigationUtils.DisableNavigationPanelResponsive(NavigationView);
    }
}
