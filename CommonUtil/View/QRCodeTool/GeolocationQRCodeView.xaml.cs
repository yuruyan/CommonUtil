using CommonUtil.Core.Model;

namespace CommonUtil.View;

public partial class GeolocationQRCodeView : Page, IGenerable<KeyValuePair<QRCodeFormat, QRCodeInfo>, Task<byte[]>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty LongitudeProperty = DependencyProperty.Register("Longitude", typeof(double), typeof(GeolocationQRCodeView), new PropertyMetadata(0.0));
    public static readonly DependencyProperty LatitudeProperty = DependencyProperty.Register("Latitude", typeof(double), ownerType: typeof(GeolocationQRCodeView), new PropertyMetadata(0.0));

    /// <summary>
    /// 经度
    /// </summary>
    public double Longitude {
        get { return (double)GetValue(LongitudeProperty); }
        set { SetValue(LongitudeProperty, value); }
    }
    /// <summary>
    /// 纬度
    /// </summary>
    public double Latitude {
        get { return (double)GetValue(LatitudeProperty); }
        set { SetValue(LatitudeProperty, value); }
    }

    public GeolocationQRCodeView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成二维码
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    Task<byte[]> IGenerable<KeyValuePair<QRCodeFormat, QRCodeInfo>, Task<byte[]>>.Generate(KeyValuePair<QRCodeFormat, QRCodeInfo> arg) {
        double longitude = Longitude;
        double latitude = Latitude;
        return Task.Run(() => QRCodeTool.GenerateQRCodeForGeolocation(
            longitude,
            latitude,
            arg.Value,
            arg.Key
        ));
    }
}
