using System.Windows.Navigation;

namespace CommonUtil.View;

public partial class RandomGeneratorView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly IReadOnlyList<NavigationItemInfo> NavigationItemInfos = new List<NavigationItemInfo> {
        new ("生成字符串") {
            Name = typeof(RandomStringGeneratorView).Name,
            Icon = "\ue687",
            IconColor = "#C8834E"
        },
        new ("生成随机数") {
            Name = typeof(RandomNumberGeneratorView).Name,
            Icon = "\ue610",
            IconColor = "#1296DB"
        },
        new ("根据正则生成") {
            Name = typeof(RandomGeneratorWithRegexView).Name,
            Icon = "\ue647",
            IconColor = "#C84E64"
        },
        new ("根据数据源生成") {
            Name = typeof(RandomGeneratorWithDataSourceView).Name,
            Icon = "\ue6ed",
            IconColor = "#22ADAC"
        },
        new ("生成随机选项") {
            Name = typeof(RandomChoiceGeneratorView).Name,
            Icon = "\ufc5a",
            IconColor = "#22ADAC"
        },
        new ("生成随机日期时间") {
            Name = typeof(RandomDateTimeGeneratorView).Name,
            Icon = "\ue613",
            IconColor = "#22ADAC"
        },
        new ("生成随机邮箱地址") {
            Name = typeof(RandomEmailAddressGeneratorView).Name,
            Icon = "\ue620",
            IconColor = "#22ADAC"
        },
        new ("生成随机 GUID") {
            Name = typeof(RandomGuidGeneratorView).Name,
            Icon = "\ue74c",
        },
        new ("生成随机 IPV4") {
            Name = typeof(RandomIPV4AddressGeneratorView).Name,
            Icon = "\ue67f",
        },
        new ("生成随机 IPV6") {
            Name = typeof(RandomIPV6AddressGeneratorView).Name,
            Icon = "\ue67f",
        },
        new ("生成随机 MAC 地址") {
            Name = typeof(RandomMACAddressGeneratorView).Name,
            Icon = "\uec89",
        },
        new ("生成随机中文名") {
            Name = typeof(RandomChineseNameGeneratorView).Name,
            Icon = "\ue712",
            IconColor = "#c84e64"
        },
        new ("生成随机中文姓氏") {
            Name = typeof(RandomChineseFamilyNameGeneratorView).Name,
            Icon = "\ue712",
            IconColor = "#c84e64"
        },
        new ("生成随机古代中文名") {
            Name = typeof(RandomChineseAncientNameGeneratorView).Name,
            Icon = "\ue712",
            IconColor = "#c84e64"
        },
        new ("生成随机词语") {
            Name = typeof(RandomChineseWordGeneratorView).Name,
            Icon = "\ue712",
            IconColor = "#c84e64"
        },
        new ("生成随机日本姓名") {
            Name = typeof(RandomJapaneseNameGeneratorView).Name,
            IconImage = ImagePath.JapaneseSymbolImageUri
        },
        new ("生成随机日本姓氏") {
            Name = typeof(RandomJapaneseFamilyNameGeneratorView).Name,
            IconImage = ImagePath.JapaneseSymbolImageUri
        },
        new ("生成随机英文姓名") {
            Name = typeof(RandomEnglishNameGeneratorView).Name,
            Icon = "\ue713",
            IconColor = "#1296db"
        },
        new ("生成随机英文音译名") {
            Name = typeof(RandomEnglishChineseNameGeneratorView).Name,
            Icon = "\ue713",
            IconColor = "#1296db"
        },
        new ("生成随机英文单词") {
            Name = typeof(RandomEnglishWordGeneratorView).Name,
            Icon = "\ue713",
            IconColor = "#1296db"
        },
    };
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(RandomGeneratorView), new PropertyMetadata(""));
    public static readonly DependencyProperty GenerateCountProperty = DependencyProperty.Register("GenerateCount", typeof(double), typeof(RandomGeneratorView), new PropertyMetadata(16.0));
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

    public RandomGeneratorView() {
        InitializeComponent();
        RouterService = new(ContentFrame, Routers);
        // Delay loading NavigationView
        this.SetLoadedOnceEventHandler(async (_, _) => {
            await Task.Delay(500);
            NavigationUtils.InitializeNavigationViewItems(NavigationView, NavigationItemInfos);
            NavigationView.SelectedItem ??= NavigationView.MenuItems[0];
        });
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

    private void ContentFrameNavigatedHandler(object sender, NavigationEventArgs e) {
        if (RouterService.CurrentPage is not FrameworkElement element) {
            return;
        }
        // Update description width
        element.SetLoadedOnceEventHandler((_, _) => {
            GenerationCountTextBlock.ClearValue(WidthProperty);
            GenerationCountTextBlock.UpdateLayout();
            var newWidth = Math.Max(RandomGeneratorHelper.DescriptionWidth, GenerationCountTextBlock.ActualWidth);
            GenerationCountTextBlock.Width = newWidth;
            RandomGeneratorHelper.UpdateDescriptionWidth(newWidth);
        });
    }
}
