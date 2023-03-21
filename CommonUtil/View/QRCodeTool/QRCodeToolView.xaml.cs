using ModernWpf.Controls;

namespace CommonUtil.View;

public partial class QRCodeToolView {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly Type[] Routers = {
        typeof(QRCodeDecodeView),
        typeof(QRCodeGeneratorView),
        typeof(URLQRCodeView),
        typeof(SMSQRCodeView),
        typeof(WIFIQRCodeView),
        typeof(MailQRCodeView),
        typeof(PhoneNumberQRCodeView),
        typeof(GeolocationQRCodeView),
    };
    private readonly RouterService RouterService;

    public QRCodeToolView() {
        InitializeComponent();
        RouterService = new(ContentFrame, Routers);
        NavigationUtils.EnableNavigationPanelResponsive(NavigationView);
    }

    /// <summary>
    /// 导航变化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void NavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
        if (args.SelectedItem is not FrameworkElement element) {
            return;
        }
        Type targetType = Routers.First(t => t.Name == element.Name);
        // Decode View
        if (element.Name == typeof(QRCodeDecodeView).Name) {
            RouterService.Navigate(typeof(QRCodeDecodeView));
        } else {
            RouterService.Navigate(typeof(QRCodeGeneratorView), targetType);
        }
    }
}
